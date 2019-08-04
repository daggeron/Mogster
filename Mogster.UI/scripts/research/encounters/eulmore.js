

"use strict";

/*
 * This is a proof of concept template and is not complete.
 * 
 * Nothing in here is finalized.
 * 
 */

const { EventEmitter2 } = require("eventemitter2");
const { Combatant } = require('../../libs/combatant');
const { observe, decorate, observable } = require('mobx');


function Send(emitter, event, payload) {
    emitter.emit(event, payload);
}

class EulmoreEX {
    constructor(remoteEmitter) {
        this.eventEmitter = new EventEmitter2();
        this.remoteEmitter = remoteEmitter;




    }

    get zone() {
        return 820;
    }

    static get EncounterDetails() {
        return { id: "Eulmore EX!!!!", zone: 820 };
    }

    get emitter() {
        return this.eventEmitter;
    }

    /*
     * Setup an event to watch for to know when the fight has started.
     * Until we get Sharlayan back up this is what we have to do.
     */
    watchStart() {
        console.log("Binding event emitter");
        //Titania emits the following line over chat when we engage her
        //"Come and play! For the night is bright, and you can sleep when you're dead!"
        const watcherCallback = effect => {
            if (effect.BuffID === 158) {
                this.start();
                this.emitter.removeListener("EffectAdded", watcherCallback);
            }
        };

        /*var disposeMe = observe(player, 'MaxHP', async (change) => {
            console.log(change.type, "MaxHP from", change.oldValue, "to", change.newValue);
            disposeMe.dispose();
        });*/

        this.emitter.on("EffectAdded", watcherCallback);
        this.wireNeededEvents();
    }

    /*
     * Start of the encounter.
     */
    start() {
        //Removes the listener we used to monitor chat for the start event.
        //Saves resources
        Send(this.remoteEmitter, "PlaySound", 'belltollnightelf.ogg');
        console.log(`Started encounter ${EulmoreEX.EncounterDetails.id}`);
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
        this.emitter.on("DutyComplete", () => {
            this.stop();
        });
        this.emitter.on("DutyLockoutUpdate", data => {
            console.log("Duty Lockout update.  Time left: ", data.Time);
        });
        this.emitter.once("DutyWipe", () => {
            this.wipe();
        });
        this.emitter.on("EffectAdded", effect => {

        });

        this.emitter.on("EffectRemoved", effect => {
            if (effect.BuffID === 158) {
                this.wipe();
            }
        });
    }
}

module.exports = EulmoreEX;
module.exports.EulmoreEX = EulmoreEX;
