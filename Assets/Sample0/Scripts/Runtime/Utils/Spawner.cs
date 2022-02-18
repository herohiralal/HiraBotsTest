using UnityEngine;

namespace AIEngineTest
{
    public class Spawner : MonoBehaviour
    {
        [SerializeField] private BaseArchetype m_Character;

        private void OnGUI()
        {
            if (m_Character == null)
            {
                if (GUILayout.Button("Spawn"))
                {
                    m_Character = GameManager.characterGenerator.Generate(Vector3.zero);
                    m_Character.m_AnimatorHelper.InitializeEquipment(EquipmentType.None);
                }
            }
            else
            {
                if (m_Character.m_AnimatorHelper.equipmentType != EquipmentType.None)
                {
                    if (GUILayout.Button("Sheathe"))
                    {
                        m_Character.m_AnimatorHelper.PrepareToUnequip(m_Character.m_AnimatorHelper.equipmentType);
                        m_Character.m_AnimatorHelper.currentMontageState = MontageType.Sheathe;
                    }
                }
                else
                {
                    EquipmentType? type = null;
                    if (GUILayout.Button("Sword"))
                    {
                        type = EquipmentType.Sword;
                    }

                    if (GUILayout.Button("SwordAndShield"))
                    {
                        type = EquipmentType.SwordAndShield;
                    }

                    if (GUILayout.Button("DualDaggers"))
                    {
                        type = EquipmentType.DualDaggers;
                    }

                    if (GUILayout.Button("Staff"))
                    {
                        type = EquipmentType.Staff;
                    }

                    if (type.HasValue)
                    {
                        m_Character.m_AnimatorHelper.InitializeEquipment(type.Value);
                        m_Character.m_AnimatorHelper.PrepareToEquip(type.Value);
                        m_Character.m_AnimatorHelper.currentMontageState = MontageType.Unsheathe;
                    }
                }
            }
        }
    }
}