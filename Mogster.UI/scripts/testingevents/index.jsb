emitter.on("DutyComplete", () => {
    console.log("Duty Complete");
});

emitter.on("DutyLockoutUpdate", data => {
    console.log("Duty Lockout update.  Time left: ", data.Time);
});

emitter.on("DutyStart", data => {
    console.log("Duty Start! Duty has time left: ", data.Time);
});

emitter.on("DutyWipe", () => {
    console.log("Everyone wiped.  Wa wa wa");
});

emitter.on("EffectAdded", effect => {
    console.log("Effect Added:", effect);
});

emitter.on("EffectRemoved", effect => {
    console.log("Effect removed:", effect);
});

emitter.on("RawLogLine", line => {
    console.log(line);
});

emitter.on("ZoneChanged", zone => {
    console.log(zone);
});