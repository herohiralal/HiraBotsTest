using System.Collections.Generic;
using UnityEngine;

namespace AIEngineTest
{
    [CreateAssetMenu(fileName = "WeaponGenerator", menuName = "Samples/WeaponGenerator", order = 0)]
    public class WeaponGenerator : ScriptableObject
    {
        [SerializeField] private WeaponArchetype m_WeaponArchetypePrefab = null;
        [SerializeField] private MeshCollection m_SwordMeshCollection;
        [SerializeField] private MeshCollection m_ShieldMeshCollection;
        [SerializeField] private MeshCollection m_DaggerMeshCollection;
        [SerializeField] private MeshCollection m_StaffMeshCollection;
        [SerializeField] private MaterialCollection m_MaterialCollection;

        private readonly Stack<WeaponArchetype> m_WeaponArchetypes = new Stack<WeaponArchetype>();

        public void ClearPool()
        {
            m_WeaponArchetypes.Clear();
        }

        private WeaponArchetype GetWeapon()
        {
            if (m_WeaponArchetypes.TryPop(out var pooled))
            {
                pooled.gameObject.SetActive(true);
                return pooled;
            }

            return Instantiate(m_WeaponArchetypePrefab, Vector3.zero, Quaternion.identity);
        }

        public void Discard(WeaponArchetype weapon)
        {
            weapon.m_MeshFilter.sharedMesh = null;
            weapon.m_MeshRenderer.sharedMaterial = null;
            weapon.transform.SetParent(null);
            weapon.gameObject.SetActive(false);
            m_WeaponArchetypes.Push(weapon);
        }

        private WeaponArchetype GenerateWeapon(MeshCollection meshCollection)
        {
            var output = GetWeapon();
            output.m_MeshFilter.sharedMesh = meshCollection.GetRandom();
            output.m_MeshRenderer.sharedMaterial = m_MaterialCollection.GetRandom();
            return output;
        }

        public WeaponArchetype GenerateSword() => GenerateWeapon(m_SwordMeshCollection);
        public WeaponArchetype GenerateShield() => GenerateWeapon(m_ShieldMeshCollection);
        public WeaponArchetype GenerateDagger() => GenerateWeapon(m_DaggerMeshCollection);
        public WeaponArchetype GenerateStaff() => GenerateWeapon(m_StaffMeshCollection);
    }
}