#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public static class RectTransformAnchorTool
{
    /// <summary>
    /// Anchors the selected RectTransform to its corners.
    /// Shortcut: Shift + Alt + Q (Shift + Option + Q on Mac)
    /// </summary>
    [MenuItem("Tools/RectTransform/Anchor to Corners #&q")]
    private static void AnchorToCorners()
    {
        // Check if a RectTransform is selected
        if (Selection.activeGameObject == null)
        {
            Debug.LogWarning("No GameObject selected. Please select a RectTransform in the hierarchy.");
            return;
        }

        if (!Selection.activeGameObject.TryGetComponent<RectTransform>(out RectTransform rectTransform))
        {
            Debug.LogWarning("Selected GameObject does not have a RectTransform component.");
            return;
        }

        // Record the object for undo
        Undo.RecordObject(rectTransform, "Anchor to Corners");

        // Get the parent RectTransform
        RectTransform parent = rectTransform.parent as RectTransform;

        if (parent == null)
        {
            Debug.LogWarning("RectTransform must have a parent with RectTransform component.");
            return;
        }

        // Get current offsets and anchors
        Vector2 offsetMin = rectTransform.offsetMin;
        Vector2 offsetMax = rectTransform.offsetMax;
        Vector2 anchorMin = rectTransform.anchorMin;
        Vector2 anchorMax = rectTransform.anchorMax;

        // Get parent size
        Vector2 parentSize = parent.rect.size;

        // Calculate the new anchor positions by incorporating the current offsets
        // This moves the anchors to the corners while preserving visual position
        Vector2 newAnchorMin = new Vector2(
            anchorMin.x + offsetMin.x / parentSize.x,
            anchorMin.y + offsetMin.y / parentSize.y
        );

        Vector2 newAnchorMax = new Vector2(
            anchorMax.x + offsetMax.x / parentSize.x,
            anchorMax.y + offsetMax.y / parentSize.y
        );

        // Apply the new anchors
        rectTransform.anchorMin = newAnchorMin;
        rectTransform.anchorMax = newAnchorMax;

        // Now set offsets to zero (this won't move the object since anchors were adjusted)
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;

        Debug.Log($"Anchored '{rectTransform.name}' to its corners. All offsets are now 0.");
    }

    /// <summary>
    /// Validate the menu item (only enabled when a RectTransform is selected)
    /// </summary>
    [MenuItem("Tools/RectTransform/Anchor to Corners #&q", true)]
    private static bool ValidateAnchorToCorners()
    {
        return Selection.activeGameObject != null &&
               Selection.activeGameObject.GetComponent<RectTransform>() != null;
    }
}
#endif
