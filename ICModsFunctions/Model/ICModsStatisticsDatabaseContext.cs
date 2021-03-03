using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace ICModsFunctions.Model
{
    public partial class ICModsStatisticsDatabaseContext : DbContext
    {
        public ICModsStatisticsDatabaseContext()
        {
        }

        public ICModsStatisticsDatabaseContext(DbContextOptions<ICModsStatisticsDatabaseContext> options)
            : base(options)
        {
        }

        public virtual DbSet<ModsDownloads> ModsDownloads { get; set; }
        public virtual DbSet<ModsDownloadsTest> ModsDownloadsTest { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(Environment.GetEnvironmentVariable("sqldb_connection"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ModsDownloads>(entity =>
            {
                entity.HasKey(e => e.StatId)
                    .HasName("PK__mods_dow__B8A525608691F6DD");

                entity.ToTable("mods_downloads");

                entity.Property(e => e.StatId).HasColumnName("stat_id");

                entity.Property(e => e.Downloads).HasColumnName("downloads");

                entity.Property(e => e.ModId).HasColumnName("mod_id");

                entity.Property(e => e.StatTime)
                    .HasColumnName("stat_time")
                    .HasColumnType("smalldatetime");
            });

            modelBuilder.Entity<ModsDownloadsTest>(entity =>
            {
                entity.HasKey(e => e.StatId)
                    .HasName("PK__mods_dow__B8A52560B69B70D3");

                entity.ToTable("mods_downloads_test");

                entity.Property(e => e.StatId).HasColumnName("stat_id");

                entity.Property(e => e.Downloads).HasColumnName("downloads");

                entity.Property(e => e.ModId).HasColumnName("mod_id");

                entity.Property(e => e.StatTime)
                    .HasColumnName("stat_time")
                    .HasColumnType("smalldatetime");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
