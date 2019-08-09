const assert = require('assert').strict;

var test = require('./test');
var currentEncounter = test;

currentEncounter.triggers.forEach(trigger => {
    trigger.active = true;
    validateTrigger(trigger);
});

console.log(test.details);

var event = "NetworkBuff";

var payload = {
    BuffID: 158
};

currentEncounter.triggers.forEach(triggerRunner);

console.log("******");

console.log("");

currentEncounter.triggers.forEach(triggerRunner);

function validateTrigger(trigger) {
    assert.ok(trigger.hasOwnProperty('id'), "No ID has been specified");
    assert.ok(trigger.hasOwnProperty('run') || trigger.hasOwnProperty('playsound'), "Either playsound or run needs to be defined");
}

function triggerRunner(trigger) {
    if (eventRunner(trigger) &&
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

function eventRunner(trigger) {
    return trigger.event.name === event && trigger.event.conditional(payload, {});
}

function conditionalRunner(trigger) {
    return trigger.hasOwnProperty('conditional') ? trigger.conditional({}) : true;
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