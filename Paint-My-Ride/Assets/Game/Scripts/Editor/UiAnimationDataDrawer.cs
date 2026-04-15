using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(UIAnimator.AnimationData))]
public class UIAnimationDataDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        int lines = 4; // type + duration + delay + ease

        var typeProp = property.FindPropertyRelative("type");

        switch ((UIAnimator.AnimationType)typeProp.enumValueIndex)
        {
            case UIAnimator.AnimationType.Move:
                lines += 2;
                break;

            case UIAnimator.AnimationType.Scale:
                lines += 2;
                break;

            case UIAnimator.AnimationType.Fade:
                lines += 2;
                break;
        }

        return lines * EditorGUIUtility.singleLineHeight + (lines * 2);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        float lineHeight = EditorGUIUtility.singleLineHeight;
        float spacing = 2f;
        Rect rect = new Rect(position.x, position.y, position.width, lineHeight);

        // Properties
        var typeProp = property.FindPropertyRelative("type");
        var durationProp = property.FindPropertyRelative("duration");
        var delayProp = property.FindPropertyRelative("delay");
        var easeProp = property.FindPropertyRelative("ease");

        // Draw common fields
        EditorGUI.PropertyField(rect, typeProp);
        rect.y += lineHeight + spacing;

        EditorGUI.PropertyField(rect, durationProp);
        rect.y += lineHeight + spacing;

        EditorGUI.PropertyField(rect, delayProp);
        rect.y += lineHeight + spacing;

        EditorGUI.PropertyField(rect, easeProp);
        rect.y += lineHeight + spacing;

        // Draw conditional fields
        switch ((UIAnimator.AnimationType)typeProp.enumValueIndex)
        {
            case UIAnimator.AnimationType.Move:
                EditorGUI.PropertyField(rect, property.FindPropertyRelative("moveFrom"));
                rect.y += lineHeight + spacing;

                EditorGUI.PropertyField(rect, property.FindPropertyRelative("moveTo"));
                break;

            case UIAnimator.AnimationType.Scale:
                EditorGUI.PropertyField(rect, property.FindPropertyRelative("scaleFrom"));
                rect.y += lineHeight + spacing;

                EditorGUI.PropertyField(rect, property.FindPropertyRelative("scaleTo"));
                break;

            case UIAnimator.AnimationType.Fade:
                EditorGUI.PropertyField(rect, property.FindPropertyRelative("fadeFrom"));
                rect.y += lineHeight + spacing;

                EditorGUI.PropertyField(rect, property.FindPropertyRelative("fadeTo"));
                break;
        }

        EditorGUI.EndProperty();
    }
}