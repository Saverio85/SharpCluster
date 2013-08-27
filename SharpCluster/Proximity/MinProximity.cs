using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpCluster
{
    /// <summary>
    /// Class to compute the minimum proximity between clusters.
    /// </summary>
    public class MinProximity:IProximity
    {
        /// <summary>
        /// Compute the Minimum Proximity of two given Datasets, defined as the minimum
        /// of all the distances between an instance of firstDataset and one from secondDataset
        /// </summary>
        /// <param name="dist">Distance used to evaluate the distance between two instances</param>
        /// <param name="firstDataset">First Cluster, parametrized as a dataset</param>
        /// <param name="secondDataset">Second Cluster, parametrized as a dataset</param>
        /// <returns>The Minimum Proximity of the two clusters</returns>
        public double Compute(IDistance dist, DataSet firstDataset, DataSet secondDataset)
        {
            double minDistance = double.MaxValue;
            foreach (Instance instA in firstDataset)
            {
                foreach (Instance instB in secondDataset)
                {
                    double tmp = dist.Distance(instA, instB);
                    if (tmp < minDistance)
                    {
                        minDistance = tmp;
                    }
                }
            }
            return minDistance;
        }
    }
}
