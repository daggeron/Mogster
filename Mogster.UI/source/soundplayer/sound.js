'use strict';

const { ipcMain } = require('electron');
const { BrowserWindow } = require('electron');
const path = require('path');
const rootPath = require('electron-root-path').rootPath;

const { createLogger, format, transports } = require('winston');
const { combine, timestamp, label, printf } = format;

const myFormat = printf(({ level, message, label, timestamp }) => {
    return `${timestamp} [${label}] ${level}: ${message}`;
});

const log = createLogger({
    format: combine(
        format.timestamp({ format: 'HH:mm:ss' }),
        format.colorize({ all: true }),
        format.prettyPrint(),
        label({ label: 'Sound Window' }),
        timestamp(),
        myFormat
    ),
    transports: [new transports.Console()]
});

class SoundWindow extends BrowserWindow {

    constructor() {
        super({
            width: 1,
            height: 1,
            resizable: false,
            focusable: false,
            fullscreenable: false,
            show: false,
            frame: false,
            transparent: false,
            alwaysOnTop: false
        });

        this.loadFile('./dist-js/soundplayer/sound.html');

        //This will not fire if show is set to true
        this.once('ready-to-show', () => {
            this.wireEvents();
        });
    }

    wireEvents() {
        log.debug("Wiring events");

        ipcMain.on("PlaySound", sound => {
            log.debug("PlaySound [",sound,"]");
            this.webContents.send("PlaySound", sound);
        });

        ipcMain.on("StopSound", sound => {
            log.debug("StopSound");
            this.webContents.send("StopSound", sound);
        });
    }
}

module.exports = SoundWindow;
