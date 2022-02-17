using UnityEngine;

namespace AIEngineTest
{
    public class WeaponEquipmentHelper : MonoBehaviour
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
        }

        [SerializeField] public Socket m_HandLSocket = Socket.defaultValue;
        [SerializeField] public Socket m_HandRSocket = Socket.defaultValue;

        [SerializeField] public Socket m_BackLSocket = Socket.defaultValue;
        [SerializeField] public Socket m_BackRSocket = Socket.defaultValue;

        [SerializeField] public Socket m_HipsLSocket = Socket.defaultValue;
        [SerializeField] public Socket m_HipsRSocket = Socket.defaultValue;
    }
}