const sandboxfs = require('sandboxfs');
const helper = require('helper');

var zoneList = new Set();
var cachedMap = new Map();
var encounterMap = new Map();
var currentEncounter = null;
var currentEncounterEmitterCallback = null;

var currentPlayer = null;


function scanForEncounters() {
    sandboxfs.readdir("./adjutant/encounters").forEach(element => {
        try {
            let encounter = require(sandboxfs.resolve("./adjutant/encounters", element));
            let encounterZone = encounter.EncounterDetails.zone;
            let encounterID = encounter.EncounterDetails.id;

            if (zoneList.has(encounterZone) ||
                cachedMap.has(encounterZone) ||
                encounterMap.has(encounterZone)) {
                throw new Error(`Duplicate encounter discovered!!!! ZoneID: ${encounterZone}`);
            }

            console.log(`Found and cached ${encounterID} attached to zoneId: ${encounterZone}`);

            zoneList.add(encounterZone);
            cachedMap.set(encounterZone, encounter);
            console.log(encounterZone);
        } catch (error) {
            console.log(`Moving to next script. Failed to load ${element}.`, error);
        }
    });

}

emitter.on("ZoneChanged", zone => {
    console.log("Zone changed to: ", zone);

    if (currentEncounter !== null
        && currentEncounter.zone !== zone) {
        console.log(`Tearing down encounter: ${cachedEcounter.EncounterDetails.id}`);

        currentEncounter.stop();
        currentEncounter = null;

        emitter.offAny(currentEncounterEmitterCallback);
        currentEncounterEmitterCallback = null;
    }

    if (zoneList.has(zone.ZoneID)) {

        let cachedEcounter = cachedMap.get(zone.ZoneID);
        console.log(`Attempting to load encounter: ${cachedEcounter.EncounterDetails.id}`);
        currentEncounter = new cachedEcounter(emitter);

        currentEncounterEmitterCallback = (event, payload, outbound) => {
            if (!outbound)
                currentEncounter.emitter.emit(event, payload);
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

let playerChecker = setInterval(() => {
    try {
        currentPlayer = CombatantManager.getCurrentPlayer();
        clearInterval(playerChecker);
        let test = helper.wrapCombatant(currentPlayer);
        console.log(test);
        console.log(CombatantManager.getCombatant(currentPlayer.ID));
    } catch (exception) {/* ignored */ }
}, 50);

function Send(event, payload) {
    emitter.emit(event, payload);
}
