'use strict';

const { ipcMain } = require('electron');
const { BrowserWindow } = require('electron');
const rootPath = require('electron-root-path').rootPath;
const log = require('electron-log');

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
            transparent: true,
            alwaysOnTop: false
        });

        this.loadFile('./src/overlay/sound/sound.html');

        this.once('ready-to-show', () => {
            this.wireEvents();
        })
    }

    wireEvents() {
        log.info("Wiring events");

        ipcMain.on("PlaySound", sound => {
            log.info("PlaySound [",sound,"]");
            this.webContents.send("PlaySound", sound);
        });

        ipcMain.on("StopSound", sound => {
            log.info("StopSound");
            this.webContents.send("StopSound", sound);
        });
    }
}

module.exports = SoundWindow;
