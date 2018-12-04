using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using CasingDesign.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace CasingDesign.API.Services
{
    public class CasingDesignRepository : ICasingDesignRepository
    {
        private CasingDesignContext _casingDesignContext;
        public CasingDesignRepository(CasingDesignContext casingDesignContext)
        {
            _casingDesignContext = casingDesignContext;
        }

        public bool CasingExists(int id)
        {
            return _casingDesignContext.CasingInventory.Any(c => c.Id == id);
        }

        public async Task<IEnumerable<Casing>> GetAllCasings()
        {
            return await _casingDesignContext.CasingInventory.ToListAsync();
        }
        public async Task<IEnumerable<Casing>> GetCasingsByDiameter(double diameter)
        {
            return await _casingDesignContext.CasingInventory.Where(c => c.ExternalDiameter == diameter).ToListAsync();
        }

    }
}