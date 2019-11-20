using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

using southosting.Models;
using southosting.Data;

namespace southosting.Data
{
    public class SouthostingContext : IdentityDbContext<SouthostingUser>
    {
        public SouthostingContext (DbContextOptions<SouthostingContext> options)
            : base(options)
        {
        }
        public DbSet<Upload> Upload { get; set; }
        public DbSet<Advert> Advert { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
