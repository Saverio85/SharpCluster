using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpCluster
{
    /// <summary>
    /// Solves the K-Means problem using the Lloyd Algorithm.
    /// </summary>
    public class KMeans : IClusteringAlgorithm
    {
        /// <summary>
        /// Distance that K-Means will use to calculate how far apart
        /// the various instances in the DataSet are
        /// </summary>
        public IDistance Dist{get;private set;}
        /// <summary>
        /// An alternative stop condition for the K-Means algorithm, that
        /// is usually needed in case the DataSet is large enough; the stop
        /// happens if less than the percentage defined by thresold of Instances
        /// change cluster in a given iteration.
        /// </summary>
        public double Thresold{get;private set;}
        /// <summary>
        /// Number of times we want the algorithm to run, returning the 
        /// one scoring the best of them all
        /// </summary>
        public int Runs { get; private set; }
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
        /// <param name="distance">Distance used to calculate how far apart are the instances</param>
        /// <param name="thresold">Alternate stopping criterion, minimum percentage of instances that must change cluster to keep the algorithm running</param>
        /// <param name="runs">Number of runs the algorithm should make, to minimize the variance due to random start</param>
        public KMeans(  int clusterNumber = 4, 
                        int iterations = 100, 
                        Instance[] centroids = null, 
                        IDistance distance = null,
                        double thresold = 0,
                        int runs = 1)
        {
            if (clusterNumber <= 0)
            {
                string method = System.Reflection.MethodBase.GetCurrentMethod().Name;
                throw new System.ArgumentException(method + " :: The number of clusters must be greater than 0", "clusters");
            }

            if ((centroids != null) && ((centroids.Length != clusterNumber)))
            {
                string method = System.Reflection.MethodBase.GetCurrentMethod().Name;
                throw new System.ArgumentException(method + " :: The number of centroids provided must be equal to the number of clusters", "centroids");
            }
            ClusterNumber = clusterNumber;
            Iterations = iterations;
            Centroids = centroids;
            Dist = distance ?? new EuclidianDistance();
            Thresold = thresold;
            Runs = runs;
        }

        /// <summary>
        /// Performs the K-Means algorithm on the dataset d
        /// </summary>
        /// <param name="d"> Dataset to be clustered</param>
        /// <returns>A list of datasets, each one rapresentig a cluster</returns>
        public List<DataSet> Cluster(DataSet d)
        {
            int r = 0;
            List<DataSet> solution = new List<DataSet>();
            double score = double.MaxValue;

            while (r < Runs)
            {
                r++;
                Instance[] TCentroids = new Instance[ClusterNumber];
                int[] assign = new int[d.Count];
                int[] formerAssign = new int[d.Count];
                int counter = 0;

                if (Centroids == null)
                {
                    TCentroids = GenerateCentroids(d);
                }
                else
                {
                    for (int i = 0; i < ClusterNumber; i++)
                    {
                        TCentroids[i] = Centroids[i].Clone();
                    }
                }

                int changes;

                do
                {
                    counter++;
                    Array.Copy(assign, formerAssign, formerAssign.Length);
                    //Assign every instance to the closest centroid
                    assign = Assign(d, TCentroids, Dist);
                    //Update the centroids to the center of the cluster assigned to them
                    for (int i = 0; i < ClusterNumber; i++)
                    {
                        List<Instance> lst = new List<Instance>();
                        for (int j = 0; j < d.Count; j++)
                        {
                            if (assign[j] == i)
                            {
                                lst.Add(d[j]);
                            }
                        }

                        if (lst.Count > 0)
                        {
                            TCentroids[i] = Dist.Center(lst);
                        }
                        else
                        {
                            TCentroids[i] = null;
                        }
                    }

                    //Check if every centroid has at least one element in the relative cluster

                    List<int> nullCentroids = new List<int>();
                    for (int i = 0; i < ClusterNumber; i++)
                    {
                        if (TCentroids[i] == null)
                        {
                            nullCentroids.Add(i);
                        }
                    }

                    // if there are null centroids, order the Instances based on the distance
                    // from the centroids they're assigned to, and replace the null centroids with
                    // them
                    if (nullCentroids.Count > 0)
                    {
                        Console.WriteLine("Empty");
                        var ordered = d.Select((x, y) => new { Inst = x, Index = y })
                            .OrderBy(x => Dist.Distance(x.Inst, TCentroids[assign[x.Index]]))
                            .Take(nullCentroids.Count)
                            .Select(x => x.Inst)
                            .ToList();

                        for (int i = 0; i < nullCentroids.Count; i++)
                        {
                            TCentroids[nullCentroids[i]] = ordered[i].Clone();
                        }
                    }
                    changes = 0;
                    for (int i = 0; i < assign.Length; i++)
                    {
                        if (assign[i] != formerAssign[i])
                        {
                            changes++;
                        }
                    }
                } while (((double)changes / assign.Length > Thresold) && (counter < Iterations));

                double tmpscore = 0;

                for (int i = 0; i < 10; i++)
                {
                    tmpscore += Math.Pow(Dist.Distance(d[i], TCentroids[assign[i]]), 2);
                }

                if (tmpscore < score)
                {

                    score = tmpscore;
                    solution = new List<DataSet>();
                    for (int i = 0; i < ClusterNumber; i++)
                    {
                        solution.Add(new DataSet());
                    }
                    for (int i = 0; i < d.Count; i++)
                    {
                        solution[assign[i]].Add(d[i]);
                    }
                }
            }
            return solution;
        }

        private Instance[] GenerateCentroids(DataSet d)
        {
            double[] maxValues = new double[d.Dimension];
            double[] minValues = new double[d.Dimension];
            Instance[] cent = new Instance[ClusterNumber];

            for (int i = 0; i < d.Dimension; i++)
            {
                maxValues[i] = 0;
                minValues[i] = 1;

                for (int j = 0; j < d.Count; j++)
                {
                    maxValues[i] = Math.Max(maxValues[i], d[j].Data[i]);
                    minValues[i] = Math.Min(minValues[i], d[j].Data[i]);
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
                Instance inst = d[i];
                double min = dist.Distance(inst, cent[0]);
                for (int j = 1; j < ClusterNumber; j++)
                {
                    double tmp = dist.Distance(inst, cent[j]);
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
