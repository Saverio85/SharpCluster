using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpCluster
{
    /// <summary>
    /// Interface for the Distance Classes
    /// </summary>
    public interface IDistance
    {
        /// <summary>
        /// Method that calculate the distance between two instances
        /// </summary>
        /// <param name="a">First Instance</param>
        /// <param name="b">Second Instance</param>
        /// <returns> Distance between the two Instances, or 0 if either one is null</returns>
        double Distance(Instance a, Instance b);
        /// <summary>
        /// Finds the point that minimize the sum of the distances of the
        /// Instances given in input from it
        /// </summary>
        /// <param name="lst">List of instances of which we need the center </param>
        /// <returns>The instance that minimizes the sum of distances from it</returns>
        Instance Center(List<Instance> lst);
    }
}
