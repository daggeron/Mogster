const sandboxfs = require('sandboxfs');
const helper = require('helper');
const assert = require('assert').strict;

var cachedMap = new Map();
var encounterMap = new Map();
var currentEncounter = null;
var currentEncounterEmitterCallback = null;


const destroyCircular = (from, seen) => {
    const to = Array.isArray(from) ? [] : {};

    seen.push(from);

    for (const [key, value] of Object.entries(from)) {
        if (typeof value === 'function') {
            continue;
        }

        if (!value || typeof value !== 'object') {
            to[key] = value;
            continue;
        }

        if (!seen.includes(from[key])) {
            to[key] = destroyCircular(from[key], seen.slice());
            continue;
        }

        to[key] = '[Circular]';
    }

    const commonProperties = [
        'name',
        'message',
        'stack',
        'code'
    ];

    for (const property of commonProperties) {
        if (typeof from[property] === 'string') {
            to[property] = from[property];
        }
    }

    return to;
};

const serializeError = value => {
    if (typeof value === 'object') {
        return destroyCircular(value, []);
    }

    // People sometimes throw things besides Error objects…
    if (typeof value === 'function') {
        // `JSON.stringify()` discards functions. We do too, unless a function is thrown directly.
        return `[Function: ${(value.name || 'anonymous')}]`;
    }

    return value;
};




function scanForEncounters() {

    sandboxfs.readdir("./research/encounters").forEach(element => {
        try {
            let encounter = require(sandboxfs.resolve("./research/encounters", element));

            if (cachedMap.has(encounter.details.zone) ||
                encounterMap.has(encounter.details.zone)) {

                throw new Error(`Duplicate encounter discovered!!!! ZoneID: ${encounter.details.zone}`);
            }

            encounter.triggers.forEach(trigger => {
                trigger.active = true;
                validateTrigger(trigger);
            });


            console.log(`Found and cached ${encounter.details.id} attached to zoneId: ${encounter.details.zone}`);

            cachedMap.set(encounter.details.zone, encounter);

        } catch (error) {
            console.log('hello');
            console.log(serializeError(error));
            console.log(`Moving to next script. Failed to load ${element}.`, error.toString());
        }
    });

}

function validateTrigger(trigger) {
    assert.ok(trigger.hasOwnProperty('id'), "No ID has been specified");
    assert.ok(trigger.hasOwnProperty('run') || trigger.hasOwnProperty('playsound'), "Either playsound or run needs to be defined");
}


emitter.onAny((event, payload) => {
    if (event === 'ZoneChanged' ||
        currentEncounter === null) {
        return;
    }

    currentEncounter.triggers.forEach(trigger => {
        triggerRunner(trigger, event, payload);
    });


});

function eventRunner(trigger, event, payload) {
    return trigger.event.name === event && trigger.event.conditional(payload, helper);
}

function conditionalRunner(trigger) {
    return trigger.hasOwnProperty('conditional') ? trigger.conditional(helper) : true;
}

function playSoundRunner(trigger) {
    let soundSrc;
    let destination = 'local';

    if (typeof trigger.playsound === 'string') {
        soundSrc = trigger.playsound;
    } else {
        soundSrc = trigger.playsound.src;
        destination = trigger.playsound.dest;
    }
    console.log(trigger.id);
    switch (destination) {
        case 'local':
            console.log(`Play local sound ${soundSrc}`);
            break;
        case 'remote':
            console.log(`Play remote sound ${soundSrc}`);
            break;
        case 'both':
            console.log(`Play local sound ${soundSrc}`);
            console.log(`Play remote sound ${soundSrc}`);
            break;
    }
}

function triggerRunner(trigger, event, payload) {
    if (eventRunner(trigger, event, payload) &&
        trigger.active === true &&
        conditionalRunner(trigger)) {
        console.log(trigger);
        if (trigger.hasOwnProperty('run') &&
            typeof trigger.run === 'function') {
            trigger.run(payload, {});
        }

        if (trigger.hasOwnProperty('playsound')) {
            console.log("playsound");
            playSoundRunner(trigger);
        }

        if (trigger.hasOwnProperty('once') &&
            trigger.once === true) {
            trigger.active = false;
        }
    }
}

emitter.on("ZoneChanged", zone => {

    console.log("Zone changed to: ", zone);

    if (currentEncounter !== null
        && currentEncounter.zone !== zone) {

        console.log(`Tearing down encounter: ${currentEncounter.details.id}`);

        currentEncounter.stop();
        currentEncounter = null;

        emitter.offAny(currentEncounterEmitterCallback);
        currentEncounterEmitterCallback = null;
    }

    if (cachedMap.has(zone.ZoneID)) {
        let cachedEcounter = cachedMap.get(zone.ZoneID);

        console.log(`Attempting to load encounter: ${cachedEcounter.details.id}`);

        currentEncounter = new cachedEcounter();

        currentEncounterEmitterCallback = (event, payload) => {
            currentEncounter.emit(event, payload);
        };

        emitter.onAny(currentEncounterEmitterCallback);

        currentEncounter.watchStart();
    }

});

try {
    scanForEncounters();
    const zoneChecker = setInterval(function () {
        try {
            let zone = helper.getCurrentZone();
            if (zone.ZoneID !== 0) {
                console.log("Received zone.  Clearing interval");
                clearInterval(zoneChecker);

                emitter.emit("ZoneChanged", zone);
            }
        } catch (exception) { /* ignored */ }
    }, 50);
} catch (error) {
    console.log(error);
}