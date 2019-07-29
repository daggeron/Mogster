import { readdirSync, existsSync, readFileSync } from 'fs';
import { resolve, extname } from 'path';
import { NodeVM, VMScript } from 'vm2';
import { EventEmitter2 } from 'eventemitter2';
import * as winston from 'winston';
//import { observe } from 'mobx';
import {  CombatantManager } from '../ffxiv/bindings';
import * as edge  from 'electron-edge-js';





if (process.send === undefined) {
    console.log("Either we were called directly or something bad happened.");
    process.exit(-1);
}

var scriptsFolder = process.argv[2];
var rootPath = resolve(scriptsFolder, '../');

const ZoneReader = edge.func({
    assemblyFile: resolve(rootPath, 'plugins/Mogster.Core.dll'),
    typeName: 'Mogster.Core.EdgeBindings.ZoneRead'
});

const log: winston.Logger = winston.createLogger({
    level: 'info',
    format: winston.format.combine(
        winston.format.simple(),
        winston.format.timestamp()),
    transports: [
        new winston.transports.File({ filename: resolve(scriptsFolder, 'error.log'), level: 'error' }),
        new winston.transports.File({ filename: resolve(scriptsFolder, 'combined.log') }),
        new winston.transports.Console({ format: winston.format.simple() })
    ]
});

//embedding it here as asar is a pita.
class HeadlessSandBox {

    eventEmitter: EventEmitter2;
    CombatantManager: CombatantManager;

    constructor() {
        this.eventEmitter = new EventEmitter2({ wildcard: true });

        this.CombatantManager = new CombatantManager(rootPath);

        process.on('uncaughtException', (err) => {
            log.error('Asynchronous error caught.', err);
        })
    }

    get emitter(): EventEmitter2 {
        return this.eventEmitter;
    }

    listDirectories(source:string) {
        return readdirSync(source, { withFileTypes: true })
            .filter(dirent => dirent.isDirectory())
            .map(dirent => dirent.name);
    }

    async scan(folder:string) {
        if (!existsSync(folder)) {
            throw "Could not locate specified path [" + folder + "]";
        }
        this.listDirectories(folder).forEach((directory) => {

            let scriptPath = resolve(folder, directory, 'index.js');

            //log.info(scriptPath);

            if (existsSync(scriptPath)) {
                this.wrap(scriptPath, resolve(folder, directory), folder);
            }
        });
    }
    resolveModule(location:string) {

    }

    async wrap(script: string, directory: string, rootDirectory: string) {
        let vm = new NodeVM({
            console: 'inherit',
            sandbox: { EventEmitter2 },
            require: {
                context: 'host',
                resolve: moduleName => resolve(rootDirectory, 'libs', moduleName.toString()),
                external: ['eventemitter2', 'mobx'],
                root: rootDirectory,
                mock: {
                    helper: {
                        getCurrentZone() {
                            return ZoneReader('', true);
                        }
                    },
                    sandboxfs: {
                        resolve(...pathSegments: string[]) {
                            if (pathSegments.length === 0) {
                                throw new Error("No path supplied");
                            } else {
                                return resolve(rootDirectory, ...pathSegments);
                            }
                        },
                        readdir(path: string) {
                            return readdirSync(resolve(rootDirectory, path)).filter(filename => {
                                return extname(filename) == '.js';
                            });
                        }
                    }
                }
            }
        });

        vm.freeze(this.eventEmitter, 'emitter');
        vm.freeze(this.CombatantManager, "CombatantManager");

        try {
            let script2 = new VMScript(readFileSync(script).toString(), directory);
            vm.run(script2, script);
        } catch (ex) {
            console.log(ex);
            log.error(ex);

            (<any>process).send({ 'internal_event': 'error', 'payload': "Hello" });
        }
    }
}


var sandbox = new HeadlessSandBox();

sandbox.scan(scriptsFolder).catch(error => {
    log.error("Everything broke", error);
});

process.on('message', message => {
    if (message.hasOwnProperty("event")) {
        sandbox.emitter.emit(message.event, message.payload, true);
    }
});

sandbox.emitter.onAny(function (event, value, inbound) {
    if (inbound !== true) {
        (<any>process).send({ 'event': event, 'payload': value });
    }
});
