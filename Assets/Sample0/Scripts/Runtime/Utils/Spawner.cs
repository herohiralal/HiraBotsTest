using UnityEngine;

namespace AIEngineTest
{
    public class Spawner : MonoBehaviour
    {
        private bool m_Locked;

        private static readonly CharacterClass[] s_Classes = (CharacterClass[]) typeof(CharacterClass).GetEnumValues();

        private System.Collections.IEnumerator Spawn(CharacterClass cc)
        {
            m_Locked = true;

            var character = GameManager.characterGenerator.Generate(Vector3.zero);

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

            while (!brain.blackboardComponent.isValid)
            {
                yield return null;
            }

            brain.blackboardComponent.SetEnumValue("Class", cc);
            brain.blackboardComponent.SetEnumValue("OwnedEquipment", equipmentType);

            m_Locked = false;
        }

        private void OnGUI()
        {
            if (m_Locked)
            {
                return;
            }

            foreach (var cc in s_Classes)
            {
                if (GUILayout.Button(cc.ToString()))
                {
                    StartCoroutine(Spawn(cc));
                }
            }
        }
    }
}