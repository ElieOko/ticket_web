using Microsoft.EntityFrameworkCore;
using SCustomers.Entities;

namespace SCustomers.Data
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            :base(options)
        {

        }
        public DbSet<User> Users { get; set; }
        public DbSet<UserLog> UserLogs { get; set; }
        public DbSet<UserPermission> UserPermissions { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<Transfer> Transfers { get; set; }
        public DbSet<TransferType> TransferTypes { get; set; }
        public DbSet<TransferStatus> TransferStatuses { get; set; }
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<OrderNumber> OrderNumbers { get; set; }
        public DbSet<Interval> Intervals { get; set; }
        public DbSet<Counter> Counters { get; set; }
        public DbSet<Call> Calls { get; set; }
        public DbSet<Card> Cards { get; set; }
        public DbSet<Title> Titles { get; set; }
    }
}
