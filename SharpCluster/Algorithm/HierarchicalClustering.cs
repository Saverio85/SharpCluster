using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpCluster.Algorithm
{
    /// <summary>
    /// Solves the clustering problem on a given dataset using Hierarchical Clustering
    /// </summary>
    public class HierarchicalClustering : IClusteringAlgorithm
    {
        /// <summary>
        /// Proximity used to compute the distance between two different clusters
        /// </summary>
        public IProximity Proximity { get; private set; }
        /// <summary>
        /// Distance that the algorithm will use to compute how fare apart two Instances are
        /// </summary>
        public IDistance Distance { get; private set; }
        /// <summary>
        /// Number of Cluster we want to get in the solution
        /// </summary>
        public int ClusterNumber{get; private set;}

        /// <summary>
        /// Construct the class defining a custom value for the proximity, distance and for the number of clusters
        /// </summary>
        /// <param name="proximity">Proximity used to compute the distance between two different clusters</param>
        /// <param name="distance">Distance that the algorithm will use to compute how fare apart two Instances are</param>
        /// <param name="clusters">Number of Cluster we want to get in the solution</param>
        public HierarchicalClustering(
            IProximity proximity,
            IDistance distance,
            int clusters)
        {

            if (clusters <= 0)
            {
                string method = System.Reflection.MethodBase.GetCurrentMethod().Name;
                throw new System.ArgumentException(method + " :: The number of clusters must be greter than 0", "clusters");
            }
            Proximity = proximity;
            Distance = distance;
            ClusterNumber = clusters;
        }
        /// <summary>
        /// Performs the Hierarchical Clustering of the dataset d, using the parameters set in the constructor
        /// </summary>
        /// <param name="d">Dataset to be clustered</param>
        /// <returns>The clustering result</returns>
        public List<DataSet> Cluster(DataSet d)
        {
            List<DataSet>[] tree = new List<DataSet>[d.Count];
            List<double[,]> proximityMatrices = new List<double[,]>();
            
            tree[0] = new List<DataSet>();
            d.ForEach(x=>tree[0].Add(new DataSet(x)));

            proximityMatrices.Add(new double[d.Count,d.Count]);
            for (int i = 0; i < tree[0].Count; i++)
            {
                for (int j = i+1; j < tree[0].Count; j++)
                {
                    proximityMatrices[0][i, j] =
                        proximityMatrices[0][j, i] =
                        Proximity.Compute(new EuclidianDistance(),tree[0][i],tree[0][j]);
                }
            }

            for (int i = 0; i < d.Count-1; i++)
            {
                int n = proximityMatrices[i].GetLength(0);

                // find the two dataset with the minimum proximity
                int[] minArr = minUpperTriangular(proximityMatrices[i]);
                int minx = minArr[0];
                int miny = minArr[1];
                double minProximity = proximityMatrices[i][minx, miny] ;

                // merge them and create the new tree
                tree[i+1] = new List<DataSet>();

                for (int j = 0; j < tree[i].Count; j++)
                {
                    if (j == minx)
                    {
                        tree[i + 1].Add(DataSet.Merge(tree[i][minx], tree[i][miny]));
                    }
                    if ((j != minx) && (j != miny))
                    {
                        tree[i + 1].Add(tree[i][j]);
                    }
                }

                //create the new proximity matrix
                proximityMatrices.Add(new double[n - 1, n - 1]);

                for (int x = 0; x < n; x++)
                {
                    for (int y = x + 1; y < n; y++)
                    {
                        if ((x != miny) && (y != miny))
                        {
                            int slidex = (x < miny) ? x : x - 1;
                            int slidey = (y < miny) ? y : y - 1;
                            if ((x == minx) || y == minx)
                            {
                                proximityMatrices[i + 1][slidex, slidey] = 
                                    proximityMatrices[i + 1][slidey, slidex] =
                                    Proximity.Compute(Distance, tree[i + 1][slidex], tree[i + 1][slidey]);
                            }
                            else
                            {
                                proximityMatrices[i + 1][slidex,slidey] =
                                    proximityMatrices[i + 1][slidey, slidex] =
                                    proximityMatrices[i][x, y];
                            }
                        }
                    }
                }
            }
            return tree[d.Count - ClusterNumber];
        }

        private int[] minUpperTriangular(double[,] d)
        {
            int minx = 0;
            int miny = 0;
            double minProximity = double.MaxValue;

            int n = d.GetLength(0);

            // find the two dataset with the minimum proximity
            int[] minArr = d.MinIndex();

            for (int x = 0; x < n; x++)
            {
                for (int y = x + 1; y < n; y++)
                {
                    if (d[x, y] < minProximity)
                    {
                        minx = x;
                        miny = y;
                        minProximity = d[x, y];
                    }
                }
            }

            return new int[] { minx, miny };
        }
    }
}
