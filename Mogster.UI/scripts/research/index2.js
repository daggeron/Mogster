const sandboxfs = require('sandboxfs');
const helper = require('helper');

var cachedMap = new Map();
var encounterMap = new Map();
var currentEncounter = null;
var currentEncounterEmitterCallback = null;

function scanForEncounters() {

    sandboxfs.readdir("./research/encounters").forEach(element => {
        try {
            let encounter = require(sandboxfs.resolve("./research/encounters", element));

            if (cachedMap.has(encounter.details.zone) ||
                encounterMap.has(encounter.details.zone)) {

                throw new Error(`Duplicate encounter discovered!!!! ZoneID: ${encounter.details.zone}`);
            }

            console.log(`Found and cached ${encounter.details.id} attached to zoneId: ${encounter.details.zone}`);

            cachedMap.set(encounter.details.zone, encounter);

        } catch (error) {
            console.log(`Moving to next script. Failed to load ${element}.`, error);
        }
    });

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