using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;

namespace AIEngineTest
{
    [System.Serializable, ExposedToHiraBots("06033E01-71F2-4344-92CB-62889A539B36")]
    public enum CharacterClass : byte
    {
        Fighter,
        Magus,
        Rogue,
        Wizard
    }

    [System.Serializable, ExposedToHiraBots("F091D744-3397-4027-ACB2-B36C89914A69")]
    public enum DamageType : byte
    {
        None,
        Slashing,
        Piercing,
        Bludgeoning
    }

    [System.Serializable, ExposedToHiraBots("B291899F-FBF9-4C63-82D7-9FA17BE606F2")]
    public enum StatusType : byte
    {
        Blocking,
        Count,
    }

    public unsafe struct StatusEffects
    {
        private const int k_FlagsLength = (((int) StatusType.Count + 7) & ~7) / 8;

        public fixed byte m_Flags[k_FlagsLength];

        public void ResetAll()
        {
            for (var i = 0; i < k_FlagsLength; i++)
            {
                m_Flags[i] = 0;
            }
        }
    }

    public class CharacterAttributes : MonoBehaviour
    {
        private CharacterClass m_Class;
        private int m_Level;

        private int m_Strength;
        private int m_Dexterity;
        private int m_Constitution;
        private int m_Intelligence;
        private int m_Wisdom;
        private int m_Charisma;

        private float m_HitPointFactor;
        private float m_SpellPointFactor;

        #region Init

        private enum AbilityScoreRangeType
        {
            VeryLow,
            Low,
            Medium,
            High,
            VeryHigh,
            Max
        }

        public void Initialize(CharacterClass cc, int lvl)
        {
            int GetRandomAbilityScore(AbilityScoreRangeType type)
            {
                var min = type switch
                {
                    AbilityScoreRangeType.VeryLow => 6,
                    AbilityScoreRangeType.Low => 8,
                    AbilityScoreRangeType.Medium => 10,
                    AbilityScoreRangeType.High => 12,
                    AbilityScoreRangeType.VeryHigh => 14,
                    AbilityScoreRangeType.Max => 16,
                    _ => throw new System.ArgumentOutOfRangeException(nameof(type), type, null)
                };

                return Random.Range(min, min + 5);
            }

            int CalculateAbilityScore(AbilityScoreRangeType type, bool addLevelUps)
            {
                return GetRandomAbilityScore(type) + (addLevelUps ? (lvl / 4) : 0);
            }

            (AbilityScoreRangeType type, bool addExtra) strType = cc switch
            {
                CharacterClass.Fighter => (AbilityScoreRangeType.Max, true),
                CharacterClass.Magus => (AbilityScoreRangeType.High, true),
                CharacterClass.Rogue => (AbilityScoreRangeType.High, false),
                CharacterClass.Wizard => (AbilityScoreRangeType.VeryLow, false),
                _ => throw new System.ArgumentOutOfRangeException(nameof(cc), cc, null)
            };

            (AbilityScoreRangeType type, bool addExtra) dexType = cc switch
            {
                CharacterClass.Fighter => (AbilityScoreRangeType.High, false),
                CharacterClass.Magus => (AbilityScoreRangeType.Medium, false),
                CharacterClass.Rogue => (AbilityScoreRangeType.Max, true),
                CharacterClass.Wizard => (AbilityScoreRangeType.Medium, false),
                _ => throw new System.ArgumentOutOfRangeException(nameof(cc), cc, null)
            };

            (AbilityScoreRangeType type, bool addExtra) conType = cc switch
            {
                CharacterClass.Fighter => (AbilityScoreRangeType.VeryHigh, false),
                CharacterClass.Magus => (AbilityScoreRangeType.Low, false),
                CharacterClass.Rogue => (AbilityScoreRangeType.Medium, false),
                CharacterClass.Wizard => (AbilityScoreRangeType.Low, false),
                _ => throw new System.ArgumentOutOfRangeException(nameof(cc), cc, null)
            };

            (AbilityScoreRangeType type, bool addExtra) intType = cc switch
            {
                CharacterClass.Fighter => (AbilityScoreRangeType.VeryLow, false),
                CharacterClass.Magus => (AbilityScoreRangeType.High, false),
                CharacterClass.Rogue => (AbilityScoreRangeType.Medium, false),
                CharacterClass.Wizard => (AbilityScoreRangeType.Max, true),
                _ => throw new System.ArgumentOutOfRangeException(nameof(cc), cc, null)
            };

            (AbilityScoreRangeType type, bool addExtra) wisType = cc switch
            {
                CharacterClass.Fighter => (AbilityScoreRangeType.Medium, false),
                CharacterClass.Magus => (AbilityScoreRangeType.VeryLow, false),
                CharacterClass.Rogue => (AbilityScoreRangeType.Low, false),
                CharacterClass.Wizard => (AbilityScoreRangeType.Medium, false),
                _ => throw new System.ArgumentOutOfRangeException(nameof(cc), cc, null)
            };

            (AbilityScoreRangeType type, bool addExtra) chaType = cc switch
            {
                CharacterClass.Fighter => (AbilityScoreRangeType.VeryLow, false),
                CharacterClass.Magus => (AbilityScoreRangeType.High, false),
                CharacterClass.Rogue => (AbilityScoreRangeType.VeryLow, false),
                CharacterClass.Wizard => (AbilityScoreRangeType.Low, false),
                _ => throw new System.ArgumentOutOfRangeException(nameof(cc), cc, null)
            };

            m_Class = cc;
            m_Level = lvl;
            m_Effects.ResetAll();
            m_Strength = CalculateAbilityScore(strType.type, strType.addExtra);
            m_Dexterity = CalculateAbilityScore(dexType.type, dexType.addExtra);
            m_Constitution = CalculateAbilityScore(conType.type, conType.addExtra);
            m_Intelligence = CalculateAbilityScore(intType.type, intType.addExtra);
            m_Wisdom = CalculateAbilityScore(wisType.type, wisType.addExtra);
            m_Charisma = CalculateAbilityScore(chaType.type, chaType.addExtra);

            m_HitPointFactor = 1f;
            m_SpellPointFactor = 1f;
        }

        #endregion

        #region Status

#pragma warning disable 414
        private StatusEffects m_Effects;
#pragma warning restore 414

        public unsafe bool HasStatus(StatusType status)
        {
            fixed (byte* effects = m_Effects.m_Flags)
            {
                return GetStatus(effects, status);
            }
        }

        public unsafe void SetStatus(StatusType status, bool value)
        {
            fixed (byte* effects = m_Effects.m_Flags)
            {
                SetStatus(effects, status, value);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe bool GetStatus(byte* effects, StatusType type)
        {
            var index = 1 << (int) type;
            var byteIndex = index / 8;
            var bitIndex = index % 8;
            return (effects[byteIndex] & (1 << bitIndex)) != 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void SetStatus(byte* effects, StatusType type, bool value)
        {
            var index = 1 << (int) type;
            var byteIndex = index / 8;
            var bitIndex = index % 8;
            effects[byteIndex] = value
                ? (byte) (effects[byteIndex] | (1 << bitIndex))
                : (byte) (effects[byteIndex] & ~(1 << bitIndex));
        }

        #endregion

        #region HitPoints

        public (int current, int max) hitPoints
        {
            get => CalculateHitPoints(m_Class, m_Level, m_Constitution, m_HitPointFactor);
            set => m_HitPointFactor = (float) value.current / value.max;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetHitDie(CharacterClass cc) => cc switch
        {
            CharacterClass.Fighter => 10,
            CharacterClass.Magus => 8,
            CharacterClass.Rogue => 8,
            CharacterClass.Wizard => 6,
            _ => throw new System.ArgumentOutOfRangeException(nameof(cc), cc, null)
        };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetMaxHitPoints(CharacterClass cc, int lvl, int con)
        {
            return (GetHitDie(cc) + con) * lvl;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static (int current, int max) CalculateHitPoints(CharacterClass cc, int lvl, int con, float hitPointFactor)
        {
            var max = GetMaxHitPoints(cc, lvl, con);
            var current = Mathf.RoundToInt(max * hitPointFactor);
            var clampedCurrent = Mathf.Clamp(current, 0, max);
            return (clampedCurrent, max);
        }

        #endregion

        #region Attack

        public int maxAttackCount => GetMaxAttackCount(m_Class, m_Level);
        public int attackModifier => GetAttackModifier(m_Class, m_Level, m_Strength, m_Dexterity);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetBaseAttackBonus(CharacterClass cc, int lvl) => cc switch
        {
            CharacterClass.Fighter => lvl,
            CharacterClass.Magus => lvl * 3 / 4,
            CharacterClass.Rogue => lvl * 3 / 4,
            CharacterClass.Wizard => lvl * 2 / 4,
            _ => throw new System.ArgumentOutOfRangeException(nameof(cc), cc, null)
        };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetMaxAttackCount(CharacterClass cc, int lvl)
        {
            return 1 + GetBaseAttackBonus(cc, lvl) / 5;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetAttackModifier(CharacterClass cc, int lvl, int str, int dex) => cc switch
        {
            CharacterClass.Fighter => GetBaseAttackBonus(cc, lvl) + ((str - 10) / 2),
            CharacterClass.Magus => GetBaseAttackBonus(cc, lvl) + ((str - 10) / 2),
            CharacterClass.Rogue => GetBaseAttackBonus(cc, lvl) + ((dex - 10) / 3),
            CharacterClass.Wizard => GetBaseAttackBonus(cc, lvl) + ((str - 10) / 2),
            _ => throw new System.ArgumentOutOfRangeException(nameof(cc), cc, null)
        };

        #endregion

        #region Damage

        public (int min, int max, DamageType type) equippedWeaponDamageRange => GetEquippedWeaponDamageRange(m_Class, m_Level, m_Strength, m_Dexterity);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static (int min, int max, DamageType type) GetEquippedWeaponDamageRange(CharacterClass cc, int lvl, int str, int dex)
        {
            switch (cc)
            {
                case CharacterClass.Fighter:
                {
                    var strMod = ((str - 10) / 2);
                    return (1 + strMod, 8 + strMod, DamageType.Slashing); // 1d8 for longsword
                }
                case CharacterClass.Magus:
                {
                    var strMod = ((str - 10) / 2);
                    return (1 + strMod, 6 + strMod, DamageType.Slashing); // 1d6 for shortsword
                }
                case CharacterClass.Rogue:
                {
                    var dexMod = ((dex - 10) / 2);
                    var sneakDie = lvl / 2;
                    return (1 + (sneakDie * 1) + dexMod, 4 + (sneakDie * 6) + dexMod, DamageType.Piercing); // 1d4 for dagger, 1d6 for sneak dice
                }
                case CharacterClass.Wizard:
                {
                    var strMod = ((str - 10) / 2);
                    return (1 + strMod, 6 + strMod, DamageType.Bludgeoning); // 1d6 for quarterstaff
                }
                default:
                    throw new System.ArgumentOutOfRangeException(nameof(cc), cc, null);
            }
        }

        #endregion

        #region Defence

        public int armourClass => GetArmourClass(m_Class, m_Dexterity);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetArmourBonus(CharacterClass cc) => cc switch
        {
            CharacterClass.Fighter => 14, // +4 shield, plate armour
            CharacterClass.Magus => 4, // mage armour
            CharacterClass.Rogue => 8, // leather armour
            CharacterClass.Wizard => 4, // mage armour
            _ => throw new System.ArgumentOutOfRangeException(nameof(cc), cc, null)
        };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetDodgeBonus(CharacterClass cc, int dex) => cc switch
        {
            CharacterClass.Fighter => Mathf.Min((dex - 10) / 2, 2), // capped by armour
            CharacterClass.Magus => (dex - 10) / 2,
            CharacterClass.Rogue => (dex - 10) / 2,
            CharacterClass.Wizard => (dex - 10) / 2,
            _ => throw new System.ArgumentOutOfRangeException(nameof(cc), cc, null)
        };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetArmourClass(CharacterClass cc, int dex)
        {
            return 10 + GetArmourBonus(cc) + GetDodgeBonus(cc, dex);
        }

        #endregion

        #region Saves

        public int fortitudeSave => GetFortitudeSaveForClass(m_Class, m_Level, m_Constitution);
        public int reflexSave => GetReflexSaveForClass(m_Class, m_Level, m_Dexterity);
        public int willSave => GetWillSaveForClass(m_Class, m_Level, m_Wisdom);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetLowSave(int lvl)
        {
            return lvl / 3;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetHighSave(int lvl)
        {
            return 2 + lvl / 2;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetFortitudeSaveForClass(CharacterClass cc, int lvl, int con) => cc switch
        {
            CharacterClass.Fighter => GetHighSave(lvl),
            CharacterClass.Magus => GetHighSave(lvl),
            CharacterClass.Rogue => GetLowSave(lvl),
            CharacterClass.Wizard => GetLowSave(lvl),
            _ => throw new System.ArgumentOutOfRangeException(nameof(cc), cc, null)
        } + ((con - 10) / 2);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetReflexSaveForClass(CharacterClass cc, int lvl, int dex) => cc switch
        {
            CharacterClass.Fighter => GetLowSave(lvl),
            CharacterClass.Magus => GetLowSave(lvl),
            CharacterClass.Rogue => GetHighSave(lvl),
            CharacterClass.Wizard => GetLowSave(lvl),
            _ => throw new System.ArgumentOutOfRangeException(nameof(cc), cc, null)
        } + ((dex - 10) / 2);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetWillSaveForClass(CharacterClass cc, int lvl, int wis) => cc switch
        {
            CharacterClass.Fighter => GetLowSave(lvl),
            CharacterClass.Magus => GetHighSave(lvl),
            CharacterClass.Rogue => GetLowSave(lvl),
            CharacterClass.Wizard => GetHighSave(lvl),
            _ => throw new System.ArgumentOutOfRangeException(nameof(cc), cc, null)
        } + ((wis - 10) / 2);

        #endregion

        #region SpellPoints

        public (int current, int max) spellPoints
        {
            get => CalculateSpellPoints(m_Class, m_Level, m_Intelligence, m_Charisma, m_SpellPointFactor);
            set => m_SpellPointFactor = (float) value.current / value.max;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetWizardBaseSpellPoints(int lvl) => lvl switch
        {
            01 => 1 * 1,
            02 => 2 * 1,
            03 => 2 * 1 + 1 * 2,
            04 => 3 * 1 + 2 * 2,
            05 => 3 * 1 + 2 * 2 + 1 * 3,
            06 => 3 * 1 + 3 * 2 + 2 * 3,
            07 => 4 * 1 + 3 * 2 + 2 * 3 + 1 * 4,
            08 => 4 * 1 + 3 * 2 + 3 * 3 + 2 * 4,
            09 => 4 * 1 + 4 * 2 + 3 * 3 + 2 * 4 + 1 * 5,
            10 => 4 * 1 + 4 * 2 + 3 * 3 + 3 * 4 + 2 * 5,
            11 => 4 * 1 + 4 * 2 + 4 * 3 + 3 * 4 + 2 * 5 + 1 * 6,
            12 => 4 * 1 + 4 * 2 + 4 * 3 + 3 * 4 + 3 * 5 + 2 * 6,
            13 => 4 * 1 + 4 * 2 + 4 * 3 + 4 * 4 + 3 * 5 + 2 * 6 + 1 * 7,
            14 => 4 * 1 + 4 * 2 + 4 * 3 + 4 * 4 + 3 * 5 + 3 * 6 + 2 * 7,
            15 => 4 * 1 + 4 * 2 + 4 * 3 + 4 * 4 + 4 * 5 + 3 * 6 + 2 * 7 + 1 * 8,
            16 => 4 * 1 + 4 * 2 + 4 * 3 + 4 * 4 + 4 * 5 + 3 * 6 + 3 * 7 + 2 * 8,
            17 => 4 * 1 + 4 * 2 + 4 * 3 + 4 * 4 + 4 * 5 + 4 * 6 + 3 * 7 + 2 * 8 + 1 * 9,
            18 => 4 * 1 + 4 * 2 + 4 * 3 + 4 * 4 + 4 * 5 + 4 * 6 + 3 * 7 + 3 * 8 + 2 * 9,
            19 => 4 * 1 + 4 * 2 + 4 * 3 + 4 * 4 + 4 * 5 + 4 * 6 + 4 * 7 + 3 * 8 + 3 * 9,
            20 => 4 * 1 + 4 * 2 + 4 * 3 + 4 * 4 + 4 * 5 + 4 * 6 + 4 * 7 + 4 * 8 + 4 * 9,
            _ => throw new System.ArgumentOutOfRangeException(nameof(lvl), lvl, null)
        };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetMagusBaseSpellPoints(int lvl) => lvl switch
        {
            01 => 1 * 1,
            02 => 2 * 1,
            03 => 3 * 1,
            04 => 3 * 1 + 1 * 2,
            05 => 4 * 1 + 2 * 2,
            06 => 4 * 1 + 3 * 2,
            07 => 4 * 1 + 3 * 2 + 1 * 3,
            08 => 4 * 1 + 4 * 2 + 2 * 3,
            09 => 5 * 1 + 4 * 2 + 3 * 3,
            10 => 5 * 1 + 4 * 2 + 3 * 3 + 1 * 4,
            11 => 5 * 1 + 4 * 2 + 4 * 3 + 2 * 4,
            12 => 5 * 1 + 5 * 2 + 4 * 3 + 3 * 4,
            13 => 5 * 1 + 5 * 2 + 4 * 3 + 3 * 4 + 1 * 5,
            14 => 5 * 1 + 5 * 2 + 4 * 3 + 4 * 4 + 2 * 5,
            15 => 5 * 1 + 5 * 2 + 5 * 3 + 4 * 4 + 3 * 5,
            16 => 5 * 1 + 5 * 2 + 5 * 3 + 4 * 4 + 3 * 5 + 1 * 6,
            17 => 5 * 1 + 5 * 2 + 5 * 3 + 4 * 4 + 4 * 5 + 2 * 6,
            18 => 5 * 1 + 5 * 2 + 5 * 3 + 5 * 4 + 4 * 5 + 3 * 6,
            19 => 5 * 1 + 5 * 2 + 5 * 3 + 5 * 4 + 5 * 5 + 4 * 6,
            20 => 5 * 1 + 5 * 2 + 5 * 3 + 5 * 4 + 5 * 5 + 5 * 6,
            _ => throw new System.ArgumentOutOfRangeException(nameof(lvl), lvl, null)
        };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetBonusSpellPoints(int maxSpellLevel, int spellcastingAbilityScore)
        {
            var spellPoints = 0;
            var mod = (spellcastingAbilityScore - 10) / 2;
            for (var i = 1; i <= maxSpellLevel; i++)
            {
                spellPoints += (Mathf.Max(mod - i, 0) + 2) / 4;
            }

            return spellPoints;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetMaxSpellPoints(CharacterClass cc, int lvl, int intel, int cha) => cc switch
        {
            CharacterClass.Fighter => 0,
            CharacterClass.Magus => GetMagusBaseSpellPoints(lvl) + GetBonusSpellPoints(6, cha),
            CharacterClass.Rogue => 0,
            CharacterClass.Wizard => GetWizardBaseSpellPoints(lvl) + GetBonusSpellPoints(9, intel),
            _ => throw new System.ArgumentOutOfRangeException(nameof(cc), cc, null)
        };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static (int current, int max) CalculateSpellPoints(CharacterClass cc, int lvl, int intel, int cha, float spellPointFactor)
        {
            var max = GetMaxSpellPoints(cc, lvl, intel, cha);
            var current = Mathf.RoundToInt(max * spellPointFactor);
            var clampedCurrent = Mathf.Clamp(current, 0, max);
            return (clampedCurrent, max);
        }

        #endregion
    }
}