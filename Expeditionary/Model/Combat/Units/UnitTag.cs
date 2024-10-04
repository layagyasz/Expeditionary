﻿using System.Text.Json.Serialization;

namespace Expeditionary.Model.Combat.Units
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum UnitTag
    {
        None,

        Airborne,
        Ammo,
        Amphibious,
        AntiAir,
        AntiTank,
        Arctic,
        Armor,
        Artillery,
        Assault,
        Cavalry,
        CBRN,
        Empyrean,
        Engineer,
        Fixed,
        Flame,
        Food,
        Fuel,
        Heavy,
        HQ,
        Infantry,
        Jamming,
        Labor,
        Light,
        Maintenance,
        Marine,
        Medical,
        Medium,
        MG,
        Mine,
        MineLaunching,
        MineLaying,
        Missile,
        MissileDefense,
        Mortar,
        Mountain,
        MountedAutomatic,
        MineClearing,
        MountedManual,
        Ordnance,
        Rail,
        RangeLong,
        RangeMedium,
        RangeOrbital,
        RangeShort,
        Recovery,
        Rocket,
        Security,
        Sensor,
        Ski,
        Signal,
        Sniper,
        Stellar,
        Supply,
        Towed,
        Tracked,
        Transport,
        UAV,
        Wheeled,
    }
}