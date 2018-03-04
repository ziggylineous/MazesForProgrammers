using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TriGrid))]
public class TriGridEditor : Editor
{
    private int prevWidth;
    private int prevHeight;

    private int prevBuilderIndex;
    private string[] builderNames;
    private CellLinker[] builders;

    private TriImage image;


    public void Awake()
    {
        Debug.Log("MazeEditor::Awake()");
        builderNames = new string[]{
            "Random Walk",
            "Hunt and Kill",
            "Recursive Backtracker"
        };

        builders = new CellLinker[] {
            new RandomWalkCellLinker(),
            new HuntAndKillCellLinker(),
            new RecursiveBacktrackerCellLinker()
        };

        prevBuilderIndex = 0;

        image = new TriImage(30, Color.white, Color.blue);
    }

    public void OnEnable()
    {
        Debug.Log("TriGridEditor::OnEnable()");

        SerializedProperty widthSerialized = serializedObject.FindProperty("width");
        SerializedProperty heightSerialized = serializedObject.FindProperty("height");

        prevWidth = widthSerialized.intValue;
        prevHeight = heightSerialized.intValue;

        image.Draw((TriGrid)serializedObject.targetObject);
    }

    public override void OnInspectorGUI()
    {
        TriGrid rectGrid = (TriGrid)target;

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
        TriGrid rectGrid = (TriGrid)target;

        rectGrid.SetSize(width, height);

        EditorUtility.SetDirty(target);

        image.Draw(rectGrid);
    }

    private void DisplayImage()
    {
        int newSideize = EditorGUILayout.IntField("side size", image.SideSize);

        if (newSideize != image.SideSize)
        {
            image.SideSize = newSideize;
            image.Draw((TriGrid)target);
        }

        if (prevWidth > 0 && prevHeight > 0)
        {
            Rect lastRect = GUILayoutUtility.GetLastRect();
            //lastRect.
            EditorGUI.DrawPreviewTexture(
                new Rect(lastRect.position.x, lastRect.yMax + 10, image.Tex.width, image.Tex.height),
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

        TriGrid maze = (TriGrid)target;

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