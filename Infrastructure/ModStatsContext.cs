using ApplicationCore.Model.Cosmos;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure
{
	public class ModStatsContext : DbContext
    {
	    public DbSet<ModStatItem> ModStatItems { get; set; }

	    public ModStatsContext(DbContextOptions<ModStatsContext> options)
		    : base(options)
	    {
	    }

	    protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			#region Container

			modelBuilder.Entity<ModStatItem>()
				.ToContainer("Main")
				.HasNoDiscriminator()
				.HasPartitionKey(nameof(ModStatItem.ModId))
				.HasNoKey();

			#endregion
		}
	}
}
