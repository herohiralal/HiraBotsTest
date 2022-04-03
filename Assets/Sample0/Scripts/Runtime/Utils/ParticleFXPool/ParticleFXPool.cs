using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AIEngineTest
{
    [CreateAssetMenu(menuName = "Samples/ParticleFXPool")]
    public class ParticleFXPool : ScriptableObject
    {
        [SerializeField] private OneShotParticleFX m_FX;

        private readonly Stack<OneShotParticleFX> m_Pool = new Stack<OneShotParticleFX>();

        public OneShotParticleFX Get(Vector3 position)
        {
            return Get(position, Quaternion.identity);
        }

        public OneShotParticleFX Get(Vector3 position, Quaternion rotation)
        {
            return Get(position, rotation, Vector3.one);
        }

        public OneShotParticleFX Get(Vector3 position, Quaternion rotation, Vector3 scale)
        {
            var output = m_Pool.Count > 0 ? m_Pool.Pop() : Instantiate(m_FX);

            var transform = output.transform;
            transform.SetPositionAndRotation(position, rotation);
            transform.localScale = scale;

            output.gameObject.SetActive(true);

            output.StartCoroutine(ReturnToPoolCoroutine(output));

            return output;
        }

        private IEnumerator ReturnToPoolCoroutine(OneShotParticleFX fx)
        {
            yield return new WaitForSeconds(fx.m_Timer);

            fx.gameObject.SetActive(false);
            m_Pool.Push(fx);
        }

        public void Clear()
        {
            m_Pool.Clear();
        }
    }
}