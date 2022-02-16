using System;
using System.Runtime.CompilerServices;

namespace AIEngineTest
{
    public static class AnimatorConstants
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetMaxSpeed(WeaponType weaponType) => weaponType switch
        {
            WeaponType.None => 4.197f,
            WeaponType.Fists => 4.197f,
            WeaponType.Sword => 4.251f,
            WeaponType.SwordAndShield => 4.236f,
            WeaponType.DualDaggers => 4.251f,
            WeaponType.Staff => 5.084f,
            _ => throw new ArgumentOutOfRangeException(nameof(weaponType), weaponType, null)
        };
    }
}