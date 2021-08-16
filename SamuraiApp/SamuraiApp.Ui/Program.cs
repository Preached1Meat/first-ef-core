using SamuraiApp.Data;
using SamuraiApp.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SamuraiApp.Ui
{
	class Program
	{
		private static SamuraiContext _context = new SamuraiContext();
		static void Main(string[] args)
		{
			_context.Database.EnsureCreated();
			//GetAllSamurais("Before Add");

			//AddSamurais("Shimada", "Okamato", "Kikuoichi", "Hayashida");
			var result = GetSamuraiByName("Shimada");

			if (result != null)
			{
				Console.WriteLine($"Found a Samurai with name {result.Name}");

				AppendSamuraiName(result, "San");

			}

			GetAllSamurais("After Add");

			Console.WriteLine("Press Any Key");
			Console.ReadLine();
		}
		static void AddSamurais(params string[] names)
		{
			var samurais = names
				.Select(n => new Samurai { Name = n });

			// EF detects Enitity object
			_context.AddRange(samurais);

			// Effectively the same as above
			//_context.Samurais.AddRange(samurais);

			_context.SaveChanges();
		}

		static void AddSamurai(string name)
		{
			var samurai = new Samurai { Name = name };
			_context.Samurais.Add(samurai);
			_context.SaveChanges();
		}

		static Samurai GetSamuraiByName(string name) =>
			_context.Samurais.FirstOrDefault(s => s.Name == name);

		static void AppendSamuraiName(Samurai samurai, string appendage)
		{
			samurai.Name += appendage;
			_context.SaveChanges();
		}

		static void GetAllSamurais(string text)
		{

			var samurais = _context.Samurais.ToList();
			Console.WriteLine($"{text}: Samurai count is {samurais.Count}");
			foreach (var samurai in samurais)
			{
				Console.WriteLine(samurai.Name);
			}
		}
	}
}
