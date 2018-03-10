using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(WeaveRectGrid))]
public class WeaveRectGridEditor : Editor
{
    private int prevWidth;
    private int prevHeight;

    private int prevBuilderIndex;
    private string[] builderNames;
    private VertexLinker[] builders;

    private WeaveRectGridImage image;


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

        builders = new VertexLinker[] {
            //new BinaryCellLinker(),
            //new SidewinderCellLinker(),
            new RandomWalkVertexLinker(),
            new HuntAndKillVertexLinker(),
            new RecursiveBacktrackerVertexLinker()
        };

        prevBuilderIndex = 0;

        image = new WeaveRectGridImage(30, 4, Color.white, Color.blue);
    }

    public void OnEnable()
    {
        Debug.Log("WeaveRectGridEditor::OnEnable()");

        SerializedProperty widthSerialized = serializedObject.FindProperty("width");
        SerializedProperty heightSerialized = serializedObject.FindProperty("height");

        prevWidth = widthSerialized.intValue;
        prevHeight = heightSerialized.intValue;

        if (prevWidth > 0 && prevHeight > 0)
            image.Draw((WeaveRectGrid)serializedObject.targetObject);
    }

    public override void OnInspectorGUI()
    {
        WeaveRectGrid rectGrid = (WeaveRectGrid) target;

        int newWidth = EditorGUILayout.IntField("width", rectGrid.width);
        int newHeight = EditorGUILayout.IntField("height", rectGrid.height);
        EditorGUILayout.LabelField("cell count", rectGrid.Graph.Size.ToString());

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
        WeaveRectGrid rectGrid = (WeaveRectGrid)target;

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
            image.Draw((WeaveRectGrid)target);
        }

        int newInset = EditorGUILayout.IntField("inset", image.Inset);

        if (newInset != image.Inset)
        {
            image.Inset = newInset;
            image.Draw((WeaveRectGrid)target);
        }

        if (prevWidth > 0 && prevHeight > 0)
        {
            Rect lastRect = GUILayoutUtility.GetLastRect();

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

    private void RebuildMaze(VertexLinker builder)
    {
        Debug.Log("rebuilding " + builder.ToString());

        WeaveRectGrid maze = (WeaveRectGrid)target;

        maze.ClearTunnels();
        maze.Graph.ClearLinks();
        builder.Build(new WeaveRectGridLinkerHelper(maze));
        EditorUtility.SetDirty(maze);

        /*BreadthFirst bf = new BreadthFirst(maze.Graph, maze.BottomLeftVertex);
        bf.Run();
        int[] distances = bf.Distances;
        float maxDistance = (float)bf.MaxDistance;
        Color nearColor = Color.red;
        Color farColor = Color.black;

        Color[] distanceColors = System.Array.ConvertAll<int, Color>(
            distances, distance => Color.Lerp(nearColor, farColor, (distance / maxDistance))
        );
        image.Draw(maze, distanceColors);*/

        image.Draw(maze);
    }
}