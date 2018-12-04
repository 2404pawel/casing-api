using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CasingDesign.API.Entities;
using CasingDesign.API.Models;
using CasingDesign.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace CasingDesign.API.Controllers
{
    [Route("api")]
    [ApiController]
    public class CalculateVerticalController : ControllerBase
    {
        private ICalculateVerticalRepository _calculateVerticalRepository;
        public CalculateVerticalController(ICalculateVerticalRepository calculateVerticalRepository)
        {
            _calculateVerticalRepository = calculateVerticalRepository;
        }

        [HttpGet("/vertical")]
        public IActionResult CalculateCasingDesign(double diameter, double depth, double fluidDensity, 
                                    double porePressure, double burstFactor, double collapseFactor, 
                                    double tensionFactor, double minimalSectionLength )
        {
            var casing = _calculateVerticalRepository.CalculateCasingDesign(diameter,  depth,  fluidDensity, 
                                     porePressure,  burstFactor,  collapseFactor, 
                                     tensionFactor,  minimalSectionLength);
            
            //var results = Mapper.Map<IEnumerable<CasingDto>>(casingEntities); 

            return Ok(casing);
        }

    }
}