

"use strict";

/*
 * This is a proof of concept template and is not complete.
 * 
 * Nothing in here is finalized.
 * 
 */


class EncounterBase {
    constructor(encounterDetails) {
		this.encounterDetails = encounterDetails;
    }

    get zone() {
        return this.encounterDetails.zone;
    }
    
	get encounter() {
        return this.encounterDetails;
    }
	
    static get encounter() {
        return this.encounterDetails;
    }
}

module.exports.EncounterBase = EncounterBase;
