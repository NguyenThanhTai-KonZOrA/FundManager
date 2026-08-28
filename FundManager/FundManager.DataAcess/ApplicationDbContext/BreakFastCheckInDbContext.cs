using FundManager.DataAccess.ExternalEntityModels;
using Microsoft.EntityFrameworkCore;

namespace FundManager.DataAccess.ApplicationDbContext
{
    public class BreakFastCheckInDbContext : DbContext
    {
        public BreakFastCheckInDbContext(DbContextOptions<BreakFastCheckInDbContext> options) : base(options)
        {
        }

        // These tables are managed outside EF migrations (pre-existing schema)
        public DbSet<ReservationSnapshot> ReservationSnapshots { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ReservationSnapshot>().ToTable("BCI_ReservationSnapshot");
        }
    }
}