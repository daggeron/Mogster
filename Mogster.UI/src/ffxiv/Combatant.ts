export class Combatant {

    readonly Name: string;
    readonly ID: number;
    readonly OwnerID: number;
    CurrentHP: number;
    TargetID: number;
    MaxHP: number;

    constructor(combatant: Combatant) {

        this.OwnerID = combatant.OwnerID;
        this.ID = combatant.ID;
        this.Name = combatant.Name;

        this.MaxHP = combatant.MaxHP;
        this.CurrentHP = combatant.CurrentHP;
        this.TargetID = combatant.TargetID;
    }

    update(combatant: Combatant) {
        if (this.MaxHP !== combatant.MaxHP) 
            this.MaxHP = combatant.MaxHP;

        if (this.CurrentHP !== combatant.CurrentHP)
            this.CurrentHP = combatant.CurrentHP;

        if (this.TargetID !== combatant.TargetID)
            this.TargetID = combatant.TargetID;

    }

}