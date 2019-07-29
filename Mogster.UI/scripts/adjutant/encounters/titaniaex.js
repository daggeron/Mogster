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

class TitaniaEX {
    constructor() {
        this.eventEmitter = new EventEmitter2();
    }
    static get zone() {
        return 858;
    }

    static get encounterID() {
        return "Titania Extreme: v0.0.1";
    }

    get emitter() {
        return this.eventEmitter;
    }

    /*
     * Setup an event to watch for to know when the fight has started.
     * Until we get Sharlayan back up this is what we have to do.
     */
    watchStart() {
        //Titania emits the following line over chat when we engage her
        //"Come and play! For the night is bright, and you can sleep when you're dead!"

        const watcherCallback = effect => {
            if (chat.actorName === "Titania"
                && chat.line === "Come and play! For the night is bright, and you can sleep when you're dead!") {
                this.start();
                this.emitter.removeListener("ChatLog", watcherCallback);
            }
        }

        this.emitter.on("ChatLog", watcherCallback);

        this.wireNeededEvents();
        console.log("Wired events");
    }

    /*
     * Start of the encounter.
     */
    start() {
        //Removes the listener we used to monitor chat for the start event.
        //Saves resources
        console.log(`Started encounter ${TitaniaEX.encounterID}`);
        
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
           
        });
    }
}

module.exports = TitaniaEX;
module.exports.TitaniaEX = TitaniaEX;
