import { readdirSync, existsSync, readFileSync } from 'fs';
import { resolve, extname } from 'path';
import { NodeVM, VMScript } from 'vm2';
import { EventEmitter2 } from 'eventemitter2';

import { Combatant, CombatantManager } from '../ffxiv/bindings';
import * as edge  from 'electron-edge-js';

import * as winston from 'winston';

const { combine, timestamp, label, printf } = winston.format;

const myFormat = printf(({ level, message, label, timestamp }) => {
    return `${timestamp} [${label}] ${level}: ${message}`;
});

const log = winston.createLogger({
    format: combine(
        winston.format.timestamp({ format: 'HH:mm:ss' }),
        winston.format.colorize({ all: true }),
        winston.format.prettyPrint(),
        label({ label: 'Sandbox' }),
        timestamp(),
        myFormat
    ),
    transports: [new winston.transports.Console()]
});

const scriptLog = winston.createLogger({
    format: combine(
        winston.format.timestamp({ format: 'HH:mm:ss' }),
        winston.format.colorize({ all: true }),
        winston.format.prettyPrint(),
        label({ label: 'Sandboxed Script' }),
        timestamp(),
        myFormat
    ),
    transports: [new winston.transports.Console()]
});

if (process.send === undefined) {
    log.error("Either we were called directly or something bad happened.");
    process.exit(-1);
}

var scriptsFolder = process.argv[2];
var rootPath = resolve(scriptsFolder, '../');

const ZoneReader = edge.func({
    assemblyFile: resolve(rootPath, 'plugins/Mogster.Core.dll'),
    typeName: 'Mogster.Core.EdgeBindings.ZoneRead'
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
            throw `Could not locate specified path [${folder}]`;
        }

        this.listDirectories(folder).forEach((directory:any) => {

            let scriptPath = resolve(folder, directory, 'index.js');

            if (existsSync(scriptPath)) {
                this.wrap(scriptPath, resolve(folder, directory), folder, directory);
            }
        });
    }

    async wrap(script: string, directory: string, rootDirectory: string, moduleName: string) {
        let vm = new NodeVM({
            console: 'redirect',
            require: {
                context: 'sandbox',
                resolve: (moduleName: any) => {
                    return resolve(rootDirectory, 'libs', moduleName);
                },
                external: true,
                builtin: ['assert'],
                root: rootDirectory,
                mock: {
                    helper: {
                        getCurrentZone() {
                            return ZoneReader('', true);
                        },
                        wrapCombatant(combatant: Combatant) {
                            return new Combatant(combatant);
                        },
                        sendEvent(event: string, payload: string) {
                            (<any>process).send({ 'event': event, 'payload': payload });
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

        vm.on('console.log', (...args: any) => {
            let message: string = '';

            args.forEach((element:any) => {
                let elementType:string = typeof element;
                switch (elementType) {
                    case 'string':
                    case 'number':
                        message += element; 
                        break;
                    default:
                        message += JSON.stringify(element);
                        break;
                }
            });

            scriptLog.info(`[${moduleName}]: ${message}`);
        });

        vm.on('console.debug', (...args:any) => {
            scriptLog.debug(`[${moduleName}]: ` + args.join(''));
        });

        vm.on('console.error', (...args:any) => {
            scriptLog.error(`[${moduleName}]: ` + args.join(''));
        });

        try {
            let script2 = new VMScript(readFileSync(script).toString(), directory);
            vm.run(script2, script);
        } catch (ex) {
            log.error(JSON.stringify(ex, ["stack"], '\t'));

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

sandbox.emitter.onAny(function (event:any, value:any, inbound:any) {
    if (inbound !== true) {
        log.info(event, value);
        (<any>process).send({ 'event': event, 'payload': value });
    }
});
