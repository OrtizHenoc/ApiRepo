using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Client> Clients { get; set; }
    public DbSet<Account> Accounts { get; set; }
    public DbSet<Transaction> Transactions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>()
                    .Property(a => a.Id)
                    .ValueGeneratedOnAdd();
        modelBuilder.Entity<Account>()
            .HasOne(a => a.Client)
            .WithMany()
            .HasForeignKey(a => a.IdClient)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Transaction>()
        .Property(t => t.Id)
        .ValueGeneratedOnAdd();

        modelBuilder.Entity<Transaction>()
            .HasOne(a => a.Account)
            .WithMany()
            .HasForeignKey(a => a.IdAccount)
            .OnDelete(DeleteBehavior.Cascade);

        base.OnModelCreating(modelBuilder);

        
    }
     
}