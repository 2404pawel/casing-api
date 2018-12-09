using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using CasingDesign.API.Entities;
using CasingDesign.API.Services;
using AutoMapper;

namespace CasingDesign.API.Models
{
    public class CalculateVerticalRepository : ICalculateVerticalRepository
    {
        private CasingDesignContext _casingDesignContext;

        public CalculateVerticalRepository(CasingDesignContext casingDesignContext)
        {
            _casingDesignContext = casingDesignContext; //wstrzykuje zaleznosc baze danych zeby z niej korzystac w obl.
        }

        public IEnumerable<CasingDto> GetCasingInventoryList()
        {
            //return  _casingDesignContext.CasingInventory.ToList(); //zeby mozna bylo zrobic foreach pozniej to ienumerable
            var casingInventory = _casingDesignContext.CasingInventory.ToList();
            var results = Mapper.Map<IEnumerable<CasingDto>>(casingInventory).ToList();

            return results; 
        }

        public IEnumerable<CasingDto> GetCasingInventoryListByDiameter(double diameter)
        {
            //return _casingDesignContext.CasingInventory.Where(c => c.ExternalDiameter == diameter).ToList();
            var casingInventoryListByDiameter = _casingDesignContext.CasingInventory.Where(c => c.ExternalDiameter == diameter).ToList();
            var results = Mapper.Map<IEnumerable<CasingDto>>(casingInventoryListByDiameter).ToList();

            return results;
        }

        public double BurstValue(double porePressure, double burstFactor)
        {
            return porePressure * burstFactor;
        }

        //required collapse resistance at the point of depth
        public double CollapseValueAtShoe(double fluidDensity, double depth, double collapseFactor)
        {
            return 0.052 * fluidDensity * depth * collapseFactor;
        }

        public CasingDto FirstCasingForBurstAndCollapse(IEnumerable<CasingDto> casingInventoryCollecion, double burstRequiredResistance, double collapseRequiredResistance, double depth)
        {
            CasingDto firstCasingForBurstAndCollapse = firstCasingForBurstAndCollapse = casingInventoryCollecion.Where(c => c.Burst >= burstRequiredResistance 
                                     && c.Collapse >= collapseRequiredResistance).OrderBy(c => c.Cost).First();
            firstCasingForBurstAndCollapse.TotalCost = depth*firstCasingForBurstAndCollapse.Cost;
            firstCasingForBurstAndCollapse.MaxDepth = depth;
            
            return firstCasingForBurstAndCollapse;
        }

        public IOrderedEnumerable<CasingDto> CasingResultsListToSelect(IEnumerable<CasingDto> casingInventoryCollecion,CasingDto firstCasingForBurstAndCollapse,
                                            double burstRequiredResistance)
        {

            var results = casingInventoryCollecion.Where(c => c.Burst >= burstRequiredResistance 
                                            && c.Collapse < firstCasingForBurstAndCollapse.Collapse // < firstCas... not to put first (burst&collapse OK) to list. Otherwise would be <=
                                            && c.Cost < firstCasingForBurstAndCollapse.Cost).OrderBy(c => c.Cost); // < firstCas... not to put first (burst&collapse OK) to list. Otherwise would be <=
         
            return results; 
        }

        public IOrderedEnumerable<CasingDto> MaxCasingDepthsWithoutAxial(IOrderedEnumerable<CasingDto> casingResultsListToSelect, 
                                       CasingDto firstCasingForBurstAndCollapse, double depth, double fluidDensity, double collapseFactor)
        {
            List<CasingDto> maxCasingDepthsWithoutAxial = new List<CasingDto>();

            // if only one casing is possible from bottom to top. End of calculations, return this one element
            if (!casingResultsListToSelect.Any())
            {
                firstCasingForBurstAndCollapse.MaxDepth = depth;
                firstCasingForBurstAndCollapse.TotalCost = firstCasingForBurstAndCollapse.Cost * depth;
                maxCasingDepthsWithoutAxial.Add(firstCasingForBurstAndCollapse);
                return maxCasingDepthsWithoutAxial.OrderBy(c => c.TotalCost);
            }

            foreach (var casing in casingResultsListToSelect)
            {
                casing.MaxDepth = casing.Collapse/ (collapseFactor* 0.052* fluidDensity);
                casing.TotalCost = casing.MaxDepth * casing.Cost;
                maxCasingDepthsWithoutAxial.Add(casing);
            }
            
            return maxCasingDepthsWithoutAxial.OrderBy(c => c.TotalCost);
        }

        public List<CasingDto> SelectedCasingDepthsWithoutAxialBestCost(IOrderedEnumerable<CasingDto> maxCasingDepthsWithoutAxial, CasingDto firstCasingForBurstAndCollapse)
                                                                        
        {
            List<CasingDto> selectedCasingDepthsWithoutAxialBestCost = new List<CasingDto>();
            selectedCasingDepthsWithoutAxialBestCost.Add(maxCasingDepthsWithoutAxial.First()); // adding 1st element as the cheapest one        

            foreach (var casing in maxCasingDepthsWithoutAxial)
            {
                if (casing.MaxDepth > selectedCasingDepthsWithoutAxialBestCost.Last().MaxDepth)
                {
                    selectedCasingDepthsWithoutAxialBestCost.Add(casing);
                }
            }

            selectedCasingDepthsWithoutAxialBestCost.Add(firstCasingForBurstAndCollapse);

            return selectedCasingDepthsWithoutAxialBestCost;
        }

        public List<CasingDto> MinLengthCheck(List<CasingDto> selectedCasingDepthsWithoutAxialBestCost, double minimalSectionLength)
        {
            List<CasingDto> sortedList = selectedCasingDepthsWithoutAxialBestCost.OrderByDescending(c => c.MaxDepth).ToList(); // sortuje, zeby pierwsza byla tą ktora jest najnizej
            List<CasingDto> minLengthChecked = new List<CasingDto>();
            int i=0;
            int temp=1;

            minLengthChecked.Add(sortedList.First()); // adding firstForBurstAndCollapse

            //for (int i = 0; i < selectedCasingDepthsWithoutAxialBestCost.Count; i++)
            while (i < sortedList.Count-1)
            {
                
                while (sortedList[i].MaxDepth - sortedList[temp].MaxDepth < minimalSectionLength)
                {
                    temp++;
                    if (temp==sortedList.Count) // jak nie ma nic spelniajacego kryeria to dociagnij ta ktora jest
                    {
                        return minLengthChecked;
                    }
                }
                // wychodzi jak znajdzie pierwsza rure powyzej, dla ktorej L > Lmin
                i=temp;
                temp++;
                
                minLengthChecked.Add(sortedList[i]);   
            }

            return minLengthChecked;
        }

        public void LengthCalc(List<CasingDto> minLengthChecked)
        {
            for (int i = 0; i < minLengthChecked.Count-1; i++)
            {
                minLengthChecked[i].Length = minLengthChecked[i].MaxDepth - minLengthChecked[i+1].MaxDepth;
            }
            minLengthChecked.Last().Length = minLengthChecked.Last().MaxDepth;

        }

        public void TensionCheck(List<CasingDto> casings, double tensionFactor)
        {
            double tension = 0;

            for (int i = 0; i < casings.Count; i++)
            {
                tension += casings[i].Weight*casings[i].Length;
                if (tension > casings[i].Axial/tensionFactor)
                {
                    throw new Exception("Tension is too high");
                }
            }
        }

        
        

        // public List<CasingDto> CheckTension(List<CasingDto> minLengthChecked, double tensionFactor)
        // {
        //     foreach
        // }

        // public List<CasingDto> AxialOnBurst(List<CasingDto> selectedCasingDepthsWithoutAxialBestCost, CasingDto firstCasingForBurstAndCollapse, 
        //                                     double depth, double fluidDensity, double porePressure, double burstFactor, 
        //                                     double collapseFactor, double tensionFactor, double minimalSectionLength)
        // {
        //     List<CasingDto> selectedCasings = new List<CasingDto>();
        //     double weightBelow = 0;
        //     double step = minimalSectionLength;
        //     double currentDepth;
        //     double maxDepthWithAxial1;
        //     double maxDepthWithAxial2;
        //     double lengthOfCasingBelow;
        //     double weightOfCasingBelow;
        //     double weightBelowTemp;
        //     double axialStressCausedByCasingBelow;
        //     double collapseReducedByAxial;
        //     double maxCollapsePressureReducedByAxial;

        //     // searching the depth of start, not to calculate when no other casings than first possible
        //     // OrderByDesc -> first on the top is the deepest possible (collapse only no axial)
        //     double tempDepth = selectedCasingDepthsWithoutAxialBestCost.OrderByDescending(c => c.MaxDepth).First().MaxDepth;
        //     if (tempDepth < (depth-minimalSectionLength)) // np 7240 < 7900 (8000-100)
        //     {
        //         currentDepth = tempDepth; // start calculate from this depth, less looping
        //     }
        //     else
        //     {
        //         currentDepth = depth - minimalSectionLength; // zakladam ze ta wybrana na dole musi miec co najmniej minLength
        //     }
            
        //     firstCasingForBurstAndCollapse.Length = depth - currentDepth;
        //     selectedCasings.Add(firstCasingForBurstAndCollapse);

        //     weightBelow = firstCasingForBurstAndCollapse.Length * firstCasingForBurstAndCollapse.Cost;
            
        //     List<CasingDto> tempListOfCasings = new List<CasingDto>();

        //     // wchodze do petli juz na glebokosci gdzie moze byc inna rura niz firstForBurstAndCollapse(ify na gorze)
        //     while (currentDepth > 0)
        //     {
        //         foreach (var casing in selectedCasingDepthsWithoutAxialBestCost)
        //         {
        //             if (casing.MaxDepth <= currentDepth)
        //             {
        //                 continue;
        //             }
                
        //             maxDepthWithAxial2 = casing.MaxDepth;
                    
        //             do
        //             {   
        //                 maxDepthWithAxial1 = maxDepthWithAxial2;
        //                 lengthOfCasingBelow = depth - maxDepthWithAxial1;
        //                 weightBelowTemp = lengthOfCasingBelow * selectedCasings.Last().Weight;
        //                 axialStressCausedByCasingBelow = weightBelowTemp / selectedCasings.Last().Area;

        //                 collapseReducedByAxial = casing.Collapse * 
        //                 ( 
        //                     (Math.Sqrt(1- (0.75* (axialStressCausedByCasingBelow/casing.YieldStrength)*
        //                                         (axialStressCausedByCasingBelow/casing.YieldStrength) ))-
        //                                             0.5* (axialStressCausedByCasingBelow/casing.YieldStrength) 
        //                     ) 
        //                 );

        //                 maxCollapsePressureReducedByAxial = collapseReducedByAxial / collapseFactor;
        //                 maxDepthWithAxial2 = maxCollapsePressureReducedByAxial / (0.052*fluidDensity);

        //             }while (Math.Abs(maxDepthWithAxial2 - maxDepthWithAxial1)>5);
        //             casing.MaxDepth = maxDepthWithAxial2;
        //             casing.TotalCost = casing.MaxDepth*casing.Cost;
        //             tempListOfCasings.Add(casing);
        //         }

        //         selectedCasings.Add(tempListOfCasings.OrderBy(c => c.TotalCost).First()); // dodaje najtansza
        //         //zwieksz ciezar ponizej


        //         currentDepth-=step;                

        //     }

        // }

        




        public List<CasingDto> CalculateCasingDesign(double diameter, double depth, double fluidDensity, 
                                    double porePressure, double burstFactor, double collapseFactor, 
                                    double tensionFactor, double minimalSectionLength )
        {
            var casingInventoryCollecion = GetCasingInventoryListByDiameter(diameter);

            //burst
            double burstRequiredResistance = BurstValue(porePressure, burstFactor); //required burst resistance, all along the same
            //var burstCasingList = casingInventoryCollecion.Where(c => c.Burst >= burstRequiredResistance).ToList(); // all casing that OK to burst required resistance

            //collapse
            double collapseRequiredResistance = CollapseValueAtShoe(fluidDensity, depth, collapseFactor); // collapse at shoe->max required collapse resistance
            //var collapseCasingList = casingInventoryCollecion.Where(c => c.Collapse >= collapseRequiredResistance).ToList();

            CasingDto firstCasingForBurstAndCollapse = FirstCasingForBurstAndCollapse(casingInventoryCollecion, burstRequiredResistance, collapseRequiredResistance, depth);
            //CasingDto firstCasingForBurstAndCollapse = casingInventoryCollecion.Where(c => c.Burst >= burstRequiredResistance 
            //                         && c.Collapse >= collapseRequiredResistance).OrderBy(c => c.Cost).First();//do sprawdzenia nastepnego. Tylko Id, bez sensu przekazywac caly obiekt

            //CasingDto firstCasingForBurstAndCollapse = casingResultsList.First(); // cheapest one
            // var firstCasingForBurstAndCollapse = casingInventoryCollecion.Where(c => c.Burst >= burstRequiredResistance 
            //                          && c.Collapse >= collapseRequiredResistance).First(); // only first that matches
            
            // List with casings burst ok and collapse to choose 
            var casingResultsListToSelect = casingInventoryCollecion.Where(c => c.Burst >= burstRequiredResistance 
                                            && c.Collapse < firstCasingForBurstAndCollapse.Collapse // < firstCas... not to put first (burst&collapse OK) to list. Otherwise would be <=
                                            && c.Cost < firstCasingForBurstAndCollapse.Cost).OrderBy(c => c.Cost); // < firstCas... not to put first (burst&collapse OK) to list. Otherwise would be <=
            
            // List ordered by cost of all casings max depths without axial influence
            var maxCasingDepthsWithoutAxial = MaxCasingDepthsWithoutAxial(casingResultsListToSelect,firstCasingForBurstAndCollapse, depth,fluidDensity,collapseFactor);

            var selectedCasingDepthsWithoutAxialBestCost = SelectedCasingDepthsWithoutAxialBestCost(maxCasingDepthsWithoutAxial, firstCasingForBurstAndCollapse);

            var minLengthChecked = MinLengthCheck(selectedCasingDepthsWithoutAxialBestCost, minimalSectionLength);
            LengthCalc(minLengthChecked);
            //TensionCheck(minLengthChecked, tensionFactor);
            
            //LengthCalc(minLengthChecked);
            
            // // Dictionary with max depths for all possible casings above last(firstCasingForBurstAndCollapse)            
            // IDictionary<CasingDto, double> maxCasingDepthsForCollapseBiAxial = MaxCasingDepthsForCollapseBiAxial( casingResultsListToSelect, firstCasingForBurstAndCollapse, 
            //                          depth, fluidDensity, collapseFactor);

            // // Dictionary with key: id of casing value: total cost from ground to max depth regarding to collapse reduced by axial 
            // //IDictionary<int,double> maxCasingDepthsForCollapseBiAxialTotalCost = MaxCasingDepthsForCollapseBiAxialTotalCost(maxCasingDepthsForCollapseBiAxial);
            // // Sorted by cost keyValuePair List with key: id of casing value: total cost from ground to max depth regarding to collapse reduced by axial 
            // List<KeyValuePair<int,double>> maxCasingDepthsForCollapseBiAxialTotalCost = MaxCasingDepthsForCollapseBiAxialTotalCost(maxCasingDepthsForCollapseBiAxial);
            
            return minLengthChecked;
            // return maxCasingDepthsForCollapseBiAxialTotalCost;
        }

        public List<CasingDto> CalculateCasingDesign(InputData inputData)
        {
            var casingInventoryCollecion = GetCasingInventoryListByDiameter(inputData.Diameter);
            
            double burstRequiredResistance = BurstValue(inputData.PorePressure, inputData.BurstFactor); 

            double collapseRequiredResistance = CollapseValueAtShoe(inputData.FluidDensity, inputData.Depth, inputData.CollapseFactor);

            CasingDto firstCasingForBurstAndCollapse = FirstCasingForBurstAndCollapse(casingInventoryCollecion, burstRequiredResistance, collapseRequiredResistance, inputData.Depth);

            var casingResultsListToSelect = casingInventoryCollecion.Where(c => c.Burst >= burstRequiredResistance 
                                            && c.Collapse < firstCasingForBurstAndCollapse.Collapse // < firstCas... not to put first (burst&collapse OK) to list. Otherwise would be <=
                                            && c.Cost < firstCasingForBurstAndCollapse.Cost).OrderBy(c => c.Cost);
            
            var maxCasingDepthsWithoutAxial = MaxCasingDepthsWithoutAxial(casingResultsListToSelect,firstCasingForBurstAndCollapse, inputData.Depth, inputData.FluidDensity, inputData.CollapseFactor);

            var selectedCasingDepthsWithoutAxialBestCost = SelectedCasingDepthsWithoutAxialBestCost(maxCasingDepthsWithoutAxial, firstCasingForBurstAndCollapse);

            var minLengthChecked = MinLengthCheck(selectedCasingDepthsWithoutAxialBestCost, inputData.MinimalSectionLength);
            LengthCalc(minLengthChecked);

            return minLengthChecked;
        }







        //public List<CasingDto> SelectedCasingDepthsWithAxialBestCostRecalc(
        //     List<CasingDto> selectedCasings, double collapseFactor, double fluidDensity, double depth, double minimalSectionLength)
        // {
        //     double lengthOfCasingBelow;
        //     double maxCollapsePressure;
        //     double weightOfCasingBelow;
        //     double axialStressCausedByCasingBelow;
        //     double collapseReducedByAxial;
        //     double maxCollapsePressureReducedByAxial;
        //     double maxDepthWithAxial1;
        //     double maxDepthWithAxial2;

        //     ListCasingDto> selectedCasingDepthsWithAxialBestCostRecalc = new List<CasingDto>();
            
        //     //checking min seciton length
        //     // foreach (CasingDto casing in selectedCasings.Reverse<CasingDto>())
        //     // {
                
        //     // }


        //     // foreach (var casing in selectedCasings)
        //     // {
        //     //     maxCollapsePressure = casing.Collapse / collapseFactor;
        //     //     maxDepthWithAxial2 = maxCollapsePressure / (0.052*fluidDensity);
        //     //     do
        //     //     {   
        //     //         maxDepthWithAxial1 = maxDepthWithAxial2;
                    
        //     //         lengthOfCasingBelow = depth - maxDepthWithAxial1;
        //     //         weightOfCasingBelow = lengthOfCasingBelow * firstCasingForBurstAndCollapse.Weight;
        //     //         axialStressCausedByCasingBelow = weightOfCasingBelow / firstCasingForBurstAndCollapse.Area;

        //     //         collapseReducedByAxial = casing.Collapse * 
        //     //         ( 
        //     //             (Math.Sqrt(1- (0.75* (axialStressCausedByCasingBelow/casing.YieldStrength)*
        //     //                                 (axialStressCausedByCasingBelow/casing.YieldStrength) ))-
        //     //                                     0.5* (axialStressCausedByCasingBelow/casing.YieldStrength) 
        //     //             ) 
        //     //         );

        //     //         maxCollapsePressureReducedByAxial = collapseReducedByAxial / collapseFactor;
        //     //         maxDepthWithAxial2 = maxCollapsePressureReducedByAxial / (0.052*fluidDensity);

        //     //     }while (Math.Abs(maxDepthWithAxial2 - maxDepthWithAxial1)>5); // could be less, but be careful not to kill loop

        //     // }
        //     return selectedCasingDepthsWithAxialBestCostRecalc;
            

        // }


        // 
        // public IDictionary<CasingDto,double> MaxCasingDepthsForCollapseBiAxial(IOrderedEnumerable<CasingDto> casingResultsListToSelect, CasingDto firstCasingForBurstAndCollapse, 
        //                             double depth, double fluidDensity, double collapseFactor)
        // {
        //     double lengthOfCasingBelow;
        //     double maxCollapsePressure;
        //     double weightOfCasingBelow;
        //     double axialStressCausedByCasingBelow;
        //     double collapseReducedByAxial;
        //     double maxCollapsePressureReducedByAxial;
        //     double maxDepthWithAxial1;
        //     double maxDepthWithAxial2;

        //     Dictionary<CasingDto,double> maxCasingDepthsForCollapseBiAxial = new Dictionary<CasingDto, double>();

        //     foreach (var casing in casingResultsListToSelect)
        //     {
        //         maxCollapsePressure = casing.Collapse / collapseFactor;
        //         maxDepthWithAxial2 = maxCollapsePressure / (0.052*fluidDensity);
                
        //         do
        //         {   
        //             maxDepthWithAxial1 = maxDepthWithAxial2;
                    
        //             lengthOfCasingBelow = depth - maxDepthWithAxial1;
        //             weightOfCasingBelow = lengthOfCasingBelow * firstCasingForBurstAndCollapse.Weight;
        //             axialStressCausedByCasingBelow = weightOfCasingBelow / firstCasingForBurstAndCollapse.Area;

        //             collapseReducedByAxial = casing.Collapse * 
        //             ( 
        //                 (Math.Sqrt(1- (0.75* (axialStressCausedByCasingBelow/casing.YieldStrength)*
        //                                     (axialStressCausedByCasingBelow/casing.YieldStrength) ))-
        //                                         0.5* (axialStressCausedByCasingBelow/casing.YieldStrength) 
        //                 ) 
        //             );

        //             maxCollapsePressureReducedByAxial = collapseReducedByAxial / collapseFactor;
        //             maxDepthWithAxial2 = maxCollapsePressureReducedByAxial / (0.052*fluidDensity);

        //         }while (Math.Abs(maxDepthWithAxial2 - maxDepthWithAxial1)>5); // could be less, but be careful not to kill loop

        //         maxCasingDepthsForCollapseBiAxial.Add(casing, maxDepthWithAxial2);
        //     }

        //     return maxCasingDepthsForCollapseBiAxial;
        // }

        // //public IDictionary<int,double> MaxCasingDepthsForCollapseBiAxialTotalCost(IDictionary<CasingDto,double> maxCasingDepthsForCollapseBiAxial)
        // public List<KeyValuePair<int,double>> MaxCasingDepthsForCollapseBiAxialTotalCost(IDictionary<CasingDto,double> maxCasingDepthsForCollapseBiAxial)
        // {
        //     //Dictionary<int,double> maxCasingDepthsForCollapseBiAxialTotalCost = new Dictionary<int, double>();
        //     List<KeyValuePair<int,double>> maxCasingDepthsForCollapseBiAxialTotalCost = new List<KeyValuePair<int, double>>();
            
        //     foreach (var casing in maxCasingDepthsForCollapseBiAxial)
        //     {
        //         maxCasingDepthsForCollapseBiAxialTotalCost.Add(new KeyValuePair<int, double> (casing.Key.Id, (casing.Value * casing.Key.Cost)));
        //     }

        //     return maxCasingDepthsForCollapseBiAxialTotalCost.OrderBy(x => x.Value).ToList();
        // }

        // //Tuple
        // public IEnumerable<(CasingDto, double, double)> ResultsListWithDepthsAndCost(IDictionary<CasingDto,double> maxCasingDepthsForCollapseBiAxial, List<KeyValuePair<int,double>> maxCasingDepthsForCollapseBiAxialTotalCost)
        // {
        //     foreach (var casing in collection)
        //     {
                
        //     }
        // }

    }
}