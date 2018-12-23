using System;

namespace CasingDesign.API.Models
{
    public class OutputData
    {
        public double ExternalDiameter { get; set; }   
        public double InternalDiameter { get; set; }
        public string Name { get; set; }
        public double YieldStrength { get; set; }
        public double Weight { get; set; }
        public double Burst { get; set; }
        public double Collapse { get; set; }
        public double Axial { get; set; }
        public double Cost { get; set; }
        public double Area {get
            {
                return Math.PI*( (ExternalDiameter*ExternalDiameter) - (InternalDiameter*InternalDiameter) )/4;
            }
        }

        public double MaxDepth { get; set; }
        public double TotalCost { get; set; }
        public double Length { get; set; }
    }
}