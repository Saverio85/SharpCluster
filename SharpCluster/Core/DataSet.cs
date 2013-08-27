using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace SharpCluster
{
    /// <summary>
    /// Create a list of Instances and handles basic operations on them
    /// </summary>
    public class DataSet:Collection<Instance>
    {
        /// <summary>
        /// Dimension of the instances 
        /// </summary>
        public int Dimension { get; private set; }
        /// <summary>
        /// Empty Constructor
        /// </summary>
        public DataSet() { }
        /// <summary>
        /// Constructor that adds the first Instance to the dataset right out of the bat
        /// </summary>
        /// <param name="i"> Instance to be added to the dataset</param>
        public DataSet(Instance i)
        {
            Add(i);
        }
        /// <summary>
        /// Adds an Instance to ElementList
        /// </summary>
        /// <param name="i">Instance to be added to ElementList</param>
        public new void Add(Instance i)
        {
            if (i == null)
            {
                return;
            }

            if ((Count > 0) && (i.Size() != Dimension))
            {
                string method = System.Reflection.MethodBase.GetCurrentMethod().Name;
                throw new System.ArgumentException(method + " :: Instances don't have all the same size", "lst");
            }

            if (Count == 0)
            {
                Dimension = i.Size();
            }

            base.Add(i);
        }
        /// <summary>
        /// Adds a list of Instnaces to the dataset
        /// </summary>
        /// <param name="lst">List of elements be added</param>
        public void AddAll(IEnumerable<Instance> lst)
        {
            foreach (Instance inst in lst)
            {
                Add(inst);
            }
        }
        /// <summary>
        /// Method to calculate the diamater of the dataset, that is the maximum distance
        /// between elements in it
        /// </summary>
        /// <param name="d"> Distance to be used in the calculation </param>
        /// <returns> The diameter of the dataset</returns>
        public double Diameter(IDistance d)
        {
            double max = 0;

            for (int i = 0; i < Count; i++)
            {
                for (int j = i + 1; j < Count; j++)
                {
                    double tmp = d.Distance(Items[i], Items[j]);
                    if (tmp > max)
                    {
                        max = tmp;
                    }
                }
            }
            return max;
        }
        /// <summary>
        /// Generate a new Random DataSet
        /// </summary>
        /// <param name="size">Number of instances in the dataset</param>
        /// <param name="instanceDimension">Dimension of the instances in the dataset</param>
        /// <returns> A new DataSet of size Random Instances</returns>
        public static DataSet RandomDataSet(int size, int instanceDimension = 2)
        {
            if (size <= 0)
            {
                string method = System.Reflection.MethodBase.GetCurrentMethod().Name;
                throw new System.ArgumentException(method + " :: The size of the random dataset must be greater than 0", "size");
            }

            if (instanceDimension <= 0)
            {
                string method = System.Reflection.MethodBase.GetCurrentMethod().Name;
                throw new System.ArgumentException(method + " :: The dimension of the instances must be greater than 0", "instanceDimension");
            }

            DataSet d = new DataSet();
            for (int i = 0; i < size; i++)
            {
                d.Add(Instance.RandomInstance(instanceDimension));
            }
            return d;
        }
        /// <summary>
        /// Transforms the dataset into a stirng containing the important details about it
        /// </summary>
        /// <returns>A string rapresenting the Dataset</returns>
        public override string ToString()
        {
            return "Dataset : {\r\n" + String.Join("\r\n", Items.Select(x => (x.ToString()))) + "\r\n}";
        }
        /// <summary>
        /// Merges two Dataset into one
        /// </summary>
        /// <param name="a">First Dataset</param>
        /// <param name="b">Second Dataset</param>
        /// <returns>Returns the result of merging Dataset a and Dataset b</returns>
        public static DataSet Merge(DataSet a, DataSet b)
        {
            DataSet tmp = new DataSet();
            a.ForEach(x => tmp.Add(x));
            b.ForEach(x => tmp.Add(x));
            return tmp;
        }
    }
}
