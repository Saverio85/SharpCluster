using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpCluster
{
    interface ClusteringAlgorithm
    {
        List<DataSet> Cluster(DataSet d);
    }
}
