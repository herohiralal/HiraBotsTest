using UnityEditor;
using UnityEngine;

namespace AIEngineTest.Editor
{
    [CustomPropertyDrawer(typeof(PlayMontageTaskProvider.MontageInfo))]
    public class PlayMontageTaskProviderMontageInfoPropertyDrawer : PropertyDrawer
    {
        private bool Break(SerializedProperty property,
            out SerializedProperty type,
            out SerializedProperty useExtraParam,
            out SerializedProperty extraParam,
            out SerializedProperty duration)
        {
            type = property.FindPropertyRelative(nameof(PlayMontageTaskProvider.MontageInfo.m_Type));
            useExtraParam = property.FindPropertyRelative(nameof(PlayMontageTaskProvider.MontageInfo.m_UseExtraParam));
            extraParam = property.FindPropertyRelative(nameof(PlayMontageTaskProvider.MontageInfo.m_ExtraParam));
            duration = property.FindPropertyRelative(nameof(PlayMontageTaskProvider.MontageInfo.m_Duration));

            return
                type is { propertyType: SerializedPropertyType.Enum }
                && useExtraParam is { propertyType: SerializedPropertyType.Boolean }
                && extraParam is { propertyType: SerializedPropertyType.Integer }
                && duration is { propertyType: SerializedPropertyType.Float };
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!Break(property,
                    out var type,
                    out _,
                    out _,
                    out _))
            {
                return 21f;
            }

            switch ((MontageType) type.enumValueIndex)
            {
                case MontageType.None:
                    return 21f + 21f; // 1 extra for error
                case MontageType.MeleeAttackRight:
                    return 21f + 21f; // attack type
                case MontageType.MeleeAttackLeft:
                    return 21f + 21f; // attack type
                case MontageType.Unsheathe:
                    return 21f; // owned equipment is extra param
                case MontageType.Sheathe:
                    return 21f; // current equipment is extra param
                case MontageType.Bow:
                    return 21f; // no extra param
                case MontageType.Die:
                    return 21f + 21f; // duration
                case MontageType.Hit:
                    return 21f; // no extra param
                case MontageType.Block:
                    return 21f; // no extra param
                case MontageType.Dodge:
                    return 21f; // no extra param
                case MontageType.Cast:
                    return 21f + 21f + 21f; // cast-type and 
                case MontageType.DualAttack:
                    return 21f + 21f; // attack type
                default:
                    throw new System.ArgumentOutOfRangeException();
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!Break(property,
                    out var type,
                    out var useExtraParam,
                    out var extraParam,
                    out var duration))
            {
                EditorGUI.HelpBox(position, $"Cannot draw {label.text} property.", MessageType.Error);
                return;
            }

            position.height = 19f;
            EditorGUI.PropertyField(position, type);
            switch ((MontageType) type.enumValueIndex)
            {
                case MontageType.Unsheathe:
                case MontageType.Sheathe:
                case MontageType.Bow:
                case MontageType.Hit:
                case MontageType.Block:
                case MontageType.Dodge:
                    break;
                case MontageType.None:
                    position.y += 21f;
                    EditorGUI.HelpBox(position, "Cannot play None montage.", MessageType.Error);
                    break;
                case MontageType.MeleeAttackRight:
                case MontageType.MeleeAttackLeft:
                case MontageType.DualAttack:
                {
                    position.y += 21f;

                    var useCustomParamPos = position;
                    useCustomParamPos.width = 19f;

                    var customParamPos = position;
                    customParamPos.x += 21f;
                    customParamPos.width -= 21f;

                    useExtraParam.boolValue = GUI.Toggle(useCustomParamPos, useExtraParam.boolValue, "");
                    var enabled = GUI.enabled;
                    GUI.enabled = useExtraParam.boolValue;
                    {
                        EditorGUI.PropertyField(customParamPos, extraParam);
                    }
                    GUI.enabled = enabled;
                    break;
                }
                case MontageType.Die:
                    position.y += 21f;
                    EditorGUI.PropertyField(position, duration);
                    break;
                case MontageType.Cast:
                {
                    position.y += 21f;

                    useExtraParam.boolValue = true;

                    var c = GUIUtils.TempContent(extraParam.displayName, extraParam.tooltip);
                    extraParam.intValue = (int) (SpellcastAnimationType) EditorGUI.EnumPopup(
                        position,
                        c,
                        (SpellcastAnimationType) extraParam.intValue);

                    position.y += 21f;
                    EditorGUI.PropertyField(position, duration);
                    break;
                }
            }
        }
    }
}