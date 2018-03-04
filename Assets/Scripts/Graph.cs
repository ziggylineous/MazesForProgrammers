using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class Graph : ISerializationCallbackReceiver
{
    [NonSerialized]
    private List<int>[] vertexLinks;

    [SerializeField]
    private int[] serializedVertexLinks;

    [NonSerialized]
    public static System.Random Random = new System.Random();

    public Graph() : this(0)
    {
        
    }

    public Graph(int nodeCount)
    {
        Size = nodeCount;
    }

    public void LinkVertices(int v, int w)
    {
        if (!vertexLinks[v].Contains(w))
        {
            vertexLinks[v].Add(w);
            vertexLinks[w].Add(v);
        }
    }

    public bool AreLinked(int v, int w)
    {
        return vertexLinks[v].IndexOf(w) != -1;
    }

    public List<int> LinksOf(int vertex) { return vertexLinks[vertex]; }
    public int RandomLinkOf(int vertex)
    {
        if (HasAnyLink(vertex))
            return vertexLinks[vertex][Random.Next(vertexLinks[vertex].Count)];

        return -1;
    }

    public bool HasAnyLink(int vertex) { return vertexLinks[vertex].Count > 0; }

    public void ClearLinksOf(int vertex)
    {
        vertexLinks[vertex].Clear();
    }

    public void ClearLinks()
    {
        foreach (List<int> links in vertexLinks)
            links.Clear();
    }

    public int Size
    {
        get { return vertexLinks.Length; }
        set
        {
            vertexLinks = new List<int>[value];

            for (int i = 0; i != value; ++i)
                vertexLinks[i] = new List<int>();
        }
    }

    public int RandomVertex
    {
        get { return Random.Next(vertexLinks.Length); }
    }

    public int LinkCount
    {
        get
        {
            int linkCount = 0;
            Array.ForEach(vertexLinks, links => linkCount += links.Count);
            return linkCount;
        }
    }

    public void OnBeforeSerialize()
    {
        if (vertexLinks == null || vertexLinks.Length == 0)
        {
            //Debug.Log("Graph::OnBeforeSerialize: no grid => nothing to serialize");
            serializedVertexLinks = new int[0];
        }
        else
        {
//            Debug.Log("Graph::OnAfterDeserialize(): serializing links");

            serializedVertexLinks = new int[Size + LinkCount];
            int current = 0;

            for (int i = 0; i != Size; ++i)
            {
                serializedVertexLinks[current++] = vertexLinks[i].Count;

                foreach (int linked in vertexLinks[i])
                    serializedVertexLinks[current++] = linked;
            }

            Debug.Assert(current == Size + LinkCount);
        }
    }

    public void OnAfterDeserialize()
    {
        if (serializedVertexLinks != null && serializedVertexLinks.Length > 0)
        {
            Debug.LogFormat("Graph::OnAfterDeserialize(): deserializing links {0}", serializedVertexLinks.Length);
            int vertexCount = 0;

            for (int i = 0;
                 i < serializedVertexLinks.Length;
                 i += 1 + serializedVertexLinks[i])
            {
                ++vertexCount;
            }

            Size = vertexCount;

            int vertex = 0;

            for (int i = 0;
                 i < serializedVertexLinks.Length;
                 i += 1 + serializedVertexLinks[i])
            {
                int count = serializedVertexLinks[i];

                for (int j = 0; j != count; ++j)
                    vertexLinks[vertex].Add(serializedVertexLinks[i + 1 + j]);

                ++vertex;
            }
        }
        else
        {
            Debug.Log("Graph::OnAfterDeserialize(): no serialized links");
        }
    }
}