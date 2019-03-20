using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace View.ViewModels
{
    public class PointXY
    {
        public double X { get; set; }

        public double Y { get; set; }

        public PointXY() { }

        public PointXY(double x, double y)
        {
            X = x;
            Y = y;
        }
    }
}
