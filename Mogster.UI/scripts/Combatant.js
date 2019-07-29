"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
class Combatant {
    constructor(combatant) {
        this.OwnerID = combatant.OwnerID;
        this.ID = combatant.ID;
        this.Name = combatant.Name;
        this.MaxHP = combatant.MaxHP;
        this.CurrentHP = combatant.CurrentHP;
        this.TargetID = combatant.TargetID;
    }
    update(combatant) {
        if (this.MaxHP !== combatant.MaxHP)
            this.MaxHP = combatant.MaxHP;
        if (this.CurrentHP !== combatant.CurrentHP)
            this.CurrentHP = combatant.CurrentHP;
        if (this.TargetID !== combatant.TargetID)
            this.TargetID = combatant.TargetID;
    }
}
exports.Combatant = Combatant;
//# sourceMappingURL=Combatant.js.map