using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpCluster.Proximity
{
    /// <summary>
    /// Class to compute the centroid proximity between clusters.
    /// </summary>
    public class CentroidProximity:IProximity
    {
        /// <summary>
        /// Compute the Centroid Proximity of two given Datasets, defined as the distance
        /// between the centers of the two clusters
        /// </summary>
        /// <param name="dist">Distance used to evaluate the distance between two instances</param>
        /// <param name="firstDataset">First Cluster, parametrized as a dataset</param>
        /// <param name="secondDataset">Second Cluster, parametrized as a dataset</param>
        /// <returns>The Centroid Proximity of the two clusters</returns>
        public double Compute(IDistance dist, DataSet firstDataset, DataSet secondDataset)
        {
            Instance centroidA = dist.Center(firstDataset.Cast<Instance>().ToList());
            Instance centroidB = dist.Center(secondDataset.Cast<Instance>().ToList());
            return dist.Distance(centroidA, centroidB);
        }
    }
}
