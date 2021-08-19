using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SamuraiApp.Domain;
using System;

namespace SamuraiApp.Data
{
	public class SamuraiContext : DbContext
	{
		public DbSet<Samurai> Samurais { get; set; }
		public DbSet<Quote> Quotes { get; set; }
		public DbSet<Battle> Battles { get; set; }
		public DbSet<SamuraiBattleStat> SamuraiBattleStats { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			// Don't hardcode into context
			// For Demo purposes
			optionsBuilder
				.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=SamuraiAppData")
				.LogTo(Console.WriteLine, new[] { DbLoggerCategory.Database.Command.Name }, LogLevel.Information)
				.EnableSensitiveDataLogging(sensitiveDataLoggingEnabled: true);

			base.OnConfiguring(optionsBuilder);
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{

			modelBuilder.Entity<Samurai>()
				.HasMany(s => s.Battles)
				.WithMany(b => b.Samurais)
				.UsingEntity<BattleSamurai>(
				 bs => bs.HasOne<Battle>().WithMany(),
				 bs => bs.HasOne<Samurai>().WithMany())
				.ToTable("BattleSamurai")// set table name explictly
				.Property(bs => bs.DateJoined) // additional payload
				.HasDefaultValueSql("getdate()");


			// horse does not have a dbSet property in the context
			// so EF does not apply the multiples naming convention
			modelBuilder.Entity<Horse>().ToTable("Horses");


			// keyless entity, EF will never track entities marked with HasNoKey
			modelBuilder.Entity<SamuraiBattleStat>().HasNoKey().ToView("SamuraiBattleStats");

			base.OnModelCreating(modelBuilder);
		}
	}
}
