using JohnnyDemo.Repository.DomainModels;
using JohnnyDemo.Repository.Models;
using Microsoft.EntityFrameworkCore;

namespace JohnnyDemo.Repository.Context
{
    public class JohnnyDemoContext : DbContext
    {
        public JohnnyDemoContext() : base() { }
        public JohnnyDemoContext(DbContextOptions<JohnnyDemoContext> options) : base(options) { }
        internal virtual DbSet<Customer> Customers { get; set; }
        internal virtual DbSet<Order> Orders { get; set; }
    }
}
