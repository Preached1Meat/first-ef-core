using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SamuraiApp.Data;
using SamuraiApp.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SamuraiApp.Ui
{
	class Program
	{
		// not used anymore, due to using the di container to inject into the main app
		private static readonly SamuraiContext _context = new();
		private static readonly SamuraiContextNoTracking _samuraiContextNoTracking = new();
		static void Main(string[] args)
		{

			var services = new ServiceCollection();
			ConfigureServices(services);

			ServiceProvider serviceProvider = services.BuildServiceProvider();
			SamuraiApp app = serviceProvider.GetService<SamuraiApp>();

			try
			{	
				app.Run();
			}
			catch (Exception ex )
			{

				app.HandleError(ex);
			}
			finally
			{
				// do nothing
			}
			
		}

		private static void ConfigureServices(IServiceCollection services)
		{
			services.AddLogging(configure => configure.AddConsole())
				.AddTransient<SamuraiApp>();

			services.AddDbContext<SamuraiContext>();
		}
	}
}
