using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpCluster
{
    /// <summary>
    /// Create a list of Instances and handles basic operations on them
    /// </summary>
    public class DataSet
    {
        /// <summary>
        /// List of the instances contained in the Dataset
        /// </summary>
        public List<Instance> ElementList { get; private set; }
        /// <summary>
        /// Dimension of the instances 
        /// </summary>
        public int Dimension { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public int Count { 
            get { 
                return (ElementList == null) ? 0 : ElementList.Count;
            } 
        }
        /// <summary>
        /// Empty Constructor, initiate the ElementList Property to an empty list
        /// </summary>
        public DataSet()
        {
            ElementList = new List<Instance>();
        }
        /// <summary>
        /// Constructor that adds the first Instance to the dataset right out of the bat
        /// </summary>
        /// <param name="i"> Instance to be added to the dataset</param>
        public DataSet(Instance i)
        {
            ElementList = new List<Instance>();
            ElementList.Add(i);
        }
        /// <summary>
        /// Adds an Instance to ElementList
        /// </summary>
        /// <param name="i">Instance to be added to ElementList</param>
        public void Add(Instance i)
        {
            if ((ElementList.Count > 0) && !SizeCheck(i))
            {
                throw new System.ArgumentException("Instances don't have all the same size", "lst");
            }

            if (ElementList.Count == 0)
            {
                Dimension = i.Size();
            }

            ElementList.Add(i);
        }
        /// <summary>
        /// Removes an Intance from ElementList
        /// </summary>
        /// <param name="i">Instance to be Removed</param>
        public void Remove(Instance i)
        {
            ElementList.Remove(i);
        }
        /// <summary>
        /// Checks if an Instance is compatible with the other ones in ElementList, that means, if his
        /// Size matches the Size of the other ones
        /// </summary>
        /// <param name="i">Instance we must run the compatibility check on</param>
        /// <returns>Returns true if the Instance is compatible, False otherwise</returns>
        public bool SizeCheck(Instance i)
        {
            if (i.Size() == Dimension)
            {
                return true;
            }
            return false;
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

            for (int i = 0; i < ElementList.Count; i++)
            {
                for (int j = i + 1; j < ElementList.Count; j++)
                {
                    double tmp = d.Distance(ElementList[i], ElementList[j]);
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
                throw new System.ArgumentException("The size of the random dataset must be greater than 0", "size");
            }

            if (instanceDimension <= 0)
            {
                throw new System.ArgumentException("The dimension of the instances must be greater than 0", "instanceDimension");
            }

            DataSet d = new DataSet();
            for (int i = 0; i < size; i++)
            {
                d.Add(Instance.RandomInstance(size));
            }
            return d;
        }
    }
}
