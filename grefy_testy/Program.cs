using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace grefy_testy
{
    class Vertex
    {
        public int Id { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public Vertex(double x, double y, int id)
        {
            X = x;
            Y = y;
            Id = id;
        }
        public override string ToString()
        {
            return $"x: {X} y: {Y}";
        }
    }
    class Point
    {
        public int Id;
        public Point ConnectedPoint;
        public Point(int id)
        {
            Id = id;
        }
        public Point()
        {
        }
    }
    class Bucket
    {
        public int Id;
        public List<Point> Points = new List<Point>();
        //public int[] ConnectedBuckets;
        public List<Bucket> ConnectedBuckets = new List<Bucket>();
    }
    class Edge
    {
        public Vertex StartPoint { get; set; }
        public Vertex EndPoint { get; set; }
    }
    class Graph
    {
        public int NumberOfVertices { get; set; }
        public List<Vertex> ListOfVertices = new List<Vertex>();
        public List<Edge> ListOfEdges = new List<Edge>();
        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var item in ListOfVertices)
            {
                sb.AppendLine("Id:" + item.Id + " X: " + item.X + " Y: " + item.Y);
            }
            return sb.ToString();
        }
        static Random rnd = new Random();
        public static Graph GenerateGraph(int numOfVertices)
        {
            var graph = new Graph();
            graph.NumberOfVertices = numOfVertices;
            double x, y;
            for (int i = 0; i < numOfVertices; i++)
            {
                x = 100 * Math.Cos(2 * Math.PI * i / numOfVertices);
                y = 100 * Math.Sin(2 * Math.PI * i / numOfVertices);
                graph.ListOfVertices.Add(new Vertex(x, y, i));
            }
            return graph;
        }
        static int counter = 0;
        public int[,] GenerateMatrix(int degree)
        {
            int[,] matrix = new int[NumberOfVertices, NumberOfVertices];
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    if (counter >= degree)
                    {
                        matrix[i, j] = 0;
                    }
                    else
                    {
                        if (i == j)
                        {
                            matrix[i, j] = 0;
                        }
                        else
                        {
                            matrix[i, j] = 1;
                            counter += 1;
                        }
                    }
                }
                counter = 0;
            }
            return matrix;
        }
        public int[,] GenerateAdciedenceMatrix(int degree)
        {
            var middlepoint = NumberOfVertices / 2;
            int[,] matrix = new int[NumberOfVertices, NumberOfVertices];
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = i; j < matrix.GetLength(1); j++)
                {
                    if (i == j)
                    {
                        matrix[i, j] = 0;
                    }
                    else
                    {
                        if (j == (middlepoint + i) - 1 || j == (middlepoint + i) + 1)
                        {
                            matrix[i, j] = 1;
                        }
                        else
                        {
                            matrix[i, j] = 0;
                        }
                    }
                }
            }
            for (int i = 0; i < NumberOfVertices; i++)
            {
                for (int j = 0; j < i; j++)
                {
                    matrix[i, j] = matrix[j, i];
                }
            }
            return matrix;
        }
        public int[,] GenerateAjacenceMatrixBeta(int degree)
        {
            int[,] matrix = new int[NumberOfVertices, NumberOfVertices];
            int randomIndex = 0;
            //wypełnienie macierzy zerami
            for (int i = 0; i < NumberOfVertices; i++)
            {
                for (int j = 0; j < NumberOfVertices; j++)
                {
                    matrix[i, j] = 0;
                }
            }
            for (int i = 0; i < NumberOfVertices; i++)
            {
                for (int c = 0; c < degree; c++)
                {
                    randomIndex = rnd.Next(0, NumberOfVertices - 1);

                    matrix[i, randomIndex] = 1;

                }
            }
            return matrix;
        }
        static int index;
        static int pointToConnect;
        static IEnumerable<int> range;
        static bool checker = false;
        HashSet<int> excludeBucketIndex = new HashSet<int>();
        public void GenerateEdgesOfRegularGraph(int numOfVertices, int degree)
        {
            range = Enumerable.Range(0, degree);
            Bucket[] pointsContainer = new Bucket[numOfVertices];
            for (int i = 0; i < pointsContainer.Length; i++)
            {
                pointsContainer[i] = new Bucket();
                pointsContainer[i].Id = i;
            }

            for (int i = 0; i < pointsContainer.Length; i++)
            {
                for (int j = 0; j < degree; j++)
                {
                    pointsContainer[i].Points.Add(new Point((i * degree) + j));
                }
            }
            //kontenery z punktami zrobione
            foreach (var container in pointsContainer)
            {
                for (int i = 0; i < degree; i++)
                {
                    if (container.Points[i].ConnectedPoint != null)
                    {
                        Console.WriteLine("jest już połączony");
                        continue;
                    }
                    else
                    {
                        while (checker != true)
                        {
                            Console.WriteLine("sprawdzam czy koszyk nie zawiera");
                            excludeBucketIndex.Add(Array.IndexOf(pointsContainer, container));
                            range = Enumerable.Range(0, numOfVertices - 1).Where(x => !excludeBucketIndex.Contains(x));
                            index = rnd.Next(0, (numOfVertices - 1) - 1); //index losowego koszyka, różnego od obecnego
                            if (pointsContainer[index].ConnectedBuckets.Contains(container))
                            {
                                checker = false;
                            }
                            else
                            {
                                checker = true;
                                break;
                            }
                        }
                        while (checker != true)
                        {
                            pointToConnect = rnd.Next(pointsContainer[index].Points.Count);
                            if (pointsContainer[index].Points[pointToConnect].ConnectedPoint != null)
                            {
                                checker = false;
                            }
                            else
                            {
                                break;
                            }
                        }
                        container.Points[i].ConnectedPoint = pointsContainer[index].Points[pointToConnect];
                        pointsContainer[index].Points[pointToConnect] = container.Points[i].ConnectedPoint;
                        container.ConnectedBuckets.Add(pointsContainer[index]);
                        pointsContainer[index].ConnectedBuckets.Add(container);
                    }
                }
            }
            List<int> indexesOfConnectedVertices = new List<int>();
            foreach (var container in pointsContainer)
            {
                foreach (var bucket in container.ConnectedBuckets)
                {
                    indexesOfConnectedVertices.Add(bucket.Id);
                }
                //for (int i = 0; i < indexesOfConnectedVertices.Count; i++)
                //{
                //    ListOfEdges.Add(new Edge
                //    {
                //        StartPoint = ListOfVertices[container.Id],
                //        EndPoint = ListOfVertices[indexesOfConnectedVertices[i]]
                //    });
                //}
                foreach (var item in indexesOfConnectedVertices)
                {
                    ListOfEdges.Add(new Edge
                    {
                        StartPoint = ListOfVertices[container.Id],
                        EndPoint = ListOfVertices[item]
                    });
                }
                indexesOfConnectedVertices.Clear();
            }
        }
        public static void ShowMatrix(int[,] matrix)
        {
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    Console.Write(matrix[i, j] + " ");
                }
                Console.WriteLine();
            }
        }
        public void GenerateEdges(int[,] matrix)
        {
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    if (matrix[i, j] == 1)
                    {
                        ListOfEdges.Add(new Edge { StartPoint = ListOfVertices[i], EndPoint = ListOfVertices[j] });
                    }
                }
            }
        }
        public void ShowEdges()
        {
            int counter = 0;
            foreach (var item in ListOfEdges)
            {
                Console.WriteLine("Krawędź: {0}", counter);
                Console.WriteLine($"Start Point = {item.StartPoint}");
                Console.WriteLine($"End Point = {item.EndPoint}");
                counter++;
            }
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            var g1 = Graph.GenerateGraph(10);
            //Console.WriteLine(g1);
            //var g = g1.GenerateMatrix(5);
            //var g = g1.GenerateAjacenceMatrixBeta(3);
            //Graph.ShowMatrix(g);
            //Console.WriteLine();
            //g1.GenerateEdges(g);
            g1.GenerateEdgesOfRegularGraph(4, 2);
            g1.ShowEdges();
        }
    }
}
