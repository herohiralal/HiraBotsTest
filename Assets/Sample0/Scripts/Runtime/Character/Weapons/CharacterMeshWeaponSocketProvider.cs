using UnityEngine;

namespace AIEngineTest
{
    public class CharacterMeshWeaponSocketProvider : MonoBehaviour
    {
        [System.Serializable]
        public struct Socket
        {
            public static Socket defaultValue { get; } = new Socket
            {
                m_Socket = null,
                m_PositionOffset = Vector3.zero,
                m_RotationOffset = Quaternion.identity
            };

            [SerializeField] public Transform m_Socket;
            [SerializeField] public Vector3 m_PositionOffset;
            [SerializeField] public Quaternion m_RotationOffset;
            private WeaponArchetype m_CurrentlyAttached;

            public WeaponArchetype currentlyAttached => m_CurrentlyAttached;

            public void Attach(WeaponArchetype go)
            {
                var t = go.transform;
                t.SetParent(m_Socket);
                t.localPosition = m_PositionOffset;
                t.localRotation = m_RotationOffset;
                m_CurrentlyAttached = go;
            }

            public WeaponArchetype Detach()
            {
                var go = m_CurrentlyAttached;
                var t = go.transform;
                t.SetParent(null);
                m_CurrentlyAttached = null;
                return go;
            }
        }

        [SerializeField] public Socket m_HandLSocket = Socket.defaultValue;
        [SerializeField] public Socket m_HandRSocket = Socket.defaultValue;

        [SerializeField] public Socket m_BackSocket = Socket.defaultValue;
        [SerializeField] public Socket m_BackLSocket = Socket.defaultValue;
        [SerializeField] public Socket m_BackRSocket = Socket.defaultValue;

        [SerializeField] public Socket m_HipsLSocket = Socket.defaultValue;
        [SerializeField] public Socket m_HipsRSocket = Socket.defaultValue;
    }
}