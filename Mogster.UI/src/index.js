'use strict';
const { app, ipcMain, Menu, Tray } = require('electron');

console.log('');
console.log("DEV WARNING!!!! Sandbox is currently disabled!!");

if (!app.requestSingleInstanceLock())
    app.quit();

//app.enableSandbox();

const process = require('process');
const rootPath = require('electron-root-path').rootPath;
const log = require('electron-log');
const path = require('path');

const config = require('config');
const { fork } = require('child_process');
//const Notifications = require('./overlay/notification/notification');
const SoundWindow = require('./overlay/sound/sound');

process.env["NODE_CONFIG_DIR"] = path.resolve(rootPath, "config");

this.VoiceClient = {};
var useDiscord = false;

if (config.has("discord.use") && config.get("discord.use")) {
    this.VoiceClient = require('./discord/fork').VoiceClient;
    this.useDiscord = true;
}
//process.env.EDGE_APP_ROOT = path.resolve(rootPath, 'plugins');
//process.env.EDGE_USE_CORECLR = 1;
//This needs to be updated
var edge = require('electron-edge-js');
var ipcBridge = edge.func({
    assemblyFile: path.resolve(rootPath, 'plugins/Mogster.Core.dll'),
    typeName: 'Mogster.Core.Program',
    methodName: 'BindEventLoop'
});

class MogsterUI {

    constructor() {
        //this.notificationWindow = null;
        this.soundWindow = null;

        this.forkProcess = null;
        this.shuttingDown = false;
    }

    async init() {
        this.initApp();
        this.initIPC();
        await this.startSandbox();
        if (useDiscord)
            this.startDiscord();
    }

    setupDiscordBot() {
        try {
            let token = config.get("discord.token");
            log.debug('Forking');

            this.voiceClient = new VoiceClient(app.getAppPath(), token, path.resolve(rootPath, "audio"));

            this.voiceClient.start();

            this.voiceClient.onAny(function (event, payload) {

            });

            ipcMain.on('Discord', (data) => {
                this.voiceClient.emit(data.event, data.payload);
            });

            log.debug('Done forking');
        } catch (exception) {
            log.error(exception);
        }
    }

    async startDiscord() {
        //let token = config.get("discord.token");
        let token = 'ReplaceME';
        let channelID = '999999999999999';
        let guildID = '999999999999999';

        this.voiceClient = fork(path.resolve(rootPath, 'discord/client.js'),
            [token, path.resolve(rootPath, 'audio'), channelID, guildID], {
                stdio: ['inherit', 'inherit', 'inherit', 'ipc']
            });

        var restartMethod = this.reloadSandbox.bind(this);
        this.voiceClient.on('exit', function (code) {
            log.error("HeadlessSandbox process terminated unexpectedly. Restarting in 5 seconds.")
            setTimeout(restartMethod, 5000);
        })

        ipcMain.on('Discord', (data) => {
            log.info('discord ', data);
            this.voiceClient.send(data);
        });

        this.voiceClient.on('message', data => {

        });

        log.debug('Done forking');
    }

    restartDiscord() {
        this.shutdownSandbox();
        this.startSandbox();
    }

    async shutdownDiscord() {
        log.debug("Shutting down DiscordBot");

        if (this.voiceClient !== null) {
            this.voiceClient.send({ 'event': 'DisconnectFromDiscord', 'payload': 'ignored' });

            await new Promise(resolve => setTimeout(resolve, 9500));
            this.voiceClient.removeAllListeners();
            this.voiceClient.kill();
        }
    }


    async startSandbox() {
        log.debug('Forking');
        this.forkProcess = fork(path.resolve(app.getAppPath(),'distjs', 'src/sandbox/sandbox.js'),
            [path.resolve(rootPath, 'scripts')], {
                stdio: ['inherit', 'inherit', 'inherit', 'ipc']
            });

        var restartMethod = this.reloadSandbox.bind(this);
        this.forkProcess.on('exit', function (code) {
            log.error("HeadlessSandbox process terminated unexpectedly. Restarting in 5 seconds.")
            setTimeout(restartMethod, 5000);
        })

        this.forkProcess.on('message', data => {
            log.info("Message received");

            if (data.hasOwnProperty("internal_event")) {
                log.error(data);
                return;
            }

            /*
             * Drop invalid messages 
             */
            if (!data.hasOwnProperty("event")) {
                log.warn("Invalid Message Received", data);
                return;
            }

            let event = data.event;
            let payload = data.payload;
            
            //this.notificationWindow.pushEvent(event, payload);
            //this.soundWindow.pushEvent(event, payload);
            
            //Discord may or may not be loaded.
            switch (event) {
                case 'RemotePlay':
                case 'RemoteStop':
                    ipcMain.emit("Discord", { 'event': event, 'payload': payload });
                    break;
                case 'PlaySound':
                case "StopSound":
                    ipcMain.emit(event, payload);
                    break;
            }
        });

        log.debug('Done forking');
    }

    reloadSandbox() {
        this.shutdownSandbox();
        this.startSandbox();
    }

    shutdownSandbox() {
        log.debug("Shutting down HeadlessSandbox");
        if (this.forkProcess !== null) {
            this.forkProcess.removeAllListeners();
            this.forkProcess.kill();
        }
    }

    async initApp() {
        app.on('before-quit',  (event) => {
            this.shutdownDiscord();
            
            this.shutdownSandbox();

        });

        app.on('ready', () => {
            this.createNotificationWindow();

            this.tray = new Tray(path.resolve(__dirname,'../../', 'static/moogle.ico'));
            const menu = Menu.buildFromTemplate([
                {
                    label: 'Reload Scripts', click: (item, window, event) => {
                        log.info('Bouncing child sandbox');
                        this.reloadSandbox();
                    }
                },
                { type: 'separator' },
                { role: 'quit' } // "role": system prepared action menu
            ]);
            this.tray.setToolTip('Mogster');

            this.tray.setContextMenu(menu);
        });

        app.on('activate', () => {
            //this.notificationWindow.show();
        });
    }

    async initIPC() {
        ipcBridge(message => {
            const data = JSON.parse(message);
            ipcMain.emit('data_event', data);
        });

        ipcMain.on('data_event', (data) => {
            this.forkProcess.send(data);
        });
    }

    createNotificationWindow() {
        //this.notificationWindow = new Notifications();
        this.soundWindow = new SoundWindow();
    }
}

const mogsterUI = new MogsterUI();
mogsterUI.init();


