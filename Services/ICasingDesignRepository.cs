using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CasingDesign.API.Entities;

namespace CasingDesign.API.Services
{
    public interface ICasingDesignRepository
    {
        bool CasingExists(int id);
        Task<IEnumerable<Casing>> GetAllCasings();
        Task<IEnumerable<Casing>> GetCasingsByDiameter(double diameter);

    }
}