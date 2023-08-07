using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NLog;
using SlotMachine.Application.Interfaces;
using SlotMachine.Domain.Models;
using SlotMachine.Domain.Services;

var builder = CreateHostBuilder(args);
var app = builder.Build();

LogManager.Setup().LoadConfigurationFromFile("nlog.config");

var game = app.Services.GetRequiredService<Game>();
game.Play();

await app.RunAsync();

IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureAppConfiguration((context, config) =>
        {
            config.SetBasePath(Directory.GetCurrentDirectory());
            config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            config.AddEnvironmentVariables();
        })
        .ConfigureServices((hostContext, services) =>
        {
            var configuration = hostContext.Configuration;
            services.Configure<SymbolSettings>(configuration.GetSection("SymbolSettings"));
            services.AddScoped<ISlotMachine, SlotMachine.Infrastructure.Services.SlotMachine>();
            services.AddScoped<Game>();
        });
