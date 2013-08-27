using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections;
using System.Reflection;
using System.Diagnostics;

namespace SharpCluster
{
    /// <summary>
    /// Class collecting utility method, extension methods, constants and so on.
    /// </summary>
    public static class Util
    {
        /// <summary>
        /// Array of Colors
        /// </summary>
        public static string[] colors = { "255 0 0", "0 255 0", "0 0 255", "255 255 0", "0 255 255", "255 0 255" };
        /// <summary>
        /// Rand Instance, to prevent getting the same random number over and over if generated too close
        /// </summary>
        public static readonly Random rand = new Random();

        /// <summary>
        /// Create a new Dataset Uniformely distributed inside a rectangle
        /// </summary>
        /// <param name="n">Number of elements in the result dataset</param>
        /// <param name="center">Center of the dataset</param>
        /// <param name="radius">Distance from the center</param>
        /// <returns>A List of Instances randomply distributed inside a rectangle</returns>
        public static List<Instance> RandomRectSet(int n, Instance center, double radius)
        {
            List<Instance> lst = new List<Instance>();
            for (int i = 0; i < n; i++)
            {
                double[] d = (double[])center.Data.Clone();
                for (int j = 0; j < d.Length; j++)
                {
                    d[j] += Math.Pow(-1, rand.Next(1, 3)) * rand.NextDouble() * radius;
                }
                lst.Add(new Instance(d));
            }
            return lst;
        }
        /// <summary>
        /// Plot the clustering result in a ppm image
        /// </summary>
        /// <param name="sol">List of clusters (as Datasets) we want to plot</param>
        /// <param name="colors">Colors to be used for each dataset</param>
        /// <param name="filename">Name and path of the ppm file</param>
        public static void MakePPM(List<DataSet> sol, string[] colors, String filename)
        {
            if (sol.Count > colors.Length)
            {
                string method = System.Reflection.MethodBase.GetCurrentMethod().Name;
                throw new System.ArgumentException(method + " :: Sol and colors must have the same number of elements");
            }

            if (String.IsNullOrEmpty(filename))
            {
                string method = System.Reflection.MethodBase.GetCurrentMethod().Name;
                throw new System.ArgumentException(method + " :: The file name can't be an empty string");
            }

            int n = 100;
            StreamWriter outfile = new StreamWriter(filename);
            outfile.WriteLine("P3");
            outfile.WriteLine(n + " " + n);
            outfile.WriteLine("255");

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    string color = "255 255 255";
                    for (int k = 0; k < sol.Count; k++)
                    {
                        for (int t = 0; t < sol[k].Count; t++)
                        {
                            double[] d = sol[k][t].Data;
                            if (((int)(d[0] * 100) == i) && ((int)(d[1] * 100) == j))
                            {
                                color = colors[k];
                            }
                        }
                    }
                    outfile.WriteLine(color);
                }
            }
            outfile.Close();
            Console.WriteLine("Done Writing " + filename);
        }

        /// <summary>
        /// Extension method that allows the ForEach LINQ statement to be used
        /// on every IEnumerable
        /// </summary>
        /// <typeparam name="T">Type of the IEnumerable Generic</typeparam>
        /// <param name="elements">IEnumrable of wich we want every element to be processed by act</param>
        /// <param name="act">Action to be performed on every element of elements</param>
        public static void ForEach<T>(this IEnumerable<T> elements, Action<T> act)
        {
            foreach (var element in elements)
            {
                act(element);
            }
        }

        /// <summary>
        /// Extension method to print a double matrix on the console, used for debugging
        /// porpuses
        /// </summary>
        /// <param name="a">The double matrix we want to print</param>
        /// <param name="format">Format the doubles must assume</param>
        public static void Print(this double[,] a, string format = "{0:0.000}")
        {
            for (int i = 0; i < a.GetLength(0); i++)
            {
                for (int j = 0; j < a.GetLength(1); j++)
                {
                    Console.Write("\t " + String.Format(format,a[i, j]));
                }
                Console.WriteLine();
            }
        }
        /// <summary>
        /// Extension method to print a List on the console, used for debugging
        /// porpuses
        /// </summary>
        /// <typeparam name="T">Type of elements in the list</typeparam>
        /// <param name="lst">List we want to print</param>
        public static void Print<T>(this List<T> lst)
        {
            Debug.WriteLine("****");
            foreach (T element in lst)
            {
                Debug.WriteLine(element.ToString());
            }
        }

        /// <summary>
        /// Extension method that finds the minimum value in a multidimensional array, 
        /// and returns the index of his location
        /// </summary>
        /// <param name="arr">Array will be serched for the minimum index</param>
        /// <returns> The location of the minimum value in the array</returns>
        public static int[] MinIndex(this Array arr)
        {
            int[] solution = new int[arr.Rank];
            var minValue = arr.GetValue(arr.LowerBounds());

            foreach (var i in arr.EnumerateIndex())
            {
                if (Comparer.Default.Compare(arr.GetValue(i), minValue) < 0)
                {
                    minValue = arr.GetValue(i);
                    Array.Copy(i, solution, i.Length);
                }
            }
            return solution;
        }
        /// <summary>
        /// Extension method that finds the Lower Bound in a multidimensional array
        /// </summary>
        /// <param name="arr">Array of which we need the lower bound array</param>
        /// <returns>An integer array, each one containing the minimum index of the given dimension</returns>
        public static int[] LowerBounds(this Array arr)
        {
            return Enumerable.Range(0, arr.Rank)
                .Select(x => arr.GetLowerBound(x))
                .ToArray();
        }
        /// <summary>
        /// Extension method that finds the Upper Bound in a multidimensional array
        /// </summary>
        /// <param name="arr">Array of which we need the upper bound array</param>
        /// <returns>An integer array, each one containing the maximum index of the given dimension</returns>
        public static int[] UpperBounds(this Array arr)
        {
            return Enumerable.Range(0, arr.Rank)
                .Select(x => arr.GetUpperBound(x))
                .ToArray();
        }
        /// <summary>
        /// Extension method that makes the indexes of a multidimensional array IEnumerable
        /// </summary>
        /// <param name="arr">Array of which we need the indexes</param>
        /// <returns>Return an IEnumerable instance that list all the indexes of the array</returns>
        public static IEnumerable<int[]> EnumerateIndex(this Array arr)
        {
            int[] lowerBound = arr.LowerBounds();
            int[] upperBound = arr.UpperBounds();

            int[] index = new int[arr.Rank];
            Array.Copy(lowerBound,index,lowerBound.Length);

            yield return index;
            
            while (true)
            {
                for (int i = 0; i < arr.Rank; i++)
                {
                    index[i]++;
                    if (index[i] <= upperBound[i])
                    {
                        yield return index;
                        break;
                    }
                    else
                    {
                        index[i] = lowerBound[i];
                        if (i == arr.Rank - 1)
                        {
                            yield break;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Extension method that returns the requested row of a multidimensional array
        /// </summary>
        /// <typeparam name="T">Type of the elements in the array</typeparam>
        /// <param name="arr">Array we need to process</param>
        /// <param name="n">Index of the Row we want to get</param>
        /// <returns>Returns an array of T, rapresenting the n_th row</returns>
        public static T[] GetRow<T>(this T[,] arr, int n)
        {
            if ((n < 0) || (n > arr.GetLength(0)))
            {
                string method = System.Reflection.MethodBase.GetCurrentMethod().Name;
                throw new System.ArgumentException(method + " :: n must be smaller than the first dimension of the array", "n");
            }

            T[] sol = new T[arr.GetLength(0)];
            for (int i = 0; i < arr.GetLength(1); i++)
            {
                sol[i] = arr[n, i];
            }
            return sol;
        }
    }
}