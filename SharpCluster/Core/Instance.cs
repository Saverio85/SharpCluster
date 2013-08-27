using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpCluster
{
    /// <summary>
    /// Class defining an Instance, that is the minimal element that we'll use for clustering.
    /// Can be described as an array of double values and a label, defined by the property Name.
    /// </summary>
    public class Instance
    {
        /// <summary>
        /// A string containing the name of the Instance, to distinguish it from the other ones
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// A double array that rapresent the instance in R^n
        /// </summary>
        public double[] Data { get; private set; }
        /// <summary>
        /// Constructor of the class
        /// </summary>
        /// <param name="data">The array rapresenting the instance</param>
        /// <param name="name">The optional name we wish to give to the instance. 
        /// By default, it's the empty string</param>
        public Instance(double[] data, string name = "")
        {
            Data = data;
            Name = name;
        }
        /// <summary>
        /// Method that returns the size of the double array rapresenting the instance
        /// </summary>
        /// <returns>The size of the double array rapresenting the instance</returns>
        public int Size()
        {
            return Data.Length;
        }
        /// <summary>
        /// Create a new Random Instance
        /// </summary>
        /// <param name="n">Dimension of the Instance</param>
        /// <param name="min">Min value for the Instance's items</param>
        /// <param name="max">Max value for the Instance's items</param>
        /// <returns> A new Intance with random values between min and max</returns>
        public static Instance RandomInstance(int n, double min = 0, double max = 1)
        {
            if (n <= 0)
            {
                string method = System.Reflection.MethodBase.GetCurrentMethod().Name;
                throw new System.ArgumentException(method + " :: The dimension of the instance must be greater than 0", "instanceDimension");
            }

            double[] d = new double[n];

            for (int i = 0; i < n; i++)
            {
                d[i] = min + Util.rand.NextDouble() * (max - min);
            }
            return new Instance(d);
        }
        /// <summary>
        /// Create a new Random Instance with a different min and max value for each index
        /// </summary>
        /// <param name="n">Dimension of the Instance</param>
        /// <param name="min">Min value for the Instance's items, different for each index</param>
        /// <param name="max">Max value for the Instance's items, different for each index</param>
        /// <returns> A new Intance with random values between min and max</returns>
        public static Instance RandomInstance(int n, double[] min, double[] max)
        {
            if (n <= 0)
            {
                string method = System.Reflection.MethodBase.GetCurrentMethod().Name;
                throw new System.ArgumentException(method + " :: The dimension of the instance must be greater than 0", "instanceDimension");
            }

            double[] d = new double[n];

            for (int i = 0; i < n; i++)
            {
                d[i] = min[i] + Util.rand.NextDouble() * (max[i] - min[i]);
            }

            //Console.WriteLine("MinValues : " + String.Join(" ", min));
            //Console.WriteLine("MaxValues : " + String.Join(" ", max));
            //Console.WriteLine("dGeneratedValues : " + String.Join(" ", d));
            return new Instance(d);
        }
        /// <summary>
        /// Generate a string from the object
        /// </summary>
        /// <returns>A string rapresenting hte Instance</returns>
        public override string ToString()
        {
            string tmp = "{ " + (String.IsNullOrEmpty(Name) ? "anon" : Name) + " { ";
            for (int i = 0; i < Data.Length; i++)
            {
                tmp += String.Format("{0:0.00}", Data[i]) + ";";
            }
            return tmp + "} };";
        }
        /// <summary>
        /// Method to get an exact copy of the instance
        /// </summary>
        /// <returns>Returns an instance having the same Data and Name values as the current one</returns>
        public Instance Clone()
        {
            return new Instance((double[])Data.Clone(), Name);
        }
        /// <summary>
        /// Generate a new Instance containing only zeroes in Data
        /// </summary>
        /// <param name="n">Dimension of the Instnace</param>
        /// <returns>Returns a new Instance, with no name and with a double array of zeroes as Data</returns>
        public static Instance ZeroInstance(int n)
        {
            return new Instance(new double[n]);
        }
        /// <summary>
        /// Defines the sum between two instances as the sum element by element of the two Data Arrays
        /// </summary>
        /// <param name="a">First Addend</param>
        /// <param name="b">Second Addend</param>
        /// <returns>Returns a new isntance, with Data defined as the element per element sum of
        /// a's and b's Data</returns>
        public static Instance operator +(Instance a, Instance b)
        {
            if (a.Size() != b.Size())
            {
                string method = System.Reflection.MethodBase.GetCurrentMethod().Name;
                throw new System.ArgumentException(method + " :: The two instances have different dimension", "a,b");
            }

            double[] arr = new double[a.Size()];

            for (int i = 0; i < a.Size(); i++)
            {
                arr[i] = a.Data[i] + b.Data[i];
            }

            return new Instance(arr, a.Name);
        }
        /// <summary>
        /// Definies the product between a real number and an instance
        /// </summary>
        /// <param name="b">The real number we want to multiply the instance for</param>
        /// <param name="a">The Instance to be multiplied</param>
        /// <returns> Returns the product between the scalar b and the Instance a, defined
        /// as the array having as i_th element b * a[i]</returns>
        public static Instance operator *(double b, Instance a)
        {
            if (b == 0)
            {
                string method = System.Reflection.MethodBase.GetCurrentMethod().Name;
                throw new System.ArgumentException(method + " :: The scalar can't be zero", "a,b");
            }

            double[] arr = new double[a.Size()];

            for (int i = 0; i < a.Size(); i++)
            {
                arr[i] = a.Data[i] * b;
            }

            return new Instance(arr, a.Name);
        }

    }
}
