using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;


namespace SoftPets.Models
{
    public class AppDbContext : DbContext
    {
        
        public AppDbContext() : base("ConexionLocal") { }
        public DbSet<Rol> Roles { get; set; }     
    }
}