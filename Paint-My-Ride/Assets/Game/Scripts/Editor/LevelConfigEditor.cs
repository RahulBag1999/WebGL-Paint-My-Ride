using UnityEngine;


using UnityEditor;

[CustomEditor(typeof(LevelConfig))]
public class LevelConfigEditor : Editor
{
    private LevelConfig config;
    private ColorData colorData;
    private const int CellSize = 40;

    void OnEnable()
    {
        config = (LevelConfig)target;
        LoadColorData();
    }

    private void LoadColorData()
    {
        // Find all ColorData assets
        string[] guids = AssetDatabase.FindAssets("t:ColorData");

        if (guids.Length == 0)
        {
            Debug.LogWarning("No ColorData asset found in project.");
            return;
        }

        if (guids.Length > 1)
        {
            Debug.LogWarning("Multiple ColorData assets found. Using the first one.");
        }

        string path = AssetDatabase.GUIDToAssetPath(guids[0]);
        colorData = AssetDatabase.LoadAssetAtPath<ColorData>(path);
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GUILayout.Space(10);

        // ---- GRID ACTION BUTTONS ----
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Create Grid"))
        {
            Undo.RecordObject(config, "Initiate Grid");
            config.InitializeGrid();
            EditorUtility.SetDirty(config);
        }

        if (GUILayout.Button("Delete Grid"))
        {
            DeleteGrid();
        }

        GUILayout.EndHorizontal();

        if (config.fullGrid == null || config.fullGrid.Length == 0)
            return;

        // CUSTOM MESSAGE SECTION (ABOVE GRID)
        GUILayout.Space(8);
        EditorGUILayout.HelpBox(
            "N/A = Non assigned cell\n" +
            "EC = Empty cell\n" +
            "MC = Movable cell\n" +
            "NMC = Non movable cell",
            MessageType.None
        );

        GUILayout.Space(10);
        DrawGrid();
    }

    void DeleteGrid()
    {
        if (!EditorUtility.DisplayDialog(
            "Delete Grid",
            "Are you sure you want to delete the grid?\nThis action cannot be undone.",
            "Delete",
            "Cancel"))
        {
            return;
        }

        Undo.RecordObject(config, "Delete Grid");
        config.Cleanup();
        EditorUtility.SetDirty(config);
    }

    void DrawGrid()
    {
        int rows = config.Rows;
        int cols = config.Columns;

        for (int r = 0; r < rows; r++)
        {
            GUILayout.BeginHorizontal();

            GUILayout.FlexibleSpace(); // LEFT SPACING

            for (int c = 0; c < cols; c++)
            {
                int index = config.GetCellIndex(r, c);
                if (index < 0) continue;

                LevelConfig.GridData cell = config.fullGrid[index];

                GUI.backgroundColor = GetColor(cell.cellType);

                string cellName = GetCellName(cell.cellType) + "\n" + r + "," + c;

                if (GUILayout.Button(
                    cellName,
                    GUILayout.Width(CellSize),
                    GUILayout.Height(CellSize)))
                {
                    ShowEnumPopup(cell);
                }

                GUI.backgroundColor = Color.white;
            }

            GUILayout.FlexibleSpace(); // RIGHT SPACING

            GUILayout.EndHorizontal();
        }
    }


    void ShowEnumPopup(LevelConfig.GridData cell)
    {
        GenericMenu menu = new GenericMenu();

        foreach (GridCellType type in System.Enum.GetValues(typeof(GridCellType)))
        {
            bool isSelected = cell.cellType == type;

            menu.AddItem(
                new GUIContent(type.ToString()),
                isSelected,
                () =>
                {
                    Undo.RecordObject(config, "Change Grid Cell Type");
                    cell.cellType = type;
                    EditorUtility.SetDirty(config);
                }
            );
        }

        menu.ShowAsContext();
    }

    private string GetCellName(GridCellType cellType)
    {
        switch (cellType)
        {
            case GridCellType.EMPTY:
                return "EC";

            case GridCellType.REDCELL:
            case GridCellType.GREENCELL:
            case GridCellType.BLUECELL:
            case GridCellType.YELLOWCELL:
            case GridCellType.PURPLECELL:
            case GridCellType.ORANGECELL:
                return "NMC";

            case GridCellType.REDCAT:
            case GridCellType.GREENCAT:
            case GridCellType.BLUECAT:
            case GridCellType.YELLOWCAT:
            case GridCellType.PURPLECAT:
            case GridCellType.ORANGECAT:
                return "MC";
        }
        return "N/A";
    }

    private Color GetColor(GridCellType cellType)
    {
        if (colorData == null)
            return Color.white;

        ColorCode colorCode = Utility.GetColorCodeByGridCellType(cellType);

        if (colorCode == ColorCode.NONE)
            return Color.white;

        ColorData.ColorDatum datum = colorData.GetColorDatum(colorCode);

        if (datum == null)
            return Color.white;

        return datum.color;
    }
}