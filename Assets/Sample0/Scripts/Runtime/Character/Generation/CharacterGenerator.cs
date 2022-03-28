using System.Collections.Generic;
using UnityEngine;

namespace AIEngineTest
{
    [CreateAssetMenu(fileName = "CharacterGenerator", menuName = "Samples/CharacterGenerator", order = 0)]
    public class CharacterGenerator : ScriptableObject
    {
        [SerializeField] private BaseArchetype m_BaseArchetypePrefab;

        [SerializeField] private MeshCollection m_MeshCollection;
        [SerializeField] private MaterialCollection m_MaterialCollection;

        private readonly Stack<BaseArchetype> m_Archetypes = new Stack<BaseArchetype>();

        public void ClearPool()
        {
            m_Archetypes.Clear();
        }

        private BaseArchetype GetArchetype(Vector3 position, Quaternion rotation)
        {
            if (m_Archetypes.TryPop(out var pooled))
            {
                pooled.gameObject.SetActive(true);
                var t = pooled.transform;
                t.position = position;
                t.rotation = rotation;
                return pooled;
            }

            return Instantiate(m_BaseArchetypePrefab, position, rotation);
        }

        public void Discard(BaseArchetype arch)
        {
            arch.gameObject.SetActive(false);
            arch.m_SkinnedMeshRenderer.sharedMesh = null;
            arch.m_SkinnedMeshRenderer.sharedMaterial = null;
        }

        public BaseArchetype Generate(Vector3 position)
        {
            return Generate(position, Quaternion.identity);
        }

        public BaseArchetype Generate(Vector3 position, Quaternion rotation)
        {
            var arch = GetArchetype(position, rotation);
            arch.m_SkinnedMeshRenderer.sharedMesh = m_MeshCollection.GetRandom();
            arch.m_SkinnedMeshRenderer.sharedMaterial = m_MaterialCollection.GetRandom();
            return arch;
        }
    }
}