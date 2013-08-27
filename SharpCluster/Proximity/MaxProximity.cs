using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpCluster
{
    /// <summary>
    /// Class to compute the max proximity between clusters.
    /// </summary>
    public class MaxProximity:IProximity
    {
        /// <summary>
        /// Compute the Max Proximity of two given Datasets, defined as the maximum
        /// of all the distances between an instance of firstDataset and one from secondDataset
        /// </summary>
        /// <param name="dist">Distance used to evaluate the distance between two instances</param>
        /// <param name="firstDataset">First Cluster, parametrized as a dataset</param>
        /// <param name="secondDataset">Second Cluster, parametrized as a dataset</param>
        /// <returns>The Maximum Proximity of the two clusters</returns>
        public double Compute(IDistance dist, DataSet firstDataset, DataSet secondDataset)
        {
            double maxDistance = double.MinValue;
            foreach (Instance instA in firstDataset)
            {
                foreach (Instance instB in secondDataset)
                {
                    double tmp = dist.Distance(instA, instB);
                    if (tmp > maxDistance)
                    {
                        maxDistance = tmp;
                    }
                }
            }
            return maxDistance;
        }
    }
}
