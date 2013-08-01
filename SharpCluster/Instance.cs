using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpCluster
{
    /// <summary>
    /// 
    /// </summary>
    public class Instance
    {
        /// <summary>
        /// A string containing the name of the Instance, to distinguish it from the other ones
        /// </summary>
        public string Name { get; private set;}
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
        public Instance(double[] data,string name = "")
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
        public static Instance RandomInstance(int n, double min = 0 , double max = 1)
        {
            if (n <= 0)
            {
                throw new System.ArgumentException("The dimension of the instance must be greater than 0", "instanceDimension");
            }

            double[] d = new double[n];
            Random rand = new Random();

            for (int i = 0; i < n; i++)
            {
                d[i] = min + rand.NextDouble() * (max - min);
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
                throw new System.ArgumentException("The dimension of the instance must be greater than 0", "instanceDimension");
            }

            double[] d = new double[n];
            Random rand = new Random();

            for (int i = 0; i < n; i++)
            {
                d[i] = min[i] + rand.NextDouble() * (max[i] - min[i]);
            }
            return new Instance(d);
        }
        /// <summary>
        /// Generate a string from the object
        /// </summary>
        /// <returns>A string rapresenting hte Instance</returns>
        public override string ToString()
        {
            return "{"+ Name + " , " + Data + "}";
        }

        public static Instance ZeroInstance(int n)
        {
            return new Instance(new double[n]);
        }

        public static  Instance operator +(Instance a, Instance b)
        {
            if(a.Size() != b.Size())
            {
                throw new System.ArgumentException("The two instances have different dimension","a,b");
            }

            double[] arr = new double[a.Size()];
            
            for (int i = 0; i < a.Size(); i++)
            {
                arr[i] = a.Data[i] + b.Data[i];
            }
            
            return new Instance(arr,a.Name + " ; " + b.Name);
        }

        public static Instance operator *(double b, Instance a)
        {
            if (b == 0)
            {
                throw new System.ArgumentException("The scalar can't be zero", "a,b");
            }

            double[] arr = new double[a.Size()];

            for (int i = 0; i < a.Size(); i++)
            {
                arr[i] = a.Data[i] / b;
            }

            return new Instance(arr, b + " * " + a.Name);
        }
    }
}
