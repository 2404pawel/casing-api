using System;
using System.Collections.Generic;
using System.Linq;
using CasingDesign.API.Entities;
using CasingDesign.API.Models;

namespace CasingDesign.API.Services
{
    public interface ICalculateVerticalRepository
    {
        IEnumerable<CasingDto> GetCasingInventoryList();
        IEnumerable<CasingDto> GetCasingInventoryListByDiameter(double diameter);

        double BurstValue(double porePressure, double burstFactor);
        double CollapseValueAtShoe(double fluidDensity, double depth, double collapseFactor);
        CasingDto FirstCasingForBurstAndCollapse(IEnumerable<CasingDto> casingInventoryCollecion, double burstRequiredResistance, double collapseRequiredResistance, double depth);
        IOrderedEnumerable<CasingDto> MaxCasingDepthsWithoutAxial(IOrderedEnumerable<CasingDto> casingResultsListToSelect,
                                       CasingDto firstCasingForBurstAndCollapse, double depth, double fluidDensity, double collapseFactor);
        List<CasingDto> SelectedCasingDepthsWithoutAxialBestCost(IOrderedEnumerable<CasingDto> maxCasingDepthsWithoutAxial, CasingDto firstCasingForBurstAndCollapse);
        List<CasingDto> MinLengthCheck(List<CasingDto> selectedCasingDepthsWithoutAxialBestCost, double minimalSectionLength);
        void LengthCalc(List<CasingDto> minLengthChecked);
        void TensionCheck(List<CasingDto> casings, double tensionFactor);
                 
        List<CasingDto> CalculateCasingDesign(double diameter, double depth, double fluidDensity, 
                                    double porePressure, double burstFactor, double collapseFactor, 
                                    double tensionFactor, double minimalSectionLength );

                
                
        
        // IDictionary<CasingDto,double> MaxCasingDepthsForCollapseBiAxial(IOrderedEnumerable<CasingDto> casingResultsListToSelect, CasingDto firstCasingForBurstAndCollapse, 
        //                             double depth, double fluidDensity, double collapseFactor);
        // //IDictionary<int,double> MaxCasingDepthsForCollapseBiAxialTotalCost(IDictionary<CasingDto,double> maxCasingDepthsForCollapseBiAxial);
        // List<KeyValuePair<int,double>> MaxCasingDepthsForCollapseBiAxialTotalCost(IDictionary<CasingDto,double> maxCasingDepthsForCollapseBiAxial);

        // IEnumerable<CasingDto> CalculateCasingDesign(double diameter, double depth, double fluidDensity, 
        //                             double porePressure, double burstFactor, double collapseFactor, 
        //                             double tensionFactor, double minimalSectionLength );                      
        // IDictionary<int, double> CalculateCasingDesign(double diameter, double depth, double fluidDensity, 
        //                             double porePressure, double burstFactor, double collapseFactor, 
        //                             double tensionFactor, double minimalSectionLength );



    }
}