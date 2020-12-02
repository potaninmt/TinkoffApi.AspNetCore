using Microsoft.EntityFrameworkCore;
using TinkoffApi.Data.Entities;

namespace TinkoffApi.DAL
{
    public class TinkoffApiContext : DbContext
    {
        public DbSet<LogEntity> Logs { get; set; }
        public DbSet<PortfolioHistory> PortfolioHistory { get; set; }
        public DbSet<OperationHistory> OperationsHistory { get; set; }
        public TinkoffApiContext(DbContextOptions<TinkoffApiContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OperationHistory>().OwnsOne(x => x.Commission);
            modelBuilder.Entity<OperationHistory>().OwnsOne(x => x.StopLoss);
            modelBuilder.Entity<OperationHistory>().OwnsOne(x => x.TakeProfit);
        }
    }
}
