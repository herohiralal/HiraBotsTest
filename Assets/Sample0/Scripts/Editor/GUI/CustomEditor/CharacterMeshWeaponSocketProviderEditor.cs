using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AIEngineTest.Editor
{
    [CustomEditor(typeof(CharacterMeshWeaponSocketProvider))]
    public class CharacterMeshWeaponSocketProviderEditor : UnityEditor.Editor
    {
        private struct SocketProperty
        {
            public SerializedProperty m_Parent;
            public SerializedProperty m_Socket;
            public SerializedProperty m_PositionOffset;
            public SerializedProperty m_RotationOffset;
        }

        private struct WeaponSocketsProperty
        {
            public SerializedProperty m_Parent;
            public SerializedProperty m_SheathedSocket;
            public SerializedProperty m_EquippedSocket;
        }

        private IEnumerable<WeaponSocketsProperty> GetWeaponSocketsProperties()
        {
            WeaponSocketsProperty GetWeaponSocketsProperty(SerializedProperty parent)
            {
                var output = new WeaponSocketsProperty
                {
                    m_Parent = parent,
                    m_SheathedSocket = parent.FindPropertyRelative(nameof(CharacterMeshWeaponSocketProvider.WeaponSockets.m_SheathedSocket)),
                    m_EquippedSocket = parent.FindPropertyRelative(nameof(CharacterMeshWeaponSocketProvider.WeaponSockets.m_EquippedSocket))
                };
                return output;
            }

            yield return GetWeaponSocketsProperty(serializedObject.FindProperty(nameof(CharacterMeshWeaponSocketProvider.m_SwordSockets)));
            yield return GetWeaponSocketsProperty(serializedObject.FindProperty(nameof(CharacterMeshWeaponSocketProvider.m_ShieldSockets)));
            yield return GetWeaponSocketsProperty(serializedObject.FindProperty(nameof(CharacterMeshWeaponSocketProvider.m_DaggerLSockets)));
            yield return GetWeaponSocketsProperty(serializedObject.FindProperty(nameof(CharacterMeshWeaponSocketProvider.m_DaggerRSockets)));
            yield return GetWeaponSocketsProperty(serializedObject.FindProperty(nameof(CharacterMeshWeaponSocketProvider.m_StaffSockets)));
        }

        private static IEnumerable<SocketProperty> GetSocketProperties(WeaponSocketsProperty weaponSocketsProperty)
        {
            SocketProperty GetSocketProperty(SerializedProperty parent)
            {
                var output = new SocketProperty
                {
                    m_Parent = parent,
                    m_Socket = parent.FindPropertyRelative(nameof(CharacterMeshWeaponSocketProvider.Socket.m_Socket)),
                    m_PositionOffset = parent.FindPropertyRelative(nameof(CharacterMeshWeaponSocketProvider.Socket.m_PositionOffset)),
                    m_RotationOffset = parent.FindPropertyRelative(nameof(CharacterMeshWeaponSocketProvider.Socket.m_RotationOffset))
                };
                return output;
            }

            yield return GetSocketProperty(weaponSocketsProperty.m_SheathedSocket);
            yield return GetSocketProperty(weaponSocketsProperty.m_EquippedSocket);
        }

        private static void DrawInspectorGUI(WeaponSocketsProperty weaponSocketsProperty)
        {
            var labelPrefix = weaponSocketsProperty.m_Parent.displayName.Replace(" Sockets", "") + " ";
            foreach (var socketProperty in GetSocketProperties(weaponSocketsProperty))
            {
                var label = GUIUtils.TempContent(
                    labelPrefix + socketProperty.m_Parent.displayName.Replace(" Socket", ""),
                    weaponSocketsProperty.m_Parent.tooltip);
                EditorGUILayout.PropertyField(socketProperty.m_Socket, label);

                if (socketProperty.m_Socket.objectReferenceValue == null)
                {
                    continue;
                }

                var indent = EditorGUI.indentLevel;
                EditorGUI.indentLevel++;

                socketProperty.m_Socket.isExpanded = EditorGUILayout.Toggle("Show Gizmo", socketProperty.m_Socket.isExpanded);
                EditorGUILayout.PropertyField(socketProperty.m_PositionOffset);
                EditorGUILayout.PropertyField(socketProperty.m_RotationOffset);

                EditorGUI.indentLevel = indent;
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            foreach (var weaponSocketsProperty in GetWeaponSocketsProperties())
            {
                DrawInspectorGUI(weaponSocketsProperty);
            }

            serializedObject.ApplyModifiedProperties();
        }

        private static void DrawSceneGUI(SocketProperty property)
        {
            if (!property.m_Socket.isExpanded)
            {
                return;
            }

            var socket = property.m_Socket.objectReferenceValue as Transform;
            if (socket == null)
            {
                return;
            }

            var original = Handles.matrix;
            Handles.matrix = socket.localToWorldMatrix;

            var position = property.m_PositionOffset.vector3Value;
            var rotation = property.m_RotationOffset.quaternionValue;

            Handles.TransformHandle(ref position, ref rotation);
            property.m_PositionOffset.vector3Value = position;
            property.m_RotationOffset.quaternionValue = rotation;

            Handles.matrix = original;
        }

        private void OnSceneGUI()
        {
            serializedObject.Update();

            foreach (var weaponSocketsProperty in GetWeaponSocketsProperties())
            {
                foreach (var socketProperty in GetSocketProperties(weaponSocketsProperty))
                {
                    DrawSceneGUI(socketProperty);
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}