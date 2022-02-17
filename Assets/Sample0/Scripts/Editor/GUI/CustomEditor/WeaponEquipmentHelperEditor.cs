using UnityEditor;
using UnityEngine;

namespace AIEngineTest.Editor
{
    [CustomEditor(typeof(CharacterMeshWeaponSocketProvider))]
    public class WeaponEquipmentHelperEditor : UnityEditor.Editor
    {
        private struct SocketProperty
        {
            public SerializedProperty m_Parent;
            public SerializedProperty m_Socket;
            public SerializedProperty m_PositionOffset;
            public SerializedProperty m_RotationOffset;
        }

        private void GetProperties(
            out SocketProperty handL, out SocketProperty handR,
            out SocketProperty back,
            out SocketProperty backL, out SocketProperty backR,
            out SocketProperty hipsL, out SocketProperty hipsR)
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

            handL = GetSocketProperty(serializedObject.FindProperty(nameof(CharacterMeshWeaponSocketProvider.m_HandLSocket)));
            handR = GetSocketProperty(serializedObject.FindProperty(nameof(CharacterMeshWeaponSocketProvider.m_HandRSocket)));

            back = GetSocketProperty(serializedObject.FindProperty(nameof(CharacterMeshWeaponSocketProvider.m_BackSocket)));

            hipsL = GetSocketProperty(serializedObject.FindProperty(nameof(CharacterMeshWeaponSocketProvider.m_HipsLSocket)));
            hipsR = GetSocketProperty(serializedObject.FindProperty(nameof(CharacterMeshWeaponSocketProvider.m_HipsRSocket)));

            backL = GetSocketProperty(serializedObject.FindProperty(nameof(CharacterMeshWeaponSocketProvider.m_BackLSocket)));
            backR = GetSocketProperty(serializedObject.FindProperty(nameof(CharacterMeshWeaponSocketProvider.m_BackRSocket)));
        }

        private static void DrawInspectorGUI(ref SocketProperty property)
        {
            var label = GUIUtils.TempContent(property.m_Parent.displayName, property.m_Parent.tooltip);
            EditorGUILayout.PropertyField(property.m_Socket, label);

            if (property.m_Socket.objectReferenceValue == null)
            {
                return;
            }

            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel++;

            EditorGUILayout.PropertyField(property.m_PositionOffset);
            EditorGUILayout.PropertyField(property.m_RotationOffset);

            EditorGUI.indentLevel = indent;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            GetProperties(
                out var handL, out var handR,
                out var back,
                out var backL, out var backR,
                out var hipsL, out var hipsR);

            DrawInspectorGUI(ref handL);
            DrawInspectorGUI(ref handR);
            DrawInspectorGUI(ref back);
            DrawInspectorGUI(ref backL);
            DrawInspectorGUI(ref backR);
            DrawInspectorGUI(ref hipsL);
            DrawInspectorGUI(ref hipsR);

            serializedObject.ApplyModifiedProperties();
        }

        private static void DrawSceneGUI(ref SocketProperty property)
        {
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

            GetProperties(
                out var handL, out var handR,
                out var back,
                out var backL, out var backR,
                out var hipsL, out var hipsR);

            DrawSceneGUI(ref handL);
            DrawSceneGUI(ref handR);
            DrawSceneGUI(ref back);
            DrawSceneGUI(ref backL);
            DrawSceneGUI(ref backR);
            DrawSceneGUI(ref hipsL);
            DrawSceneGUI(ref hipsR);

            serializedObject.ApplyModifiedProperties();
        }
    }
}