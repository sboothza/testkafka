using System.Data.Common;
using DbCommon;
using Npgsql;
using UserModule.Interfaces;

namespace Application;

public class Startup
{
	public IConfiguration Configuration { get; }

	public Startup(IConfiguration configuration)
	{
		Configuration = configuration;
	}

	public void ConfigureServices(IServiceCollection services)
	{
		services.AddControllers();
		DbProviderFactories.RegisterFactory("Npgsql", NpgsqlFactory.Instance);
		services.Configure<DatabaseConfig>(Configuration.GetSection("Databases"));
		services.AddSingleton<IUserRepository, UserRepository>();
	}

	public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime lifetime)
	{
		if (env.IsDevelopment())
		{
			app.UseDeveloperExceptionPage();
		}

		app.UseRouting();
		// app.UseSwagger();
		app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
	}
}