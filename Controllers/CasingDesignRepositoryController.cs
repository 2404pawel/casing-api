using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using CasingDesign.API.Models;
using CasingDesign.API.Entities;
using CasingDesign.API.Services;
using Microsoft.EntityFrameworkCore;

namespace CasingDesign.API.Controllers
{ //Controller only to present all datas from db (if needed)
    [Route("api")]
    [ApiController]
    public class CasingDesignRepositoryController : ControllerBase
    {
        //inject the repository
        private ICasingDesignRepository _casingDesignRepository;
        public CasingDesignRepositoryController(ICasingDesignRepository casingDesignRepository)
        {
            _casingDesignRepository = casingDesignRepository;
        }

        [HttpGet("/casinginventory")]
        public async Task<IActionResult> GetAllCasings() 
        {
            var casingEntities = await _casingDesignRepository.GetAllCasings();
            var results = Mapper.Map<IEnumerable<CasingDto>>(casingEntities); //not to reveal DB casing structure, created DTO

            return Ok(results);
        }

        [HttpGet("/casinginventory/{diameter}")]
        public async Task<IActionResult> GetCasingsByDiameter(double diameter)
        {
            var casingEntities = await _casingDesignRepository.GetCasingsByDiameter(diameter);
            var results = Mapper.Map<IEnumerable<CasingDto>>(casingEntities); //not to reveal DB casing structure, created DTO

            return Ok(results);
        }







        // [HttpGet("/TestCALC")]
        // public double GetBurstValue(double burstPressure, double burstFactor)
        // {
        //     var CalcClass = new CalculateVertical();
        //     var burst = CalcClass.BurstValue(burstPressure, burstFactor);
        //     return burst;
        // }

        // [HttpGet("/burstcasing")]
        // public async Task<IActionResult> GetCasingForBurst(double burstPressure, double burstFactor)
        // {
        //     var CalcClass = new CalculateVertical();
        //     var burst = CalcClass.BurstValue(burstPressure, burstFactor);

        //     var result = await _context.CasingInventory.Where(c => c.Burst >= burst).ToListAsync();

        //     return Ok(result);
        // }

        // [HttpGet("/calculatevertical")]
        // public IActionResult CalculateCasingDesign(int casingDiameter, double depth, double fluidDensity, 
        //         double porePressure, double burstFactor, double collapseFactor, double tensionFactor, double minmalSectionLength )
        // {//casingDiameter - 1=5cali, 2=5.5calc itd...
        //     var casingInventoryList = _context.CasingInventory.ToList(); //all available casings

        //     //var burst = 
        // }
        


    }
}