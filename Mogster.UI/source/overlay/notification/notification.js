/**
 * Created by Ji on 9/15/16.
 */

'use strict';
const path = require('path');

const { BrowserWindow } = require('electron');
var Positioner = require('electron-positioner');

class NotificationWindow {

	constructor() {
		this.notificationWindow = null;
		this.createNotificationWindow();
	}

	pushEvent(event, data) {
		this.notificationWindow.webContents.send(event, data);
	}

	createNotificationWindow() {
		this.notificationWindow = new BrowserWindow({
			width: 350,
			height: 500,
			resizable: false,
			skipTaskbar: true,
			// fullscreenable: false,
			webPreferences: {
				devTools: false,
				contextIsolation: true,
				nodeIntegration: false,
				backgroundThrottling: false,
				preload: path.join(__dirname, 'preload-simple.js')
			},
			show: true,
			frame: false,
			transparent: true,
			alwaysOnTop: true
		});
		var positioner = new Positioner(this.notificationWindow);
		positioner.move('topRight');

		this.initWindowEvents();
		this.notificationWindow.setIgnoreMouseEvents(true);
		this.notificationWindow.loadURL(`file://${path.join(__dirname, '/notification.html')}`);
	}

	initWindowEvents() {
		this.notificationWindow.on('close', () => {
			this.notificationWindow = null;
			this.isShown = false;
		});
	}

	show() {
		if (!this.notificationWindow)
			this.createNotificationWindow();


		if (!this.isShown) {
			this.notificationWindow.show();
			this.isShown = true;
		}
	}

	hide() {
		if (this.isShown) {
			this.notificationWindow.hide();
			this.isShown = false;
		}
	}

}

module.exports = NotificationWindow;
