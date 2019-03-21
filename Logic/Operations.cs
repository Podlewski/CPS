using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic
{
    public static class Operations
    {
        public static List<double> AddSignals(List<double> signalX, List<double> signalY)
        {
            List<double> result = new List<double>();

            for (int i = 0; i < signalX.Count; i++)
            {
                result.Add(signalX[i] + signalY[i]);
            }

            return result;
        }


        public static List<double> SubtractSignals(List<double> signalX, List<double> signalY)
        {
            List<double> result = new List<double>();

            for (int i = 0; i < signalX.Count; i++)
            {
                result.Add(signalX[i] - signalY[i]);
            }

            return result;
        }

        public static List<double> MultiplySignals(List<double> signalX, List<double> signalY)
        {
            List<double> result = new List<double>();

            for (int i = 0; i < signalX.Count; i++)
            {
                result.Add(signalX[i] * signalY[i]);
            }

            return result;
        }

        public static List<double> DivideSignals(List<double> signalX, List<double> signalY)
        {
            List<double> result = new List<double>();

            for (int i = 0; i < signalX.Count; i++)
            {
                result.Add(signalX[i] / signalY[i]);
            }

            return result;
        }
    }
}
