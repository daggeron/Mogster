const { decorate, observable } = require('mobx');

export class Combatant {

    readonly Name: string;
    readonly ID: number;
    readonly OwnerID: number;
    CurrentHP: number;
    TargetID: number;
    MaxHP: number;
    IsAggressive: boolean;
    InCombat: boolean;
    IsAgroed: boolean;

    constructor(combatant: Combatant) {

        this.OwnerID = combatant.OwnerID;
        this.ID = combatant.ID;
        this.Name = combatant.Name;

        this.MaxHP = combatant.MaxHP;
        this.CurrentHP = combatant.CurrentHP;
        this.TargetID = combatant.TargetID;
        this.IsAggressive = combatant.IsAggressive;
        this.InCombat = combatant.InCombat;
        this.IsAgroed = combatant.IsAgroed;
    }

    update(combatant: Combatant) {
        if (this.MaxHP !== combatant.MaxHP) 
            this.MaxHP = combatant.MaxHP;

        if (this.CurrentHP !== combatant.CurrentHP)
            this.CurrentHP = combatant.CurrentHP;

        if (this.TargetID !== combatant.TargetID)
            this.TargetID = combatant.TargetID;

        this.InCombat = combatant.InCombat;
        this.IsAgroed = combatant.IsAgroed;
    }

}

decorate(Combatant, {
    CurrentHP: observable,
    MaxHP: observable,
    TargetID: observable,
    InCombat: observable,
    IsAgroed: observable
});