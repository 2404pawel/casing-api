using System;
using Microsoft.EntityFrameworkCore;

namespace CasingDesign.API.Entities

{
    public class CasingDesignContext : DbContext
    {
        public CasingDesignContext(DbContextOptions<CasingDesignContext> options)
            : base(options){}
        
        public DbSet<Casing> CasingInventory { get; set; }
    }
}