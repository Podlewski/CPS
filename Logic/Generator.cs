using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic
{
    public class Generator
    {
        private Random random = new Random();
        public double Amplitude { get; set; }
        public double StartTime { get; set; }
        public double Period { get; set; }
        public double JumpTime { get; set; }
        public double JumpN { get; set; }
        public double FillFactor { get; set; }

        public double GenerateSinusoidalSignal(double time)
        {
            return Amplitude * Math.Sin((2 * Math.PI / Period) * (time - StartTime));
        }

    }
}
