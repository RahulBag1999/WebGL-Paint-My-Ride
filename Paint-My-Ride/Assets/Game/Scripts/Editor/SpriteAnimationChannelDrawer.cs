using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(SpriteAnimationChannel))]
public class SpriteAnimationChannelDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float height = 0f;
        float spacing = 4f;

        // Base fields
        height += EditorGUIUtility.singleLineHeight + spacing; // channelName
        height += EditorGUIUtility.singleLineHeight + spacing; // targetType
        height += EditorGUIUtility.singleLineHeight + spacing; // renderer/image

        // Animations (THIS is important)
        var animations = property.FindPropertyRelative("animations");
        height += EditorGUI.GetPropertyHeight(animations, true);

        return height;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        float lineHeight = EditorGUIUtility.singleLineHeight;
        float spacing = 4f;

        Rect rect = new Rect(position.x, position.y, position.width, lineHeight);

        var channelName = property.FindPropertyRelative("channelName");
        var targetType = property.FindPropertyRelative("targetType");
        var spriteRenderer = property.FindPropertyRelative("spriteRenderer");
        var uiImage = property.FindPropertyRelative("uiImage");
        var animations = property.FindPropertyRelative("animations");

        // Channel Name
        EditorGUI.PropertyField(rect, channelName);
        rect.y += lineHeight + spacing;

        // Target Type
        EditorGUI.PropertyField(rect, targetType);
        rect.y += lineHeight + spacing;

        // Conditional field
        if ((RenderTargetType)targetType.enumValueIndex == RenderTargetType.SpriteRenderer)
        {
            EditorGUI.PropertyField(rect, spriteRenderer);
        }
        else
        {
            EditorGUI.PropertyField(rect, uiImage);
        }

        rect.y += lineHeight + spacing;

        // 🔥 IMPORTANT: pass TRUE so Unity handles expansion properly
        float animHeight = EditorGUI.GetPropertyHeight(animations, true);
        Rect animRect = new Rect(rect.x, rect.y, rect.width, animHeight);
        EditorGUI.PropertyField(animRect, animations, true);

        EditorGUI.EndProperty();
    }
}