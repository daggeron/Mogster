const { readdir, resolve } = require('sandboxfs');
const { getCurrentZone } = require('helper');

var zoneList = new Set();
var cachedMap = new Map();
var encounterMap = new Map();

function scanForEncounters() {
    readdir("./adjutant/encounters").forEach(element => {
        let encounter = require(resolve("./adjutant/encounters", element));

        if (zoneList.has(encounter.zone) ||
            cachedMap.has(encounter.zone) ||
            encounterMap.has(encounter.zone)) {
            throw new Error('Duplicate encounter discovered!!!! ZoneID:' + encounter.zone);
        }
        console.log(`Found and cached ${encounter.encounterID} attached to zoneId: ${encounter.zone}`);

        zoneList.add(encounter.zone);
        cachedMap.set(encounter.zone, encounter);
    });
}

emitter.on("ZoneChanged", zone => {
    if (zoneList.has(zone.ZoneID)) {

        let cachedEcounter = cachedMap.get(zone.ZoneID)
        let encounter = new cachedEcounter();

        emitter.onAny((event, payload) => {
            encounter.emitter.emit(event, payload);
        });

        encounter.watchStart();
    }
    console.log("Zone changed to: ", zone);
});

function forceZoneResend() {

}

try {
    scanForEncounters();

    //Keep pinging for the current zone.  We do it this way because if FFXIv is closed then open it should resume functionality. (OR we are in a hot reload)  
    
    let zoneChecker = setInterval(function () {
        let zone = getCurrentZone();
        if (zone !== 0) {
            emitter.emit("ZoneChanged", zone);
            clearInterval(zoneChecker);
        }
    }, 1000);
    
} catch (error) {
    console.log(error);
}