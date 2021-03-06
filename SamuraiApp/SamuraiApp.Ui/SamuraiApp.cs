using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SamuraiApp.Data;
using SamuraiApp.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamuraiApp.Ui
{
	public class SamuraiApp
	{
		private readonly ILogger<SamuraiApp> logger;
		private readonly SamuraiContext _context;

		public SamuraiApp(ILogger<SamuraiApp> logger, SamuraiContext samuraiContext)
		{
			this.logger = logger;
			this._context = samuraiContext;
		}

		public void Run()
		{
			_context.Database.EnsureCreated();
			//GetAllSamurais("Before Add");
			//AddSamurais("Shimada", "Okamato", "Kikuoichi", "Hayashida");
			//var result = GetSamuraiByName("Shimada");

			//if (result != null)
			//{
			//	Console.WriteLine($"Found a Samurai with name {result.Name}");
			//	AppendSamuraiName(result, "San");

			//}

			//GetAllSamurais("After Add");


			//InsertSamuraiWithQuotes("stijn 2 ", new List<string>()
			//{
			//	"hello world",
			//	"well hello there"
			//});

			//AddQuoteToSamuraiWhileTracked("stijn", "nog een quote");

			//AddQuoteToSamuraiWhileNotTracked("stijn", "untracked quote using attach", true);

			//EagerLoadingWithQuotes();

			//ProjectSomeProperties();

			//LazyLoadQuotes();

			//FilteringWithRelatedData();

			//ModifyingRelatedDataWhenTracked();

			//ModifyingRelatedDataWhenNotTracked();

			//AddNewSamuraiToExistingBattle();

			//ReturnBattleWithSamurais();

			//AddAllSamuraisToBattlesFail();

			//AddAllSamuraisToBattlesEager();

			//RemoveSamuraiFromABattle();

			//WillNotRemoveSamuraiFromBattle();

			//RemoveSamuraiFromABattleExplicit();

			//AddnewSamuraiWithHorse();

			//AddNewHorseToSamraiUsingId();

			//ReplaceAHorse();

			//GetSamuraiWithHorse();

			GetHorsesWithSamurai();

			Console.WriteLine("Press Any Key");
			Console.ReadLine();
		}

		private void AddSamurais(params string[] names)
		{
			var samurais = names
				.Select(n => new Samurai { Name = n });

			// EF detects Enitity object
			_context.AddRange(samurais);

			// Effectively the same as above
			//_context.Samurais.AddRange(samurais);

			_context.SaveChanges();
		}

		private void AddSamurai(string name)
		{
			var samurai = new Samurai { Name = name };
			_context.Samurais.Add(samurai);
			_context.SaveChanges();
		}

		private Samurai GetSamuraiByName(string name) =>
			_context.Samurais.FirstOrDefault(s => s.Name == name);

		private void AppendSamuraiName(Samurai samurai, string appendage)
		{
			samurai.Name += appendage;
			_context.SaveChanges();
		}

		private void GetAllSamurais(string text)
		{

			var samurais = _context.Samurais.ToList();
			Console.WriteLine($"{text}: Samurai count is {samurais.Count}");
			foreach (var samurai in samurais)
			{
				Console.WriteLine(samurai.Name);
			}
		}

		private void InsertSamuraiWithQuotes(string samuraiName, List<string> quotes)
		{
			var samurai = new Samurai
			{
				Name = samuraiName,
				Quotes = quotes
					.Select(q => new Quote { Text = q })
					.ToList()
			};

			_context.Add(samurai);
			_context.SaveChanges();
		}

		private void AddQuoteToSamuraiWhileTracked(string samuraiName, string quote)
		{
			GetSamuraiByName(samuraiName)
				?.Quotes.Add(new Quote
				{
					Text = quote
				});
			_context.SaveChanges();
		}

		private void AddQuoteToSamuraiWhileNotTracked(string samuraiName, string quote, bool useAttach = false)
		{
			var samurai = GetSamuraiByName(samuraiName);
			samurai.Quotes.Add(new Quote
			{
				Text = quote
			});

			using (var newContext = new SamuraiContext())
			{
				if (useAttach)
				{
					newContext.Samurais.Attach(samurai);
				}
				else
				{
					newContext.Samurais.Update(samurai);
				}
				newContext.SaveChanges();
			}
		}

		private void EagerLoadingWithQuotes()
		{
			// default include, executes a single query
			var samuraiWithQuotes = _context.Samurais
				.Include(s => s.Quotes)
				.ToList();

			// Splitquery - can improve performance
			var splitQuery = _context.Samurais
					.AsSplitQuery()
					.Include(s => s.Quotes)
					.ToList();

			// filter on Include
			var filteredInclude = _context.Samurais
				.Include(s => s.Quotes.Where(q => q.Text.Contains("hello")))
				.ToList();

			// get one samure , include the first quote
			var singleSamuraiQuotes = _context.Samurais
				.Where(s => s.Name.Contains("stijn"))
				.Include(s => s.Quotes.FirstOrDefault());

		}

		private void ProjectSomeProperties()
		{
			// anonymous objects are not tracked by the context
			var somePropsWithQuotes = _context.Samurais
				.Select(s => new { s.Id, s.Name, NumberOfQuotes = s.Quotes.Count })
				.ToList();

			// anonymous objects containing a property which is a registered entity 
			// will be tracked by the context
			var samuraiAndQuotes = _context.Samurais
				.Select(s => new
				{
					Samurai = s,
					HelloQuotes = s.Quotes
					.Where(q => q.Text.Contains("hello"))
				})
				.ToList();

			// list of new anonymous objects with a samurai entity as property
			var firstSamurai = samuraiAndQuotes[0].Samurai.Name += " The Happiest ";


			// context recognizes the samurai entity of the anomymous object
			// returned by the projection query
			// and tracks when changes are made
			var modified = _context.ChangeTracker.Entries()
				.Where(e => e.State == EntityState.Modified)
				.FirstOrDefault();

			logger.Log(LogLevel.Information, $"modified enity is a {modified.Metadata.Name}");
		}

		public void ExplicitLoadQuotes()
		{
			// make sure a horse is in DB
			_context.Set<Horse>().Add(new Horse { SamuraiId = 12, Name = "mr.horse" });
			_context.SaveChanges();

			_context.ChangeTracker.Clear();

			var samurai = _context.Samurais.Find(12);
			_context.Entry(samurai).Collection(s => s.Quotes).Load();
			_context.Entry(samurai).Reference(s => s.Horse).Load();
		}

		public void LazyLoadQuotes()
		{
			var samurai = _context.Samurais.Find(12);

			logger.Log(LogLevel.Information, $"LazyLoadQuotes: no LL enabled and no explicit loading count is : {samurai.Quotes.Count}");

			_context.Entry(samurai).Collection(s => s.Quotes).Load();

			logger.Log(LogLevel.Information, $"LazyLoadQuotes: no LL enabled and with explicit loading count is : {samurai.Quotes.Count}");

			// Lazy loading with proxies
			// must be configured on context 
			// EF Core will then enable lazy loading for any navigation property that can be overridden
			// that is, it must be virtual and on a class that can be inherited from
		}

		private void FilteringWithRelatedData()
		{
			// subquery on quotes, quotes not actually loaded
			var samurais = _context.Samurais.Where(s =>
				s.Quotes.Any(q => q.Text.Contains("hello")))
				.ToList();

			// eager loaded
			var samuraisWithquotesLoaded = _context.Samurais
				.Include(s => s.Quotes)
				.Where(s => s.Quotes.Any(q => q.Text.Contains("hello")))
				.ToList();

		}

		private void ModifyingRelatedDataWhenTracked()
		{
			var samuraisWithquotesLoaded = _context.Samurais
				.Include(s => s.Quotes)
				.FirstOrDefault(s => s.Id == 1);

			samuraisWithquotesLoaded.Quotes[0].Text = "Did you here that";
			_context.SaveChanges();

		}

		private void ModifyingRelatedDataWhenNotTracked()
		{
			var samuraisWithquotesLoaded = _context.Samurais
				.Include(s => s.Quotes)
				.FirstOrDefault(s => s.Id == 12);

			var quote = samuraisWithquotesLoaded.Quotes[0];
			quote.Text += "Did you here that agains";


			using (var newContext = new SamuraiContext())
			{
				//// new context, quotes are not yet tracked
				//// updating single quote will update all quotes beloning to samurai id from quote enitity
				//newContext.Quotes.Update(quote);

				// use entry + state to change single quote
				// without updating all other quotes 
				newContext.Entry(quote).State = EntityState.Modified;
				newContext.SaveChanges();
			}
		}

		private void AddNewSamuraiToExistingBattle()
		{
			var battle = _context.Battles.FirstOrDefault();
			battle.Samurais.Add(new Samurai { Name = " Rookie Samurai" });
			_context.SaveChanges();
		}

		public void ReturnBattleWithSamurais()
		{
			var battle = _context.Battles.Include(b => b.Samurais).FirstOrDefault();
		}

		public void ReturnBattlesWithSamurais()
		{
			var battles = _context.Battles.Include(b => b.Samurais).ToList();
		}

		private void AddAllSamuraisToBattlesFail()
		{

			var allBattles = _context.Battles.ToList();
			var allSamurais = _context.Samurais.ToList();

			// trying to add all samurai to all battles
			// if duplicate key exists (composite key) this will throw PK constraint violation
			foreach (var battle in allBattles)
			{
				battle.Samurais.AddRange(allSamurais);
			}
			_context.SaveChanges();
		}

		private void AddAllSamuraisToBattlesEager()
		{

			// by including either one of the relations, ef core will figure out the duplicates
			// on the join table
			// this has performance implications
			var allBattles = _context.Battles.ToList();
			var allSamurais = _context.Samurais.Include(b => b.Battles).ToList();

			foreach (var battle in allBattles)
			{
				battle.Samurais.AddRange(allSamurais);
			}
			_context.SaveChanges();
		}


		private void RemoveSamuraiFromABattle()
		{
			//query makes context track samurais for given battle
			var battleWithSamurai = _context.Battles
				.Include(b => b.Samurais.Where(s => s.Id == 12))
				.Single(s => s.BattleId == 1);

			var samurai = battleWithSamurai.Samurais[0];
			battleWithSamurai.Samurais.Remove(samurai);
			_context.SaveChanges();
		}

		private void WillNotRemoveSamuraiFromBattle()
		{
			// the relation is not tracked by the context;
			var battle = _context.Battles.Find(1);
			var samurai = _context.Samurais.Find(12);

			// here battle has no samurais
			// so context will not remove
			battle.Samurais.Remove(samurai);
			_context.SaveChanges();
		}

		private void RemoveSamuraiFromABattleExplicit()
		{
			// set return a DBSet, even if no DBset property in our context  
			var battleSamurai = _context.Set<BattleSamurai>()
				.SingleOrDefault(bs => bs.BattleId == 1 && bs.SamuraiId == 12);

			if (battleSamurai == null)
			{
				return;
			}

			//_context.Remove(battleSamurai);
			battleSamurai.DateJoined = DateTime.Now;
			_context.SaveChanges();
		}

		private void AddnewSamuraiWithHorse()
		{
			var samurai = new Samurai { Name = "Jina Ujichika" };
			samurai.Horse = new Horse { Name = "Silver" };
			_context.Add(samurai);
			_context.SaveChanges();
		}

		private void AddNewHorseToSamraiUsingId()
		{
			var horse = new Horse { Name = "Silver", SamuraiId = 2 };
			_context.Add(horse);
			_context.SaveChanges();
		}

		private void ReplaceAHorse()
		{
			var samurai = _context.Samurais.Include(s => s.Horse)
				.FirstOrDefault(s => s.Horse != null);

			// replacing the horse, EF will delete the record in the DB first
			// constraints don't allow the horse to exist without a 
			samurai.Horse = new Horse { Name = "Newer Horse" };
			_context.SaveChanges();


			// here EF will just do an insert, when changing the owner of the horse
			// who already has a horse, this will result in duplicate record due 
			// to the unique index constraint
			var horse = _context.Set<Horse>()
				.FirstOrDefault(h => h.Name == "Newer Horse");
			horse.SamuraiId = 2;
			_context.SaveChanges();

		}

		
		
		public void GetSamuraiWithHorse()
		{
			var samurais = _context.Samurais.Include(s => s.Horse).ToList();
		}

	
		public void GetHorsesWithSamurai()
		{
			// horse has no navigation property to samurai 
			// just the samuraiID
			var horse = _context.Set<Horse>().Find(3);

			var horseWithSamurai = _context.Samurais
				.Include(s => s.Horse)
				.FirstOrDefault(s => s.Horse.Id  == 3);
		}

		public void HandleError(Exception ex)
		{
			logger.LogError(ex.InnerException, $"Application Encountered error: { ex.Message}");
		}




	}
}
