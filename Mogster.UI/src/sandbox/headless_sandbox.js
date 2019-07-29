const { fork } = require('child_process');
const EventEmitter = require('eventemitter2');
const path = require('path');
const log = require('electron-log');


class HeadlessSandbox extends EventEmitter {
    constructor(rootPath, scriptsFolder) {
        super();

        this.forkProcess = null;
        this.rootPath = rootPath;
        this.scriptsFolder = scriptsFolder;
    }

    async start() {

        this.forkProcess = fork(path.resolve('app.asar/src/sandbox/sandbox.js'),
            [this.scriptsFolder], {
                stdio: ['pipe', 'pipe', 'pipe', 'ipc']
            });

        var restartMethod = this.reload.bind(this);
        this.forkProcess.on('exit', function (code) {
            log.error("HeadlessSandbox process terminated unexpectedly. Restarting in 5 seconds.")
            setTimeout(restartMethod, 5000);
        })

        this.forkProcess.on('message', data => {
            this.emit(data.event, data.payload, true);
        });

        this.onAny(function (event, value, inbound) {
            if (inbound !== true) {
                process.send({ 'event': event, 'payload': value });
            }
        });
    }

    reload() {
        this.shutdown();
        this.start();
    }

    shutdown() {
        log.debug("Shutting down HeadlessSandbox");
        if (this.forkProcess !== null) {
            this.forkProcess.removeAllListeners();
            this.forkProcess.kill();
        }
    }
}

module.exports.HeadlessSandbox = HeadlessSandbox;