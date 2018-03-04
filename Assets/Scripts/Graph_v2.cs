using System;
using UnityEngine;
using System.Collections.Generic;

namespace v2
{
    [Serializable]
    public class Graph<T>
    {
        private List<int>[] vertexLinks;
		[SerializeField]
		private int[] serializedVertexLinks;

        [SerializeField]
		private T[] vertexData;

        [NonSerialized]
        public static System.Random Random = new System.Random();


        public Graph() : this(0)
        {

        }

        public Graph(int nodeCount)
        {
            Size = nodeCount;
        }

		public T this[int v]
        {
            get { return vertexData[v]; }
            set { vertexData[v] = value; }
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
	}
}
