using System;
using kafkacommon;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using UserModule.Interfaces;
using UserModuleCommon;


namespace Application
{
	class Program
	{
		public static void Main(string[] args)
		{
			IHost host = CreateHostBuilder(args).Build();
			host.Run();
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); })
				.UseConsoleLifetime();
	}
}