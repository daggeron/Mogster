

"use strict";

/*
 * This is a proof of concept template and is not complete.
 * 
 * Nothing in here is finalized.
 * 
 */

const { EventEmitter2 } = require("eventemitter2")
const { Combatant } = require('../../libs/combatant');
const { observe, decorate, observable } = require('mobx');

function Send(emitter, event, payload) {
    emitter.emit(event, payload, true);
}



class SeiryuEX {
    constructor(remoteEmitter) {
        this.eventEmitter = remoteEmitter;
       
        this.eventEmitter.on('CombatantAdded', (combatant) => {
            console.log("Hello");
            if (combatant.Name === "Seiryu" && combatant.Level === 46) {
                console.log("I think I found the correct one");
                console.log(combatant);
            }
            
        });

    }

    get zone() {
        return 825;
    }

    static get EncounterDetails() {
        return { id: "Wreath of Snakes EX", zone: 825 };
    }

    get emitter() {
        return this.eventEmitter;
    }

    /*
     * Setup an event to watch for to know when the fight has started.
     * Until we get Sharlayan back up this is what we have to do.
     */
    watchStart() {
        console.log("Binding event emitter xxx");
        //Titania emits the following line over chat when we engage her
        //"Come and play! For the night is bright, and you can sleep when you're dead!"
        const watcherCallback = effect => {
            console.log(effect);
            if (effect.BuffID === 76) {
                this.start();
                this.emitter.removeListener("EffectAdded", watcherCallback);
            }
        };

        console.log("blah");
        /*var disposeMe = observe(player, 'MaxHP', async (change) => {
            console.log(change.type, "MaxHP from", change.oldValue, "to", change.newValue);
            disposeMe.dispose();
        });*/
        console.log("blah");
        //this.eventEmitter.on("EffectAdded", watcherCallback);
        this.start();
        console.log("blah");
        this.wireNeededEvents();
        console.log("blah");
    }

    /*
     * Start of the encounter.
     */
    start() {
        //Removes the listener we used to monitor chat for the start event.
        //Saves resources
        Send(this.emitter, "PlaySound", 'belltollnightelf.ogg');
        console.log(`Started encounter ${SeiryuEX.EncounterDetails.id}`);
        this.emitter.on("DutyLockoutUpdate", data => {
            console.log("Duty Lockout update.  Time left: ", data.Time);
        });
    }

    /*
     * End of the encounter
     * 
     */
    stop() {
        this.emitter.removeAllListeners();
    }

    /*
     * Wipe
     */
    wipe() {
        console.log("Wipe detected");
        this.stop();
        this.watchStart();
    }

    /*
     * Wire up all of the events we need to function.
     */
    wireNeededEvents() {
        this.eventEmitter.on("DutyComplete", () => {
            this.stop();
        });
        this.eventEmitter.on("DutyLockoutUpdate", data => {
            console.log("Duty Lockout update.  Time left: ", data.Time);
        });
        this.eventEmitter.once("DutyWipe", () => {
            this.wipe();
        });
        this.eventEmitter.on("RawLogLine", effect => {
            console.log(effect);
        });

        this.eventEmitter.on("EffectRemoved", effect => {
            if (effect.BuffID === 158) {
                this.wipe();
            }
        });
    }
}

module.exports = SeiryuEX;
module.exports.SeiryuEX = SeiryuEX;
