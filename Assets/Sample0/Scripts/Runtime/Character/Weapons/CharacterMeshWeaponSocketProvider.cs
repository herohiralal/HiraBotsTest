using System.Collections.Generic;
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

                m_CurrentlyAttached.m_AttackCollisionHelper.enabled = false;
                m_CurrentlyAttached.m_IdleCollisionHelper.enabled = false;
            }

            public WeaponArchetype Detach()
            {
                m_CurrentlyAttached.m_AttackCollisionHelper.enabled = false;
                m_CurrentlyAttached.m_IdleCollisionHelper.enabled = true;

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
            public static WeaponSockets defaultValue { get; } = new WeaponSockets
            {
                m_SheathedSocket = Socket.defaultValue,
                m_EquippedSocket = Socket.defaultValue,
                m_Equipped = false
            };

            public Socket m_SheathedSocket;
            public Socket m_EquippedSocket;
            [System.NonSerialized] public bool m_Equipped;

            public void Sheathe()
            {
                m_Equipped = false;
                m_SheathedSocket.Attach(m_EquippedSocket.Detach());
            }

            public void Unsheathe()
            {
                m_Equipped = true;
                m_EquippedSocket.Attach(m_SheathedSocket.Detach());
            }
        }

        [SerializeField] public WeaponSockets m_SwordSockets = WeaponSockets.defaultValue;
        [SerializeField] public WeaponSockets m_ShieldSockets = WeaponSockets.defaultValue;
        [SerializeField] public WeaponSockets m_DaggerLSockets = WeaponSockets.defaultValue;
        [SerializeField] public WeaponSockets m_DaggerRSockets = WeaponSockets.defaultValue;
        [SerializeField] public WeaponSockets m_StaffSockets = WeaponSockets.defaultValue;

        public bool GetEquippedWeaponR(out WeaponArchetype weaponArchetype)
        {
            if (m_SwordSockets.m_Equipped)
            {
                weaponArchetype = m_SwordSockets.m_EquippedSocket.currentlyAttached;
                return true;
            }

            if (m_DaggerRSockets.m_Equipped)
            {
                weaponArchetype = m_SwordSockets.m_EquippedSocket.currentlyAttached;
                return true;
            }

            if (m_StaffSockets.m_Equipped)
            {
                weaponArchetype = m_SwordSockets.m_EquippedSocket.currentlyAttached;
                return true;
            }

            weaponArchetype = null;
            return false;
        }

        public bool GetEquippedWeaponL(out WeaponArchetype weaponArchetype)
        {
            if (m_DaggerLSockets.m_Equipped)
            {
                weaponArchetype = m_DaggerLSockets.m_EquippedSocket.currentlyAttached;
                return true;
            }

            weaponArchetype = null;
            return false;
        }
    }
}