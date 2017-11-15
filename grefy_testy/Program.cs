using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        public bool IsConnected;
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
        public bool GenerateEdgesOfRegularGraph(int numOfVertices, int degree, Graph g)
        {
            bool checkStop;
            bool checkFree;
            int numberOfEdges = (numOfVertices * degree) / 2;
            range = Enumerable.Range(0, degree);
            Bucket[] pointsContainers = new Bucket[numOfVertices];
            for (int i = 0; i < pointsContainers.Length; i++)
            {
                pointsContainers[i] = new Bucket();
                pointsContainers[i].Id = i;
            }

            for (int i = 0; i < pointsContainers.Length; i++)
            {
                for (int j = 0; j < degree; j++)
                {
                    pointsContainers[i].Points.Add(new Point((i * degree) + j));
                }
            }
            //kontenery z punktami zrobione
            foreach (var container in pointsContainers)
            {
                Console.WriteLine("Kontener: {0}", container.Id);
                for (int i = 0; i < degree; i++)
                {
                    if (container.Points[i].IsConnected == true)
                    {
                        Console.WriteLine("Punkt {0} w koszyku {1} jest już połączony", container.Points[i].Id, Array.IndexOf(pointsContainers, container));
                        continue;
                    }
                    else
                    {
                        while (checker != true)
                        {
                            Console.WriteLine("Punkt {0} w koszyku {1} nie jest połączony", container.Points[i].Id, Array.IndexOf(pointsContainers, container));
                            //Console.ReadKey();
                            checkFree = CheckFreeVertices(pointsContainers, degree);
                            if (checkFree == true)
                            {
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine("Są koszyki z którymi się można połączyć");
                                Console.ResetColor();
                            }
                            else
                            {
                                if (pointsContainers.Any(x => x.ConnectedBuckets.Count < degree))
                                {
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.WriteLine("Nie ma koszyków z którymi się można połączyć");
                                    Console.ResetColor();
                                    Console.ForegroundColor = ConsoleColor.Blue;
                                    Console.WriteLine("Wywołuję metodę od początku, kliknij coś");
                                    Console.ResetColor();

                                    Console.ReadKey();
                                    checker = true;
                                    return false;

                                }

                            }
                            excludeBucketIndex.Add(Array.IndexOf(pointsContainers, container));
                            //range = Enumerable.Range(0, numOfVertices - 1).Where(x => !excludeBucketIndex.Contains(x));
                            index = rnd.Next(0, numOfVertices); //index losowego koszyka, różnego od obecnego

                            Console.WriteLine("wylosowany index kontenera: {0}", index);
                            if (index == container.Id)
                            {
                                Console.WriteLine("wylosowano taki sam kontener jak obecny");
                                checker = false;
                            }
                            else if (pointsContainers[index].Points.All(x => x.IsConnected == true))
                            {
                                Console.WriteLine("wszystkie punkty w wylosowanym kontenerze są już połączone");
                            }
                            else
                            {
                                if (pointsContainers[index].ConnectedBuckets.Contains(container))
                                {
                                    checker = false;
                                }
                                else
                                {
                                    checker = true;
                                }
                            }
                        }
                        checker = false;
                        while (checker != true)
                        {
                            Console.WriteLine("Szukam punktu do połączenia");
                            pointToConnect = rnd.Next(pointsContainers[index].Points.Count);
                            Console.WriteLine("wylosowany punkt do połączenia: {0}", pointsContainers[index].Points[pointToConnect].Id);
                            if (pointsContainers[index].Points[pointToConnect].IsConnected == true)
                            {
                                Console.WriteLine("Wylosowany punkt jest już połączony");
                                checker = false;
                            }
                            else
                            {
                                container.Points[i].IsConnected = true;
                                pointsContainers[index].Points[pointToConnect].IsConnected = true;
                                if (pointsContainers[index].Points[pointToConnect].ConnectedPoint != null)
                                {
                                    checker = false;
                                }
                                else
                                {
                                    checker = true;
                                }
                            }
                        }
                        checker = false;
                        container.Points[i].ConnectedPoint = pointsContainers[index].Points[pointToConnect];
                        pointsContainers[index].Points[pointToConnect] = container.Points[i].ConnectedPoint;
                        container.ConnectedBuckets.Add(pointsContainers[index]);
                        pointsContainers[index].ConnectedBuckets.Add(container);
                        //dopisać metody: 
                        //                2. poprawić generowanie krawędzi - źle działa (niepoprawna ilość)
                    }
                }
            }
            //List<int> indexesOfConnectedVertices = new List<int>();
            //foreach (var container in pointsContainers)
            //{
            //    foreach (var bucket in container.ConnectedBuckets)
            //    {
            //        indexesOfConnectedVertices.Add(bucket.Id);
            //    }
            //    foreach (var item in indexesOfConnectedVertices)
            //    {
            //        try
            //        {
            //            ListOfEdges.Add(new Edge
            //            {
            //                StartPoint = ListOfVertices[container.Id],
            //                EndPoint = ListOfVertices[item]
            //            });
            //        }
            //        catch (Exception e)
            //        {

            //        }
            //    }
            //    indexesOfConnectedVertices.Clear();
            //}
            foreach (var container in pointsContainers)
            {
                for (int i = 0; i < container.ConnectedBuckets.Count; i++)
                {
                    ListOfEdges.Add(new Edge
                    {
                        StartPoint = ListOfVertices[Array.IndexOf(pointsContainers, container)],
                        EndPoint = ListOfVertices[Array.IndexOf(pointsContainers, container.ConnectedBuckets[i])]
                    });
                    var bucketToDelete = container.ConnectedBuckets[i].ConnectedBuckets.Where(x => x.Id == container.Id).First();
                    container.ConnectedBuckets[i].ConnectedBuckets.Remove(bucketToDelete);
                    Console.WriteLine("Bucket has been removed");
                }
            }
            return true;
        }
        public static bool CheckFreeVertices(Bucket[] bucketCollection, int degree)
        {
            int counter = 0;
            int degreeCounter = 0;
            bool checker = false;
            List<Bucket> bucketsWithoutMaximalDegree = new List<Bucket>();
            foreach (var bucket in bucketCollection)
            {
                if (bucket.ConnectedBuckets.Count == degree)
                {
                    counter++;
                }
                else
                {
                    degreeCounter++;
                    bucketsWithoutMaximalDegree.Add(bucket);
                }
            }
            if (bucketsWithoutMaximalDegree.Count == 2)
            {
                if (bucketsWithoutMaximalDegree[0].ConnectedBuckets.Contains(bucketsWithoutMaximalDegree[1]) && bucketsWithoutMaximalDegree[1].ConnectedBuckets.Contains(bucketsWithoutMaximalDegree[0]))
                {
                    checker = false;
                    Console.WriteLine("KLESZCZE - WSZYTSKIE MINIMUM 2 KRAWĘDZIE I NIE DA SIĘ DALEJ IŚĆ");
                    Console.ReadKey();
                    return false;
                }
            }
            if (counter == bucketCollection.Length - 1)
            {
                return false;
            }
            else if (counter == degree - 2 && checker == true)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("Zagłodzenie - resetuję (naciśnij coś)");
                Console.ResetColor();
                Console.ReadKey();
                bucketsWithoutMaximalDegree.Clear();
                return false;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("Liczba pełnych wierzchołków: {0}", counter);
                //zwrócić Tupla zawierającego boola i counter, jeśli counter == degree-2 - od poczatku
                Console.ResetColor();
                bucketsWithoutMaximalDegree.Clear();
                return true;
            }
        }
        public static bool CheckIfThereIsConnection(Bucket b1, Bucket b2)
        {
            if (b1.ConnectedBuckets.Contains(b2) || b2.ConnectedBuckets.Contains(b1))
            {
                return true;
            }
            else
            {
                return false;
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
            var sw = new Stopwatch();
            var g1 = Graph.GenerateGraph(10);
            bool isDrawn = false;
            //Console.WriteLine(g1);
            //var g = g1.GenerateMatrix(5);
            //var g = g1.GenerateAjacenceMatrixBeta(3);
            //Graph.ShowMatrix(g);
            //Console.WriteLine();
            //g1.GenerateEdges(g);
            sw.Start();
            long s = 0;
            while (isDrawn == false)
            {
                isDrawn = g1.GenerateEdgesOfRegularGraph(8, 4, g1);
                sw.Stop();
                s = sw.ElapsedMilliseconds;
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Time: {0}", s);
                Console.ResetColor();
                if(s > 10000)
                {
                    Console.WriteLine("Wywalam pętlę");
                    Console.ReadKey();
                    break;
                }
                sw.Start();
            }
            Console.WriteLine(g1.ListOfEdges.Count());
            Console.ReadKey(); 
            g1.ShowEdges();
            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds);
        }
    }
}
