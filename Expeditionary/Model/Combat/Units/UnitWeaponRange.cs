﻿namespace Expeditionary.Model.Combat.Units
{
    public record struct UnitWeaponRange(Modifier Physical, Modifier Targeting)
    {
        public float Get()
        {
            return Math.Min(Physical.GetValue(), Targeting.GetValue());
        }
    }
}
