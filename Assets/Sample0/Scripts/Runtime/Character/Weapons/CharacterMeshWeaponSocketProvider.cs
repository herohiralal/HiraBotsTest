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

        [System.Serializable]
        public struct WeaponSockets
        {
            public Socket m_SheathedSocket;
            public Socket m_EquippedSocket;

            public void Sheathe()
            {
                m_SheathedSocket.Attach(m_EquippedSocket.Detach());
            }

            public void Unsheathe()
            {
                m_EquippedSocket.Attach(m_SheathedSocket.Detach());
            }
        }

        [SerializeField] public WeaponSockets m_SwordSockets;
        [SerializeField] public WeaponSockets m_ShieldSockets;
        [SerializeField] public WeaponSockets m_DaggerLSockets;
        [SerializeField] public WeaponSockets m_DaggerRSockets;
        [SerializeField] public WeaponSockets m_StaffSockets;
    }
}