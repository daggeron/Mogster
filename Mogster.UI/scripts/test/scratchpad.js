"use strict";
/* Experimental/Unfinished code ignore!! */
class Monitor {
    constructor(monitorID, target) {
        this.target = target;
        this.observeHandles = [];
        this.monitorID = monitorID;
    }
    watch(field, func) {
        let oh = observe(target, field, func);
        this.observeHandles.push(oh);
        return oh;
    }
    unWatch(oh) {
        let pos = this.observeHandles.indexOf(oh);
        this.observeHandles.splice(pos, 1);
        oh();
    }
    unWatchAll() {
        this.observeHandles.forEach(handle => {
            this.unWatch(handle);
        });
    }
}
//# sourceMappingURL=scratchpad.js.map