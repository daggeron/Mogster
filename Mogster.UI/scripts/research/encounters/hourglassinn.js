"use strict";

/*
 * This is a proof of concept template and is not complete.
 * 
 * Nothing in here is finalized.
 * 
 */
const { EventEmitter2 } = require("./eventemitter2");

const emitter = new EventEmitter2();

const encounterDetails = {
    id: "Eulmore",
    zone: 820
};

var prop = new Map();

const triggers = [
    {
        id: 'Simplest trigger possible.  Alert on NetworkBuffx2',
        event: {
            name: 'NetworkBuff',
            conditional: (payload, helper) => {
                return payload.BuffID === 158;
            }
        },
        playsound: 'belltollnightelf.ogg'
    },
    {
        id: 'Simplest trigger possible.  Alert on NetworkBuff',
        event: {
            name: 'NetworkBuff',
            conditional: (payload, helper) => {
                return payload.BuffID === 158;
            }
        },
        playsound: {
            src: 'belltollnightelf.ogg',
            dest: 'both'
        }
    },
    {
        id: 'Engage test trigger',
        once: true,
        event: {
            name: 'NetworkBuff',
            conditional: (payload, helper) => {
                return payload.BuffID === 158;
            }
        },
        playsound: 'belltollnightelf.ogg',
        run: (payload, helper) => {
            console.log('Hello');
            prop.set("combat", true);
        }
    },
    {
        id: 'Test count trigger',
        event: {
            name: 'NetworkBuff',
            conditional: (payload, helper) => {
                return payload.BuffID === 158;
            }
        },
        run: (payload, helper) => {
            let value = prop.has("buffCount") ? prop.get("buffCount") : 0;
            prop.set("buffCount", value++);
        }
    },
    {
        id: 'Test conditional trigger and then reset',
        event: {
            name: 'NetworkBuff',
            conditional: (payload, helper) => {
                return payload.BuffID === 158;
            }
        },
        conditional: (helper) => {
            let value = prop.has("buffCount") ? prop.get("buffCount") : 0;
            return value > 5;
        },
        playsound: 'belltollnightelf.ogg',
        run: (payload, helper) => {
            prop.set("buffCount", 0);
        }
    },
    {
        id: 'Test incombat buff alert',
        once: true,
        event: {
            name: 'EffectRemoved',
            conditional: (payload, helper) => {
                return payload.BuffID === 158 && prop.get('combat') === true;
            }
        },
        playsound: 'belltollnightelf.ogg',
        run: (payload, helper) => {

        }
    },
    {
        id: 'Wipe test trigger',
        once: true,
        event: {
            name: 'EffectRemoved',
            conditional: (payload, helper) => {
                return payload.BuffID === 158;
            }
        },
        playsound: 'belltollnightelf.ogg',
        run: (payload, helper) => {
            prop.set("combat", false);
        }

    },
    {   //This trigger shows all of the options available
        id: 'Full spread',
        once: true,
        event: {
            name: 'EffectRemoved',
            conditional: (payload, helper) => {
                return payload.BuffID === 158;
            }
        },
        conditional: (helper) => {
            return prop.has("combat") && prop.get("combat");
        },
        delay: 450,
        /*delay: (payload, helper) => {
            return 450;
        },*/
        suppress: 450,
        /*suppress: (payload, helper) => {
            return 450;
        },*/
        playsound: 'belltollnightelf.ogg',
        /*playsound: {
            src: 'belltollnightelf.ogg',
            //dest: 'local'
            //dest: 'remote'
            dest: 'both'
        },*/
        run: (payload, helper) => {
            prop.set("combat", false);
        }

    }
];

function Encounter() { }
Encounter.details = encounterDetails;
Encounter.triggers = triggers;
Encounter.emitter = emitter;

module.exports = Encounter;
