using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SamuraiApp.Data;
using SamuraiApp.Domain;
using System.Linq;

namespace SamuraiTest
{


	[TestClass]
	public class SamuraiTests
	{
		private SamuraiContext _samuraiContext;

		[TestInitialize]
		public void Setup()
		{
			var options = new DbContextOptionsBuilder<SamuraiContext>()
			.UseInMemoryDatabase("caninsert")
			.Options;
			_samuraiContext = new SamuraiContext(options);
		}

		[TestCleanup]
		public void Cleanup()
		{
			_samuraiContext.Database.EnsureDeleted();
			_samuraiContext.Dispose();
		}

		[TestMethod]
		public void CanInsertSamurai()
		{
			var samurai = new Samurai();
			_samuraiContext.Add(samurai);
			Assert.AreEqual(EntityState.Added, _samuraiContext.Entry(samurai).State);
		}
	}
}
