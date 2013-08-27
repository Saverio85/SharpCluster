using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpCluster
{
    /// <summary>
    /// Interface of classes used to compute the Proximity of two clusters
    /// </summary>
    public interface IProximity
    {
        /// <summary>
        /// Compute the Proximity of two given Datasets
        /// </summary>
        /// <param name="dist">Distance used to evaluate the distance between two instances</param>
        /// <param name="firstDataset">First Cluster, parametrized as a dataset</param>
        /// <param name="secondDataset">Second Cluster, parametrized as a dataset</param>
        /// <returns>The Proximity of the two clusters</returns>
        double Compute(IDistance dist, DataSet firstDataset, DataSet secondDataset);
    }
}
