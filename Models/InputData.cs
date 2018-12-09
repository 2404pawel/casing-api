using System;

namespace CasingDesign.API.Models
{
    public class InputData
    {
        public double Diameter { get; set; }
        public double Depth { get; set; }
        public double FluidDensity { get; set; }
        public double PorePressure { get; set; }
        public double BurstFactor { get; set; }
        public double CollapseFactor { get; set; }
        public double TensionFactor { get; set; }
        public double MinimalSectionLength { get; set; }
    }
}