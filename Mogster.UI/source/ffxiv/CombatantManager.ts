import { Combatant } from './Combatant';
import { resolve } from 'path';

import * as edge from 'electron-edge-js';

/*
 * Note: This may change in the future.  
 * However, from the end user perspective it doesn't make a difference.
 * 
 * Note: Edgejs instantiates a new instance each time we bind edge.func
 */
export class CombatantManager {
    rootPath: string;
    getCombatantFunction: edge.Func<number, Combatant>;    
    //getCombatantListFunction: edge.Func<unknown, unknown>;
    getCurrentPlayerFunction: edge.Func<any, Combatant>; 


    constructor(rootPath:string) {
        this.rootPath = rootPath;
        this.getCombatantFunction = edge.func({
            assemblyFile: resolve(rootPath, 'plugins/Mogster.Core.dll'),
            typeName: 'Mogster.Core.EdgeBindings.CombatantManagerReader',
            methodName: 'GetCombatant'
        });

        this.getCurrentPlayerFunction = edge.func({
            assemblyFile: resolve(rootPath, 'plugins/Mogster.Core.dll'),
            typeName: 'Mogster.Core.EdgeBindings.CombatantManagerReader',
            methodName: 'GetCurrentPlayer'
        });
        /*this.getCombatantListFunction = edge.func({
            assemblyFile: resolve(rootPath, 'plugins/Mogster.Core.dll'),
            typeName: 'Mogster.Core.Program',
            methodName: 'GetCombatantList'
        });*/
    }

    getCurrentPlayer(): Combatant {
        return new Combatant(this.getCurrentPlayerFunction('', true));
    }

    getCombatant(id: number) {
        return new Combatant(this.getCombatantFunction(id, true));
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

