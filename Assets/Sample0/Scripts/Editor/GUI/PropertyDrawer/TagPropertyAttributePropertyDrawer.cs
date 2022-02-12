using UnityEditor;
using UnityEngine;

namespace Sample0.Scripts.Editor.GUI
{
    [CustomPropertyDrawer(typeof(TagPropertyAttributePropertyDrawer))]
    public class TagPropertyAttributePropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.String)
            {
                EditorGUI.HelpBox(position, "[TagProperty] can only be used with string properties.", MessageType.Error);
                return;
            }

            property.stringValue = EditorGUI.TagField(position, label, property.stringValue);
        }
    }
}