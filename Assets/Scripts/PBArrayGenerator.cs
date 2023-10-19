using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class PBArrayGenerator
{
    public class Edge
    {
        public int source, dest, weight;
    }

    private static int MinKey(int[] key, bool[] set, int verticesCount)
    {
        int min = int.MaxValue, minIndex = 0;

        for (int v = 0; v < verticesCount; ++v)
        {
            if (set[v] == false && key[v] < min)
            {
                min = key[v];
                minIndex = v;
            }
        }

        return minIndex;
    }

    /*    private static void Print(int[] parent, int[,] graph, int verticesCount)
        {
            Console.WriteLine("Edge     Weight");
            for (int i = 1; i < verticesCount; ++i)
                Console.WriteLine("{0} - {1}    {2}", parent[i], i, graph[i, parent[i]]);
        }*/

    private static Edge[] Prim(int[,] graph, int verticesCount)
    {
        int[] parent = new int[verticesCount];
        int[] key = new int[verticesCount];
        bool[] mstSet = new bool[verticesCount];

        for (int i = 0; i < verticesCount; ++i)
        {
            key[i] = int.MaxValue;
            mstSet[i] = false;
        }

        key[0] = 0;
        parent[0] = -1;

        for (int count = 0; count < verticesCount - 1; ++count)
        {
            int u = MinKey(key, mstSet, verticesCount);
            mstSet[u] = true;

            for (int v = 0; v < verticesCount; ++v)
            {
                if (Convert.ToBoolean(graph[u, v]) && mstSet[v] == false && graph[u, v] < key[v])
                {
                    parent[v] = u;
                    key[v] = graph[u, v];
                }
            }
        }
        Edge[] edges = new Edge[verticesCount - 1];
        for (int i = 0; i < verticesCount - 1; i++)
        {
            edges[i] = new Edge();
            edges[i].source = parent[i + 1];
            edges[i].dest = i + 1;
            edges[i].weight = graph[i + 1, parent[i + 1]];
        }
        return edges;

    }
    private static void Swap<T>(ref T a, ref T b)
    {
        T c = a;
        a = b;
        b = c;
    }


    private static List<(int, int)> BresenhamLine(int x0, int y0, int x1, int y1)
    {
        // Optimization: it would be preferable to calculate in
        // advance the size of "result" and to use a fixed-size array
        // instead of a list.
        List<(int, int)> result = new List<(int, int)>();

        bool steep = Math.Abs(y1 - y0) > Math.Abs(x1 - x0);
        if (steep)
        {
            Swap(ref x0, ref y0);
            Swap(ref x1, ref y1);
        }
        if (x0 > x1)
        {
            Swap(ref x0, ref x1);
            Swap(ref y0, ref y1);
        }

        int deltax = x1 - x0;
        int deltay = Math.Abs(y1 - y0);
        int error = 0;
        int ystep;
        int y = y0;
        if (y0 < y1) ystep = 1; else ystep = -1;
        for (int x = x0; x <= x1; x++)
        {
            if (steep) result.Add(new(y, x));
            else result.Add(new(x, y));
            error += deltay;
            if (2 * error >= deltax)
            {
                y += ystep;
                error -= deltax;
            }
        }

        return result;
    }

    private static bool[,] GrowLineIterative(bool[,] bools, List<(int, int)> BPixels, int iterations)
    {
        if (iterations == 0) return null;
        var res = new bool[bools.GetLength(0), bools.GetLength(1)];
        var gl = GrowLine(bools, BPixels);
        res = Draw(res, gl);
        for (int i = 0; i < iterations; i++)
        {
            gl = GrowLine(res, BPixels);
            res = Draw(res, gl);
        }
        return res;
    }
    private static List<(int, int)> GrowLine(bool[,] bools, List<(int, int)> BPixels)
    {
        List<(int, int)> result = new List<(int, int)>();


        for (int x = 0; x < bools.GetLength(0); x++)
        {
            for (int y = 0; y < bools.GetLength(1); y++)
            {
                int numberOfNeighbours = 0;
                if (BPixels.Contains((x - 1, y)))
                {
                    numberOfNeighbours++;
                }
                if (BPixels.Contains((x, y - 1)))
                {
                    numberOfNeighbours++;
                }
                if (BPixels.Contains((x + 1, y)))
                {
                    numberOfNeighbours++;
                }
                if (BPixels.Contains((x, y + 1)))
                {
                    numberOfNeighbours++;
                }
                if (numberOfNeighbours > 1)
                {
                    result.Add((x, y));
                }
            }
        }

        return result;
    }

    private static bool[,] Draw(bool[,] bools, List<(int, int)> values)
    {
        for (int x = 0; x < bools.GetLength(0); x++)
        {
            for (int y = 0; y < bools.GetLength(1); y++)
            {
                if (values.Contains((x, y)))
                {
                    bools[x, y] = true;
                }
            }
        }
        return bools;
    } private static bool[,] Draw(bool[,] bools, bool[,] values)
    {
        for (int x = 0; x < bools.GetLength(0); x++)
        {
            for (int y = 0; y < bools.GetLength(1); y++)
            {
                if (values[x,y])
                {
                    bools[x, y] = true;
                }
            }
        }
        return bools;
    }

    /*//secound val is vertices count
    public (int[,], int) arrayToGraph(bool[,] bools)
    {

        int[,] graph = new int[bools.Length, bools.Length];

        List<(int, int)> vertices = new();
        for (int x = 0; x < bools.GetLength(0); x++)
        {
            for (int y = 0; y < bools.GetLength(1); y++)
            {

                if (bools[x, y])
                {
                    vertices.Add((x, y));
                }
            }
        }



        for (int i = 0; i < graph.GetLength(0); i++)
        {
            for (int j = 0; j < graph.GetLength(1); j++)
            {*//*
                Debug.Log(i + " " + j);
                Debug.Log((j / bools.GetLength(0)) + " -- " + (j % bools.GetLength(0)));
                //*//*
                if (bools[j / (bools.GetLength(0)), j % (bools.GetLength(0))])
                {
                    foreach (var vertice in vertices)
                    {
                        //
                        int distance = (int)Math.Sqrt(Math.Pow(j / bools.GetLength(0) - vertice.Item1, 2) + Math.Pow(j % bools.GetLength(0) - vertice.Item2, 2));
                        graph[vertice.Item1 + vertice.Item2, j] = distance;
                        graph[i, vertice.Item1 + vertice.Item2] = distance;
                    }
                }
                *//* else
                 {
                     graph[i, j] = 0;
                     graph[j, i] = 0;
                 }*//*
            }
        }
        return (graph, vertices.Count);
    }*/

    public (int[,], List<(int, int)>) arrayToGraph(bool[,] bools)
    {


        List<(int, int)> vertices = new();
        for (int x = 0; x < bools.GetLength(0); x++)
        {
            for (int y = 0; y < bools.GetLength(1); y++)
            {

                if (bools[x, y])
                {
                    vertices.Add((x, y));
                }
            }
        }
        int[,] graph = new int[vertices.Count, vertices.Count];

        for (int i = 0; i < graph.GetLength(0); i++)
        {
            for (int j = 0; j < graph.GetLength(1); j++)
            {
                foreach (var vertice in vertices)
                {
                    if (vertice.Item1 != i && vertice.Item2 != j)
                    {
                        int distance = (int)Math.Sqrt(Math.Pow(j / bools.GetLength(0) - vertice.Item1, 2) + Math.Pow(j % bools.GetLength(0) - vertice.Item2, 2));
                        if (vertices.IndexOf(vertice) != -1)
                        {
                            graph[vertices.IndexOf(vertice), j] = distance;
                            graph[i, vertices.IndexOf(vertice)] = distance;
                        }
                    }

                }
            }
        }
        return (graph, vertices);
    }

    public static void ConvertBoolArrayToImage(bool[,] boolArray, string imageName)
    {
        if (boolArray != null)
        {
            int width = boolArray.GetLength(0);
            int height = boolArray.GetLength(1);

            // Create a new Texture2D to represent the image
            Texture2D texture = new Texture2D(width, height);

            // Loop through the bool array and set pixel colors
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Color color = boolArray[x, y] ? Color.black : Color.white;
                    texture.SetPixel(x, y, color);
                }
            }

            // Apply changes to the texture
            texture.Apply();

            // Encode the texture as a PNG and save it to the Assets folder
            byte[] bytes = texture.EncodeToPNG();
            string outputPath = Application.dataPath + "/" + imageName;
            System.IO.File.WriteAllBytes(outputPath, bytes);

            Debug.Log("Image saved to " + outputPath);
        }
        else
        {
            Debug.LogError("boolArray is null. Please assign a bool array.");
        }
    }

    public List<(int, int)> PBLines(int[,] graph, List<(int,int)> vertices)
    {
        Edge[] edges = Prim(graph, vertices.Count);
        List<(int, int)> BPixels = new List<(int, int)>();

        foreach (Edge edge in edges)
        {
            var start = vertices[edge.source];
            var end = vertices[edge.dest];
            List<(int, int)> line = BresenhamLine(start.Item1, start.Item2, end.Item1, end.Item2);
            foreach ((int, int) pixel in line)
            {
                BPixels.Add(pixel);
            }
        }
        return BPixels;
    }
    public List<(int, int)> PBLines(int[,] graph, List<(int, int)> vertices, (int, int) shift)
    {
        Edge[] edges = Prim(graph, vertices.Count);
        List<(int, int)> BPixels = new List<(int, int)>();

        foreach (Edge edge in edges)
        {
            var start = vertices[edge.source];
            var end = vertices[edge.dest];
            List<(int, int)> line = BresenhamLine(start.Item1 + shift.Item1, start.Item2 + shift.Item2, end.Item1 + shift.Item1, end.Item2 + shift.Item2);
            foreach ((int, int) pixel in line)
            {
                BPixels.Add(pixel);
            }
        }
        return BPixels;
    }


    public bool[,] ConnectTheDots(bool[,] bools, int radius)
    {


        var graph = arrayToGraph(bools);
        //Debug.Log(graph.Item1.ToString());
        var lines = PBLines(graph.Item1, graph.Item2);
        lines.AddRange(PBLines(graph.Item1, graph.Item2, (1, 0)));
        lines.AddRange(PBLines(graph.Item1, graph.Item2, (0, 1)));
        lines.AddRange(PBLines(graph.Item1, graph.Item2, (-1, 0)));
        lines.AddRange(PBLines(graph.Item1, graph.Item2, (0, 1)));
        ConvertBoolArrayToImage(bools, "bools1.png");
        
        bools = Draw(bools, lines);
        ConvertBoolArrayToImage(bools, "bools2.png");
        
        var thickness = GrowLineIterative(bools, lines, radius);
        ConvertBoolArrayToImage(thickness, "thickness.png");
        bools = Draw(bools, thickness);
        ConvertBoolArrayToImage(bools, "bools3.png");
        Debug.Log(graph.Item2);

        return bools;
    }

}
