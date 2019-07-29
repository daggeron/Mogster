//require('child_process').fork('./sandbox/sandbox.js', ['test'], { stdio: ['pipe', 'pipe', 'pipe', 'ipc'] });
const HeadlessSandBox = require('./sandbox/headless_sandbox').HeadlessSandbox;
const rootPath = require('electron-root-path').rootPath;
const path = require('path');
const log = require('electron-log');
log.info(rootPath);
this.headlessSandbox = new HeadlessSandBox(path.resolve(rootPath, 'src/sandbox'), path.resolve(rootPath, 'scripts'));

this.headlessSandbox.start();