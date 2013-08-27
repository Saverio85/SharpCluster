using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace SharpCluster.Algorithm
{
    /// <summary>
    /// Solves the clustering problem on a given dataset using the DBScan algorithm, 
    /// a density based centroidal clustering
    /// </summary>
    public class DBScan:IClusteringAlgorithm
    {
        /// <summary>
        /// Radius of the "sphere" built around every Instance, in which we look
        /// for other Instances to define its Density
        /// </summary>
        public double Eps { get; private set; }
        /// <summary>
        /// Minimum number of Instances needed inside the sphere of radius Eps
        /// for the Instance to be a Core one
        /// </summary>
        public double MinPts { get; private set; }
        /// <summary>
        /// Distance used to calculate how far apart Instances are
        /// </summary>
        public IDistance Dist { get; private set; }

        private enum Position { Core, Border, Noise };

        private Func<Position, int> posToInt = x => (x == Position.Core) ? 0 : ((x == Position.Border) ? 1 : 2);
        /// <summary>
        /// Create a new instance of the DBScan class, defining the eps and the minpts value. the distance
        /// is set to be Euclidian by default.
        /// </summary>
        /// <param name="eps">Radius of the "sphere" built around every Instance, in which we look
        /// for other Instances to define its Density</param>
        /// <param name="minpts">Minimum number of Instances needed inside the sphere of radius Eps
        /// for the Instance to be a Core one</param>
        /// <param name="dist">Distance used to calculate how far apart Instances are</param>
        public DBScan(double eps,double minpts, IDistance dist = null)
        {
            if (minpts <= 0)
            {
                string method = System.Reflection.MethodBase.GetCurrentMethod().Name;
                throw new System.ArgumentException(method + " :: The number of clusters must be greter than 0", "minpts");
            }
            if (eps <= 0)
            {
                string method = System.Reflection.MethodBase.GetCurrentMethod().Name;
                throw new System.ArgumentException(method + " :: The minimum distance must pe greter than 0", "eps");
            }

            Dist = dist ?? new EuclidianDistance();
            Eps = eps;
            MinPts = minpts;
        }
        /// <summary>
        /// Performs the DBScan Clustering of the dataset d, using the parameters set in the constructor
        /// </summary>
        /// <param name="d">Dataset to be clustered</param>
        /// <returns>The clustering result</returns>
        public List<DataSet> Cluster(DataSet d)
        {
            double[,] distanceMatrix = GenDistanceMatrix(d);
            int[,] neighborMatrix = GenNeighborMatrix(distanceMatrix);

            var classification = Enumerable.Range(0, d.Count)
                .Select(x => (neighborMatrix.GetRow(x).Sum() >= MinPts) ? Position.Core : Position.Noise)
                .ToArray();

            for (int i = 0; i < d.Count; i++)
            {
                if (classification[i] == Position.Noise)
                {
                    for (int j = 0; j < d.Count; j++)
                    {
                        if ((neighborMatrix[i, j] == 1) && (classification[j] == Position.Core))
                        {
                            classification[i] = Position.Border;
                            break;
                        }
                    }
                }
            }

            //classification.GroupBy(x => x).ForEach(x => Debug.WriteLine(Enum.GetName(typeof(Position),x.Key) + " : " + x.Count()));

            int[] assign = new int[d.Count];
            Enumerable.Range(0, d.Count).ForEach(i => assign[i] = -1);
            int cluster = 0;

            for (int i = 0; i < d.Count; i++)
            {
                if ((classification[i] == Position.Core) && (assign[i] == -1))
                {
                    cluster++;
                    Queue<int> q = new Queue<int>();
                    q.Enqueue(i);
                    while (q.Count > 0)
                    {
                        int k = q.Dequeue();
                        for (int j = 0; j < d.Count; j++)
                        {
                            if ((neighborMatrix[k, j] == 1) && (assign[j] == -1) && (classification[j] == Position.Core))
                            {
                                assign[j] = cluster;
                                q.Enqueue(j);
                            }
                        }
                    }
                }
            }

            for (int i = 0; i < d.Count; i++)
            {
                if ((assign[i] == -1) && (classification[i] == Position.Border))
                {
                    int minIndex = -1;
                    double minDistance = double.MaxValue;
                    for (int j = 0; j < d.Count; j++)
                    {
                        if ((neighborMatrix[i, j] == 1) && (classification[j] == Position.Core))
                        {
                            if (distanceMatrix[i,j] < minDistance)
                            {
                                minIndex = j;
                                minDistance = distanceMatrix[i,j];
                            }
                        }
                    }
                    assign[i] = assign[minIndex];
                }
            }

            List<DataSet> solution = new List<DataSet>();
            Enumerable.Range(0,cluster+1).ForEach(_=>solution.Add(new DataSet()));
            Enumerable.Range(0, d.Count).ForEach(i => {
                int index = (assign[i] == -1) ? 0 : assign[i];
                solution[index].Add(d[i]);
            });
            Debug.WriteLine(solution.Count);
            return solution;
        }

        private double[,] GenDistanceMatrix(DataSet d)
        {
            double[,] distanceMatrix = new double[d.Count, d.Count];

            for (int i = 0; i < d.Count; i++)
            {
                for (int j = i + 1; j < d.Count; j++)
                {
                    double dist = Dist.Distance(d[i], d[j]);
                    distanceMatrix[i, j] = distanceMatrix[j, i] = dist;
                }
            }
            return distanceMatrix;
        }

        private int[,] GenNeighborMatrix(double[,] distMatrix)
        {
            int rows = distMatrix.GetLength(0);
            int cols = distMatrix.GetLength(1);
            int[,] m = new int[rows, cols];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    if (distMatrix[i, j] < Eps)
                    {
                        m[i, j] = m[j, i] = 1;
                    }
                }
            }
            return m;
        }
        /// <summary>
        /// Method used to find the best MinPts and Eps values. It returns a list of doubles of size
        /// d.Count, each one rapresenting the distance between i_th Instance and the k_th closer to it
        /// </summary>
        /// <param name="d">Dataset we want to test</param>
        /// <param name="k">For each element in d, k defines the k_th further element we want the distance
        /// to be calculated from</param>
        /// <returns>The list of distnace, with the element i_th set as the distance of d[i] to the
        /// k_th closer elemtn</returns>
        public List<double> GetKDistance(DataSet d,int k)
        {
            double[,] distMatrix = GenDistanceMatrix(d);
            var l = Enumerable.Range(0, d.Count)
                .Select(x => distMatrix.GetRow(x).OrderBy(z => z).Take(k).Last())
                .OrderBy(y=>y)
                .ToList();
            return l;
        }
    }
}
