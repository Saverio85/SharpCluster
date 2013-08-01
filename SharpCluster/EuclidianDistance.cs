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
        /// <returns> The Euclidian Distance between the two instances</returns>
        public double Distance(Instance a, Instance b)
        {
            if (a.Size() != b.Size())
            {
                throw new System.ArgumentException("Instances don't have hte same Size", "a,b"); 
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
    }
}
