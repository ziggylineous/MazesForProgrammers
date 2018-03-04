using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RectGrid))]
public class MazeEditor : Editor
{
    private int prevWidth;
    private int prevHeight;

    private int prevBuilderIndex;
    private string[] builderNames;
    private CellLinker[] builders;

    private RectGridImage image;


    public void Awake()
    {
        Debug.Log("MazeEditor::Awake()");
        builderNames = new string[]{
            //"Binary",
            //"Sidewinder",
            "Random Walk",
            "Hunt and Kill",
            "Recursive Backtracker"
        };

        builders = new CellLinker[] {
            //new BinaryCellLinker(),
            //new SidewinderCellLinker(),
            new RandomWalkCellLinker(),
            new HuntAndKillCellLinker(),
            new RecursiveBacktrackerCellLinker()
        };

        prevBuilderIndex = 0;

        image = new RectGridImage(30, Color.white, Color.blue);
    }

    public void OnEnable()
    {
        Debug.Log("RectGridEditor::OnEnable()");

        SerializedProperty widthSerialized = serializedObject.FindProperty("width");
        SerializedProperty heightSerialized = serializedObject.FindProperty("height");

        prevWidth = widthSerialized.intValue;
        prevHeight = heightSerialized.intValue;

        image.Draw((RectGrid)serializedObject.targetObject);
    }

    public override void OnInspectorGUI()
    {
        RectGrid rectGrid = (RectGrid) target;

        int newWidth = EditorGUILayout.IntField("width", rectGrid.width);
        int newHeight = EditorGUILayout.IntField("height", rectGrid.height);
        EditorGUILayout.LabelField("cell count", rectGrid.graph.Size.ToString());

        if (newWidth != prevWidth || newHeight != prevHeight)
        {
            prevWidth = newWidth;
            prevHeight = newHeight;
            ChangeSize(newWidth, newHeight);
        }

        int builderIndex = EditorGUILayout.Popup(prevBuilderIndex, builderNames);
        if (builderIndex != prevBuilderIndex || GUILayout.Button("rebuild"))
        {
            prevBuilderIndex = builderIndex;
            RebuildMaze(builders[builderIndex]);
        }

        DisplayImage();
    }

    private void ChangeSize(int width, int height)
    {
        RectGrid rectGrid = (RectGrid)target;

        rectGrid.SetSize(width, height);

        EditorUtility.SetDirty(target);

        image.Draw(rectGrid);
    }

    private void DisplayImage()
    {
        int newCellSize = EditorGUILayout.IntField("cellSize", image.CellSize);

        if (newCellSize != image.CellSize)
        {
            image.CellSize = newCellSize;
            image.Draw((RectGrid)target);
        }

        if (prevWidth > 0 && prevHeight > 0)
        {
            Rect lastRect = GUILayoutUtility.GetLastRect();
            //lastRect.
            EditorGUI.DrawPreviewTexture(
                new Rect(lastRect.position.x, lastRect.yMax + 10, image.CellSize * prevWidth, image.CellSize * prevHeight),
                image.Tex
            );
        }

        if (GUILayout.Button("Save Image"))
        {
            image.Tex.Save("maze");
        }
    }

    private void RebuildMaze(CellLinker builder)
    {
        Debug.Log("rebuilding " + builder.ToString());

        RectGrid maze = (RectGrid)target;

        maze.graph.ClearLinks();
        builder.Build(maze);
        EditorUtility.SetDirty(maze);

        BreadthFirst bf = new BreadthFirst(maze.graph, maze.BottomLeftVertex);
        bf.Run();
        int[] distances = bf.Distances;
        float maxDistance = (float)bf.MaxDistance;
        Color nearColor = Color.red;
        Color farColor = Color.black;

        Color[] distanceColors = System.Array.ConvertAll<int, Color>(
            distances, distance => Color.Lerp(nearColor, farColor, (distance / maxDistance))
        );
        image.Draw(maze, distanceColors);
    }
}