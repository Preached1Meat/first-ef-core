﻿using SamuraiApp.Data;
using SamuraiApp.Domain;
using System;
using System.Linq;

namespace SamuraiApp.Ui
{
	class Program
	{
		private static SamuraiContext _context = new SamuraiContext();
		static void Main(string[] args)
		{
			_context.Database.EnsureCreated();
			GetSamurais("Before Add");
			AddSamurai();
			GetSamurais("After Add");

			Console.WriteLine("Press Any Key");
			Console.ReadLine();
		}


		static void AddSamurai()
		{
			var samurai = new Samurai { Name = "Julie" };
			_context.Samurais.Add(samurai);
			_context.SaveChanges();
		}

		static void GetSamurais(string text)
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
