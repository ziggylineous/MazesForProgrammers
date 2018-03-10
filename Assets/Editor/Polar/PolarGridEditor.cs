using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PolarGrid))]
public class PolarGridEditor : Editor
{
    private int prevRowCount;

    private int prevBuilderIndex;
    private string[] builderNames;
    private VertexLinker[] builders;

    private PolarGridImage image;


    public void Awake()
    {
        Debug.Log("PolarGridEditor::Awake()");

        builderNames = new string[]{
            "Random Walk",
            "Hunt and Kill",
            "Recursive Backtracker"
        };

        builders = new VertexLinker[] {
            new RandomWalkVertexLinker(),
            new HuntAndKillVertexLinker(),
            new RecursiveBacktrackerVertexLinker()
        };

        prevBuilderIndex = 0;

        image = new PolarGridImage(20, Color.white, Color.blue);
    }

    public void OnEnable()
    {
        Debug.Log("PolarGridEditor::OnEnable()");

        SerializedProperty rowCountSerialized = serializedObject.FindProperty("rowCount");
        prevRowCount = rowCountSerialized.intValue;

        if (prevRowCount > 0)
            image.Draw((PolarGrid)serializedObject.targetObject);
    }

    public override void OnInspectorGUI()
    {
        PolarGrid polarGrid = (PolarGrid) target;

        int newRowCount = EditorGUILayout.IntField("row count", polarGrid.RowCount);
        EditorGUILayout.LabelField("cell count", polarGrid.Graph.Size.ToString());

        if (newRowCount != prevRowCount && newRowCount > 0)
        {
            prevRowCount = newRowCount;
            ChangeSize(newRowCount);
        }

        if (prevRowCount > 0)
        {
			int builderIndex = EditorGUILayout.Popup(prevBuilderIndex, builderNames);
			if (builderIndex != prevBuilderIndex || GUILayout.Button("rebuild"))
			{
				prevBuilderIndex = builderIndex;
				RebuildMaze(builders[builderIndex]);
			}
			
			DisplayImage();
            
        }
    }

    private void ChangeSize(int newRowCount)
    {
        PolarGrid polarGrid = (PolarGrid)target;

        polarGrid.RowCount = newRowCount;

        EditorUtility.SetDirty(target);

        image.Draw(polarGrid);
    }

    private void DisplayImage()
    {
        int newCellHeight = EditorGUILayout.IntField("Cell height", image.CellHeight);

        if (newCellHeight != image.CellHeight)
        {
            image.CellHeight = newCellHeight;
            image.Draw((PolarGrid)target);
        }

        if (prevRowCount > 0)
        {
            Rect lastRect = GUILayoutUtility.GetLastRect();

            EditorGUI.DrawPreviewTexture(
                new Rect(lastRect.position.x, lastRect.yMax + 10, image.Tex.width, image.Tex.height),
                image.Tex
            );
        }

        if (GUILayout.Button("Save Image"))
        {
            image.Tex.Save("polarGrid_" + builderNames[prevBuilderIndex]);
        }
    }

    private void RebuildMaze(VertexLinker builder)
    {
        Debug.Log("rebuilding " + builder.ToString());

        PolarGrid maze = (PolarGrid)target;

        maze.Graph.ClearLinks();
        builder.Build(new VertexLinkerHelper(maze.Graph, maze.AdjacentGraph));
        EditorUtility.SetDirty(maze);

        BreadthFirst bf = new BreadthFirst(maze.Graph, 0);
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