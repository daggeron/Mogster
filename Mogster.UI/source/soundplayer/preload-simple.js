const { ipcRenderer } = require('electron');

require = null;

ipcRenderer.on('PlaySound', (event, data) => {
	const event2 = new CustomEvent('PlaySound', { detail: data });
	window.dispatchEvent(event2);
});
