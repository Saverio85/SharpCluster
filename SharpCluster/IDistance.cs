using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpCluster
{
    public interface IDistance
    {
        double Distance(Instance a, Instance b);
        double Norm(Instance a);
    }
}
