using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpCluster
{
    /// <summary>
    /// Class that performs the K-Means algorithm on a Dataset
    /// </summary>
    public class KMeans:ClusteringAlgorithm
    {
        /// <summary>
        /// Number of Clusters the algorithm must return
        /// </summary>
        public int ClusterNumber { get; private set; }
        /// <summary>
        /// Maximum number of iterations before the algorithm stops
        /// </summary>
        public int Iterations { get; private set; }
        /// <summary>
        /// Centroids to be used to initialize the algorithm
        /// </summary>
        public Instance[] Centroids { get; set; }
        /// <summary>
        /// Constructor of the KMeans Class
        /// </summary>
        /// <param name="clusterNumber">Number of clusters the algorithm must return</param>
        /// <param name="iterations">Maximum number of iterations before the algorithm stops</param>
        /// <param name="centroids">Initial centroids that the algorithm should use</param>
        public KMeans(int clusterNumber = 4, int iterations = 100, Instance[] centroids = null)
        {
            if (clusterNumber <= 0)
            {
                throw new System.ArgumentException("The number of clusters must be greater than 0", "clusters");
            }

            if ((centroids != null) && ((centroids.Length != clusterNumber)))
            {
                throw new System.ArgumentException("The number of centroids provided must be equal to the number of clusters","centroids");
            }
            ClusterNumber = clusterNumber;
            Iterations = iterations;
            Centroids = centroids;
        }
        /// <summary>
        /// Performs the K-Means algorithm on the dataset d
        /// </summary>
        /// <param name="d"> Dataset to be clustered</param>
        /// <returns>A list of datasets, each one rapresentig a cluster</returns>
        public List<DataSet> Cluster(DataSet d)
        {
            Instance[] TCentroids = new Instance[ClusterNumber];
            EuclidianDistance dist = new EuclidianDistance();

            int[] assign = new int[d.Count];
            int[] formerAssign = new int[d.Count];
            int counter = 0;

            do
            {
                counter++;
                if ( Centroids == null )
                {
                    TCentroids = GenerateCentroids(d);
                }
                Array.Copy(assign, formerAssign, formerAssign.Length);
                assign = Assign(d, Centroids, dist);
                TCentroids = UpdateCentroids(assign,d);
            } while ( !Enumerable.SequenceEqual( assign , formerAssign ) && (counter < Iterations));
            
            List<DataSet> solution = new List<DataSet>();
            for (int i = 0; i < ClusterNumber; i++)
            {
                solution.Add(new DataSet());
            }
            for (int i = 0; i < d.Count; i++)
            {
                solution[assign[i]].Add(d.ElementList[i]);
            }
            return solution;
        }

        private Instance[] UpdateCentroids(int[] assign, DataSet d)
        {
            Instance[] cent = new Instance[ClusterNumber];
            int[] cardinality = new int[ClusterNumber];

            for (int i = 0; i < ClusterNumber; i++)
            {
                cent[i] = Instance.ZeroInstance(d.Dimension);
            }

            for (int i = 0; i < d.Count; i++)
            {
                cent[assign[i]] += d.ElementList[i];
                cardinality[assign[i]]++;
            }

            for (int i = 0; i < ClusterNumber; i++)
            {
                double coefficient = 1 / cardinality[i];
                cent[i] = coefficient * cent[i];
            }
            return cent;
        }

        private Instance[] GenerateCentroids(DataSet d)
        {
            double[] maxValues = new double[d.Dimension];
            double[] minValues = new double[d.Dimension];
            Instance[] cent = new Instance[ClusterNumber];

            for (int i = 0; i < d.Dimension; i++)
            {
                for (int j = 0; j < d.Count; j++)
                {
                    maxValues[i] = Math.Max(maxValues[i], d.ElementList[j].Data[i]);
                    minValues[i] = Math.Min(minValues[i], d.ElementList[j].Data[i]);
                }
            }

            for (int i = 0; i < ClusterNumber; i++)
            {
                cent[i] = Instance.RandomInstance(d.Dimension, minValues, maxValues);
            }

            return cent;
        }

        private int[] Assign(DataSet d, Instance[] cent, IDistance dist)
        {
            int[] assignment = new int[d.Count];

            for (int i = 0; i < d.Count; i++)
            {
                Instance inst = d.ElementList[i];
                double min = dist.Distance(inst, Centroids[0]);
                for (int j = 1; j < ClusterNumber; j++)
                {
                    double tmp = dist.Distance(inst, Centroids[j]);
                    if (tmp < min)
                    {
                        min = tmp;
                        assignment[i] = j;
                    }
                }
            }
            return assignment;
        }
    }
}
