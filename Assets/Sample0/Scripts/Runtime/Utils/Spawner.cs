using UnityEngine;

namespace AIEngineTest
{
    public class Spawner : MonoBehaviour
    {
        private int m_MinLevel = 10, m_MaxLevel = 14;
        private static readonly CharacterClass[] s_Classes = (CharacterClass[]) typeof(CharacterClass).GetEnumValues();

        private static void Spawn(CharacterClass cc, int lvl)
        {
            var character = GameManager.characterGenerator.Generate(Vector3.zero);

            character.m_CharacterAttributes.Initialize(cc, lvl);

            var equipmentType = cc switch
            {
                CharacterClass.Magus => EquipmentType.Sword,
                CharacterClass.Fighter => EquipmentType.SwordAndShield,
                CharacterClass.Rogue => EquipmentType.DualDaggers,
                CharacterClass.Wizard => EquipmentType.Staff,
                _ => throw new System.ArgumentOutOfRangeException()
            };

            character.m_AnimatorHelper.InitializeEquipment(equipmentType);

            var brain = character.m_Brain;

            brain.blackboardComponent.SetEnumValue("Class", cc);
            brain.blackboardComponent.SetEnumValue("OwnedEquipment", equipmentType);
        }

        private void OnGUI()
        {
            GUILayout.BeginHorizontal();

            GUILayout.Space(30);

            GUILayout.BeginVertical();

            GUILayout.Space(100);

            GUILayout.Label($"MinLevel: {m_MinLevel} ");
            m_MinLevel = Mathf.Clamp((int) GUILayout.HorizontalSlider(m_MinLevel, 1, 20), 1, 20);

            GUILayout.Space(10);

            GUILayout.Label($"MaxLevel: {m_MaxLevel} ");
            m_MaxLevel = Mathf.Clamp((int) GUILayout.HorizontalSlider(m_MaxLevel, 1, 20), m_MinLevel, 20);

            GUILayout.Space(10);

            foreach (var cc in s_Classes)
            {
                if (GUILayout.Button(cc.ToString()))
                {
                    Spawn(cc, Random.Range(m_MinLevel, m_MaxLevel + 1));
                }
            }

            GUILayout.EndVertical();

            GUILayout.EndHorizontal();
        }
    }
}