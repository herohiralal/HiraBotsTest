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

    public class CharacterAttributes : MonoBehaviour
    {
        public const int k_CharacterLevel = 5;

        public CharacterClass m_Class;

        [Range(7, 20)] public int m_Strength;
        [Range(7, 20)] public int m_Dexterity;
        [Range(7, 20)] public int m_Constitution;
        [Range(7, 20)] public int m_Intelligence;
        [Range(7, 20)] public int m_Wisdom;
        [Range(7, 20)] public int m_Charisma;

        [Range(0, 1)] public float m_HitPointFactor;
        [Range(0, 1)] public float m_SpellPointFactor;

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

        public void Initialize(CharacterClass cc)
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
                return GetRandomAbilityScore(type) + (addLevelUps ? (k_CharacterLevel / 4) : 0);
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

        #region HitPoints

        public (int current, int max) hitPoints
        {
            get => CalculateHitPoints(m_Class, m_Constitution, m_HitPointFactor);
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
        public static int GetMaxHitPoints(CharacterClass cc, int con)
        {
            return (GetHitDie(cc) + con) * k_CharacterLevel;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static (int current, int max) CalculateHitPoints(CharacterClass cc, int con, float hitPointFactor)
        {
            var max = GetMaxHitPoints(cc, con);
            var current = Mathf.RoundToInt(max * hitPointFactor);
            var clampedCurrent = Mathf.Clamp(current, 0, max);
            return (clampedCurrent, max);
        }

        #endregion

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetBaseAttackBonus(CharacterClass cc) => cc switch
        {
            CharacterClass.Fighter => k_CharacterLevel,
            CharacterClass.Magus => k_CharacterLevel * 3 / 4,
            CharacterClass.Rogue => k_CharacterLevel * 3 / 4,
            CharacterClass.Wizard => k_CharacterLevel * 2 / 4,
            _ => throw new System.ArgumentOutOfRangeException(nameof(cc), cc, null)
        };

        #region SpellPoints

        public (int current, int max) spellPoints
        {
            get => CalculateSpellPoints(m_Class, m_Intelligence, m_Charisma, m_SpellPointFactor);
            set => m_SpellPointFactor = (float) value.current / value.max;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetWizardBaseSpellPoints(int level) => level switch
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
            _ => throw new System.ArgumentOutOfRangeException(nameof(level), level, null)
        };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetMagusBaseSpellPoints(int level) => level switch
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
            _ => throw new System.ArgumentOutOfRangeException(nameof(level), level, null)
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
        public static int GetMaxSpellPoints(CharacterClass cc, int intel, int cha) => cc switch
        {
            CharacterClass.Fighter => 0,
            CharacterClass.Magus => GetMagusBaseSpellPoints(k_CharacterLevel) + GetBonusSpellPoints(6, cha),
            CharacterClass.Rogue => 0,
            CharacterClass.Wizard => GetWizardBaseSpellPoints(k_CharacterLevel) + GetBonusSpellPoints(9, intel),
            _ => throw new System.ArgumentOutOfRangeException(nameof(cc), cc, null)
        };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static (int current, int max) CalculateSpellPoints(CharacterClass cc, int intel, int cha, float spellPointFactor)
        {
            var max = GetMaxSpellPoints(cc, intel, cha);
            var current = Mathf.RoundToInt(max * spellPointFactor);
            var clampedCurrent = Mathf.Clamp(current, 0, max);
            return (clampedCurrent, max);
        }

        #endregion
    }
}