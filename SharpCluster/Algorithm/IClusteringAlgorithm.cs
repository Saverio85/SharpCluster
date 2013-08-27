using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpCluster
{
    /// <summary>
    /// Interface describing the prototype of a Clustering class
    /// </summary>
    public interface IClusteringAlgorithm
    {
        /// <summary>
        /// Function that performs the clustering algorithm defined by the class
        /// </summary>
        /// <param name="d"> Dataset that the algorithm will cluster</param>
        /// <returns> Returns the list of clusters found by the clustering algorithm</returns>
        List<DataSet> Cluster(DataSet d);
    }
}
