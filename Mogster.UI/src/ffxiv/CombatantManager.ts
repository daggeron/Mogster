//import { Combatant } from './Combatant';
//import { resolve } from 'path';

//import * as edge from 'electron-edge-js';


export class CombatantManager {
    rootPath: string;
    /*getCombatantFunction: edge.Func<unknown, unknown>;    
    getCombatantListFunction: edge.Func<unknown, unknown>; 
    getCurrentPlayerFunction: edge.Func<unknown, unknown>; */


    constructor(rootPath:string) {
        this.rootPath = rootPath;
        /*this.getCombatantFunction = edge.func({
            assemblyFile: resolve(rootPath, 'plugins/Mogster.Core.dll'),
            typeName: 'Mogster.Core.Program',
            methodName: 'GetCombatant'
        });
        this.getCurrentPlayerFunction = edge.func({
            assemblyFile: resolve(rootPath, 'plugins/Mogster.Core.dll'),
            typeName: 'Mogster.Core.Program',
            methodName: 'GetCurrentPlayer'
        });
        this.getCombatantListFunction = edge.func({
            assemblyFile: resolve(rootPath, 'plugins/Mogster.Core.dll'),
            typeName: 'Mogster.Core.Program',
            methodName: 'GetCombatantList'
        });*/
    }

    getCurrentPlayer() {
        //let $engine = this;
        /*return new Promise<Combatant>(function (resolve, reject) {
            $engine.getCurrentPlayerFunction('', function (error, result: any) {
                if (error) {
                    reject(error);
                } else {
                    if (result === null) {
                        reject(new Error("Could not find you!"));
                        return;
                    }
                    let combatant = new Combatant(result);
                    resolve(combatant);
                }
            });
        });*/
    }

    getCombatant(id: number) {
        //let $engine = this;
        /*return new Promise<Combatant>(function (resolve, reject) {
            $engine.getCombatantFunction(id, function (error, result: any) {
                if (error) {
                    reject(error);
                } else {
                    if (result === null) {
                        reject(new Error("Null combatant"));
                        return;
                    }
                    let combatant = new Combatant(result);
                    resolve(combatant);
                }
            });
        });*/
    }

    getCombatantList() {
        //let $engine = this;
        /*return new Promise<Combatant>(function (resolve, reject) {
            $engine.getCombatantFunction('', function (error, result: any) {
                if (error) {
                    reject(error);
                } else {
                    let combatant = new Combatant(result);
                    resolve(combatant);
                }
            });
        });*/
    }

}

