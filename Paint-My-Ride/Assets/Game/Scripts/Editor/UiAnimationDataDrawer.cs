using UnityEngine;

using UnityEditor;

[CustomPropertyDrawer(typeof(UIAnimator.AnimationData))]
public class UIAnimationDataDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        int lines = 3; // type + duration + ease

        var typeProp = property.FindPropertyRelative("type");

        if (typeProp != null)
        {
            switch ((UIAnimator.AnimationType)typeProp.enumValueIndex)
            {
                case UIAnimator.AnimationType.Move:
                case UIAnimator.AnimationType.Scale:
                case UIAnimator.AnimationType.Fade:
                    lines += 2;
                    break;
            }
        }

        return lines * (EditorGUIUtility.singleLineHeight + 2f);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        float lineHeight = EditorGUIUtility.singleLineHeight;
        float spacing = 2f;

        Rect rect = new Rect(
            position.x,
            position.y,
            position.width,
            lineHeight);

        var typeProp = property.FindPropertyRelative("type");
        var durationProp = property.FindPropertyRelative("duration");
        var easeProp = property.FindPropertyRelative("ease");

        if (typeProp == null)
        {
            EditorGUI.LabelField(rect, "AnimationData property error");
            EditorGUI.EndProperty();
            return;
        }

        // Common Fields
        EditorGUI.PropertyField(rect, typeProp);
        rect.y += lineHeight + spacing;

        if (durationProp != null)
        {
            EditorGUI.PropertyField(rect, durationProp);
            rect.y += lineHeight + spacing;
        }

        if (easeProp != null)
        {
            EditorGUI.PropertyField(rect, easeProp);
            rect.y += lineHeight + spacing;
        }

        // Conditional Fields
        switch ((UIAnimator.AnimationType)typeProp.enumValueIndex)
        {
            case UIAnimator.AnimationType.Move:
                {
                    var moveFromProp = property.FindPropertyRelative("moveFrom");
                    var moveToProp = property.FindPropertyRelative("moveTo");

                    if (moveFromProp != null)
                    {
                        EditorGUI.PropertyField(rect, moveFromProp);
                        rect.y += lineHeight + spacing;
                    }

                    if (moveToProp != null)
                    {
                        EditorGUI.PropertyField(rect, moveToProp);
                    }

                    break;
                }

            case UIAnimator.AnimationType.Scale:
                {
                    var scaleFromProp = property.FindPropertyRelative("scaleFrom");
                    var scaleToProp = property.FindPropertyRelative("scaleTo");

                    if (scaleFromProp != null)
                    {
                        EditorGUI.PropertyField(rect, scaleFromProp);
                        rect.y += lineHeight + spacing;
                    }

                    if (scaleToProp != null)
                    {
                        EditorGUI.PropertyField(rect, scaleToProp);
                    }

                    break;
                }

            case UIAnimator.AnimationType.Fade:
                {
                    var fadeFromProp = property.FindPropertyRelative("fadeFrom");
                    var fadeToProp = property.FindPropertyRelative("fadeTo");

                    if (fadeFromProp != null)
                    {
                        EditorGUI.PropertyField(rect, fadeFromProp);
                        rect.y += lineHeight + spacing;
                    }

                    if (fadeToProp != null)
                    {
                        EditorGUI.PropertyField(rect, fadeToProp);
                    }

                    break;
                }
        }

        EditorGUI.EndProperty();
    }
}