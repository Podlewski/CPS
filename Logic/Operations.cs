using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic
{
    public static class Operations
    {
        public static List<double> AddSignals(List<double> signal1, List<double> signal2)
        {
            List<double> result = new List<double>();
            for (int i = 0; i < signal1.Count; i++)
            {
                result.Add(signal1[i] + signal2[i]);
            }

            return result;
        }
    }
}
