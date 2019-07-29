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

class HourglassInn {
    constructor() {
        this.eventEmitter = new EventEmitter2();
    }
    static get zone() {
        return 178;
    }

    static get encounterID() {
        return "The Hourglass inn test";
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
        }

        this.emitter.on("EffectAdded", watcherCallback);
        this.wireNeededEvents();
    }

    /*
     * Start of the encounter.
     */
    start() {
        //Removes the listener we used to monitor chat for the start event.
        //Saves resources

        console.log(`Started encounter ${HourglassInn.encounterID}`);
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

module.exports = HourglassInn;
module.exports.HourglassInn = HourglassInn;
