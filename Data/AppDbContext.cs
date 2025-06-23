using CampusLink_Application.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace CampusLink_Application.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Student> Students { get; set; }
    }
}
