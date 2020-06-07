using Microsoft.EntityFrameworkCore;
using TestCoreApp.Api.Models;

namespace TestCoreApp.Api.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<value> Values { get; set; }
    }
}