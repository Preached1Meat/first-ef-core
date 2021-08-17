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
			GetAllSamurais("Before Add");

			throw new ApplicationException("crit error");

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

			//AddQuoteToSamuraiWhileNotTracked("stijn", "untracked quote");

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

		private void AddQuoteToSamuraiWhileNotTracked(string samuraiName, string quote)
		{
			var samurai = _context.Samurais.Find(samuraiName);
			samurai.Quotes.Add(new Quote
			{
				Text = quote
			});

			using (var newContext = new SamuraiContext())
			{
				newContext.Samurais.Update(samurai);
				newContext.SaveChanges();
			}
		}


		public void HandleError(Exception ex)
		{
			logger.LogError($"Application Encountered error: { ex.Message}");
		}
	}
}
