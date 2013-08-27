using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace SharpCluster
{
    /// <summary>
    /// Class to compute the agerage proximity between clusters.
    /// </summary>
    public class AverageProximity : IProximity
    {
        /// <summary>
        /// Compute the Average Proximity of two given Datasets, defined as the average
        /// of all the distances between an instance of firstDataset and one from secondDataset
        /// </summary>
        /// <param name="dist">Distance used to evaluate the distance between two instances</param>
        /// <param name="firstDataset">First Cluster, parametrized as a dataset</param>
        /// <param name="secondDataset">Second Cluster, parametrized as a dataset</param>
        /// <returns>The Average Proximity of the two clusters</returns>
        public double Compute(IDistance dist, DataSet firstDataset, DataSet secondDataset)
        {
            if ((firstDataset.Count == 0) || (secondDataset.Count == 0))
            {
                return 0;
            }

            double tot = 0;

            foreach (Instance insta in firstDataset)
            {
                foreach (Instance instb in secondDataset)
                {
                    tot += dist.Distance(insta, instb);
                }
            }

            int cardinality = firstDataset.Count * secondDataset.Count;
            return (double)tot / cardinality;
        }
    }
}
