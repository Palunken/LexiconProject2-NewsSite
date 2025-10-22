using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using The_Post.Services;
using Microsoft.AspNetCore.Identity.UI.Services;
using The_Post.Data;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        // Load configuration from local.settings.json and environment variables
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection");

        // Register DbContext to connect to the SQL database
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddTransient<IEmailSender, EmailSender>();

        services.AddHttpClient();
    })
    .Build();

host.Run();

