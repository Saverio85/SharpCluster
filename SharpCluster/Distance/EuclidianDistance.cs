using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpCluster
{
    /// <summary>
    /// Class implementing the euclidian distance between vectors
    /// </summary>
    public class EuclidianDistance : IDistance
    {
        /// <summary>
        /// Method that calculate the distance between two instances
        /// </summary>
        /// <param name="a">First Instance</param>
        /// <param name="b">Second Instance</param>
        /// <returns> The Euclidian Distance between the two instances, or 0 if either one is null</returns>
        public double Distance(Instance a, Instance b)
        {
            if ((a == null) || (b == null))
            {
                return 0;
            }

            if (a.Size() != b.Size())
            {
                string method = System.Reflection.MethodBase.GetCurrentMethod().Name;
                throw new System.ArgumentException(method + " :: Instances don't have hte same Size", "a,b");
            }

            double t = 0;

            for (int i = 0; i < a.Data.Length; i++)
            {
                t += Math.Pow(a.Data[i] - b.Data[i], 2);
            }
            return Math.Sqrt(t);
        }
        /// <summary>
        /// Method to calculate the norm of an instance
        /// </summary>
        /// <param name="a"> The Instance</param>
        /// <returns> The norm of the instance a</returns>
        public double Norm(Instance a)
        {
            double tmp = 0;
            foreach (double d in a.Data)
            {
                tmp += Math.Pow(d, 2);
            }
            return Math.Sqrt(tmp);
        }
        /// <summary>
        /// Finds the point that minimize the sum of the distances of the
        /// Instances given in input from it
        /// </summary>
        /// <param name="lst">List of instances of which we need the center </param>
        /// <returns>The instance that minimizes the sum of distances from it</returns>
        public Instance Center(List<Instance> lst)
        {
            if(lst.Count == 0)
            {
                string method = System.Reflection.MethodBase.GetCurrentMethod().Name;
                throw new System.ArgumentException(method + " :: The element list can't be empty","lst");
            }

            int n = lst[0].Size();
            Instance tmp = Instance.ZeroInstance(n);

            foreach (Instance i in lst)
            {
                tmp += i;
            }

            double coefficient = (double) 1 / lst.Count;
            return coefficient * tmp;
        }
    }
}