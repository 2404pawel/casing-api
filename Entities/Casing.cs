using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CasingDesign.API.Entities

{
    public class Casing
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public double ExternalDiameter { get; set; }
        [Required]
        public double InternalDiameter { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public double YieldStrength { get; set; }
        [Required]
        public double Weight { get; set; }
        [Required]
        public double Burst { get; set; }
        [Required]
        public double Collapse { get; set; }
        [Required]
        public double Axial { get; set; }
        [Required]
        public double Cost { get; set; }

    }
}