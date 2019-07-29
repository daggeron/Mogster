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
const Store = require('electron-store');

const store = new Store({
    schema: {
        "discord": {
            "use": { "type": "boolean", "default":"false" },
            "token": { "type": "string", "default": "changeme" },
            "guild": { "type": "string", "default": "changeme" },
            "channel": { "type": "string", "default": "changeme" }
        }
    }, 
    cwd: path.resolve(rootPath, 'config')
});

const { fork } = require('child_process');
//const Notifications = require('./overlay/notification/notification');
const SoundWindow = require('./soundplayer/sound');

/*if (store.get("discord.use")) {
    this.VoiceClient = require('./discord/fork').VoiceClient;
}*/

//process.env.EDGE_APP_ROOT = path.resolve(rootPath, 'plugins');
//process.env.EDGE_USE_CORECLR = 1;
//This needs to be updated`

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

        if (store.get("discord.use"))
            this.startDiscord();
    }

    async startDiscord() {
        let token = store.get("discord.token");
        let channelID = store.get("discord.channel");
        let guildID = store.get("discord.guild");

        this.voiceClient = fork(path.resolve(rootPath, 'discord/client.js'),
            [token, path.resolve(rootPath, 'audio'), channelID, guildID], {
                stdio: ['inherit', 'inherit', 'inherit', 'ipc']
            });

        var restartMethod = this.reloadSandbox.bind(this);
        this.voiceClient.on('exit', function (code) {
            if (store.get("discord.use")) {
                log.error("HeadlessSandbox process terminated unexpectedly. Restarting in 5 seconds.")
                setTimeout(restartMethod, 5000);
            } else {
                this.voiceClient = null;
                ipcMain.removeListener('Discord', discordListener);
            }

        })
        let discordListener = (data) => {
            log.info('discord ', data);
            this.voiceClient.send(data);
        }
        ipcMain.on('Discord', discordListener);

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
            this.voiceClient.send({ 'event': 'RemoteShutdown', 'payload': 'ignored' });

            setTimeout(() => { this.voiceClient.kill() }, 2000);
        }
    }


    async startSandbox() {
        log.debug('Forking');
        this.forkProcess = fork(path.resolve(app.getAppPath(),'dist-js', 'sandbox/sandbox.js'),
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

            this.tray = new Tray(path.resolve(__dirname,'../', 'static/moogle.ico'));
            const menu = Menu.buildFromTemplate([
                {
                    label: 'Reload Scripts', click: (item, window, event) => {
                        log.info('Bouncing child sandbox');
                        this.reloadSandbox();
                    }
                },
                {
                    label: 'Stop local Audio Playback', click: (item, window, event) => {
                        ipcMain.emit("Discord", { 'event': 'RemoteStop', 'payload': '' });
                        ipcMain.emit("StopSound");
                    }
                },
                {
                    label: "Discord", submenu: [{
                        label: 'Use Discord', type: 'checkbox', checked: store.get("discord.use"), click: (item, window, event) => {
                            if (item.checked) {
                                store.set("discord.use", true)
                                this.startDiscord();
                            } else {
                                store.set("discord.use", false)
                                this.shutdownDiscord();
                            } 
                        } 
                    }, {
                            label: 'Stop local Audio Playback', click: (item, window, event) => {
                                ipcMain.emit("Discord", { 'event': 'RemoteStop', 'payload': '' });
                                ipcMain.emit("StopSound");
                            }
                        }, {
                            label: 'Play Test Sound', click: (item, window, event) => {
                                ipcMain.emit("Discord", { 'event': 'RemotePlay', 'payload': 'belltollnightelf.ogg' });
                            }
                        }]
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


