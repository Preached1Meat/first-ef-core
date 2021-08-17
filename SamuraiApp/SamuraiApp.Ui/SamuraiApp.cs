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
			ProjectSomeProperties();

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
			
			logger.Log(LogLevel.Information,$"modified enity is a {modified.Metadata.Name}");
		}

		public void HandleError(Exception ex)
		{
			logger.LogError($"Application Encountered error: { ex.Message}");
		}


	}
}
