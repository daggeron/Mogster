"use strict";
/* Change log:
 * Fixed needing to redecorate Combatant
 *
 */
/*
 * Notes: CombatantManager methods return Promises: https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/Promise
 * Note:  To simply return a value from a promise you can do
 *
 *
 * var return = await someFunction();
 * Note: The function has to be async in order to do that.  Also you need to wrap it in a try/catch
 *
 */
/*
 * Events:
 *  PlaySound: Sound file relative to the audio folder location %mogsterlocation%/audio
 *  StopSound: Stops all sounds
 *  RemotePlay: Play over discord sound file relative to the audio folder location %mogsterlocation%/audio
 *  RemoteStop: Stops all sounds on discord
 */
const { Combatant } = require('../Combatant');
const { observe, decorate, observable } = require('./libs/mobx/lib/mobx');

decorate(Combatant, {
    CurrentHP: observable,
    MaxHP: observable,
    TargetID: observable
});

/*
 * Redecorate Combatant as it seems it doesn't survive the sandbox transition
 *
 * This decorates the Combatant class to allow us to observe, in a cpu friendly fashion, changes;
 *
 * Example:
 *
 * var player = GetCurrentPlayer();
 * observe(player, 'TargetID', (change) => {
            console.log(change.type, "TargetID from", change.oldValue, "to", change.newValue);
    });
 */
/*
 * List of spell ids we would rather not be spammed with (looking at you teleport)
 *
 */
var ignoredSpells = [5];

//This is super spammy (duh?)
emitter.on("RawLogLine", async (line) => {
    //console.log(line);
});

emitter.on("ZoneChanged", ZoneChange);
emitter.on("DutyComplete", () => {
    console.log("YAY!");
});

//emitter.on("StartCasting", OnStartCasting);
/*emitter.on("CombatantAdded", (combatant) => {
    let cb = new Combatant(combatant);
    CombatantAdded(cb);
});
emitter.on("CombatantRemoved", (combatant) => {
    CombatantRemoved(combatant);
});*/
function CombatantAdded(combatant) {
}
function CombatantRemoved(combatant) {
    if (monitoredEntities.has(combatant.ID))
        clearMonitor(combatant.ID, monitoredEntities.get(combatant.ID));
}
/*
 *      targetID;
        targetName;

        actorID;
        actorName;

        skillId;
        skillName;

        timeOfCast;
 *
 */
function OnStartCasting(ability) {
    if (ignoredSpells.includes(ability.skillId))
        return;
    /*if (ability.actorName !== "Titania") {
        return;
    }
    if (ability.skillId === "3D4C") {
        setTimeout(function () { Send("PlaySound", 'getout.ogg'); }, 12000);
        //do something
    }*/
}
/*
 *         zoneID;
           zoneName;
 */
function ZoneChange(zone) {
    console.log(zone);
    Send("PlaySound", 'belltollnightelf.ogg');
}
function Send(event, payload) {
    emitter.emit(event, payload);
}
/*
 *  Should only be called after you've successfully found the entity.
 *  Will auto unmonitor if the entity can't be found.
 */
var player = null;
var monitoredEntities = new Map();
function monitorCombatant(id, entity) {
    if (entity === null)
        throw new Error("Entity was null.  Can only monitor found entities.");
    let pts = setInterval(async () => {
        try {
            let result = await CombatantManager.getCombatant(id);
            entity.update(combatant);
        }
        catch (exception) {
            console.log("Error, clearing interval");
            clearMonitor(id, pts);
            return;
        }
    }, 50);
    monitoredEntities.set(id, pts);
}
function clearMonitor(id, pts) {
    try {
        clearInterval(pts);
        monitoredEntities.delete(id);
    }
    catch (exception) { }
}
function monitorCurrentPlayer() {
    console.log("Entered monitor");
    if (player === null) {
        return;
    }
    console.log("Starting interval scan");
    let pts = setInterval(async () => {
        try {
            let result = await CombatantManager.getCurrentPlayer();
            player.update(result);
        }
        catch (exception) {
            console.log(exception);
            console.log("Error, clearing interval");
            if (player != null) {
                clearMonitor(player.ID, pts);
            }
            return;
        }
    }, 1);
    console.log("Adding to monitor map");
    monitoredEntities.set(player.ID, pts);
    console.log("done adding to monitor map");
}
function wireup(player) {
    observe(player, 'MaxHP', async (change) => {
        console.log(change.type, "MaxHP from", change.oldValue, "to", change.newValue);
    });
    observe(player, 'CurrentHP', async (change) => {
        console.log(change.type, "CurrentHP from", change.oldValue, "to", change.newValue);
    });
    observe(player, 'TargetID', async (change) => {
        if (change.newValue === 0)
            return;
        try {
            let result = await CombatantManager.getCombatant(change.newValue);
            let combatant = new Combatant(result);
            console.log(combatant);
        }
        catch (exception) {
            console.log("Could not find combatant.  NPC?");
        }
        console.log(change.type, "TargetID from", change.oldValue, "to", change.newValue);
    });
}
/*
 * Scan for initial player then wire him up.
 */
var scanForCurrentPlayer = setInterval(async () => {
    console.log("moo");
    try {
        let result = await CombatantManager.getCurrentPlayer();
        player = new Combatant(result);
        wireup(player);
        monitorCurrentPlayer();
        //Very important that we cancel our scan once we are wired up!
        clearInterval(scanForCurrentPlayer);
    }
    catch (exception) {
        console.log(exception);
    }
}, 1000);
setInterval(function () { }, 60000);
//# sourceMappingURL=index.js.map