
using APP;
using APP.Services;
using BLL.Enums;
using BLL.Interfaces;
using BLL.Model;
using BLL.Services;
using DLL.Repositories;
using DLLProvider;
using DSharpPlus;
using System.Diagnostics;

namespace REST {
    public class Program {
        public static void Main(string[] args) {
            var builder = WebApplication.CreateBuilder(args);

            // Initialise
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            var database = (DatabaseSaveType)int.Parse(builder.Configuration.GetConnectionString("DataLayer")!);
            var botToken = builder.Configuration.GetConnectionString("BotToken");
            var prefix = builder.Configuration.GetConnectionString("Prefix");
            DataRepositories repositories = DataRepositoryFactory.GetDataRepositories(connectionString!, database);

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddOpenApi();

            // BindingAddress logging.
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            builder.Logging.AddDebug();

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi.
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddSingleton<IDataRepository>(provider => repositories.dataRepository);
            builder.Services.AddSingleton<DiscordService>(provider => {
                var config = new Config {
                    Token = botToken,  // Provide your Discord token here
                    ConnectionString = connectionString,
                    DatabaseType = database,
                    Prefix = prefix // Set the command prefix if necessary
                };
                var cache = new CacheData(); // You can also provide a proper cache implementation here
                return new DiscordService(config, cache);
            });

            // Build the web app.
            var app = builder.Build();
            app.UseDefaultFiles();
            app.MapStaticAssets();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment()) {
                app.UseSwagger();
                app.UseSwaggerUI();
                app.MapOpenApi();
            }

            app.UseAuthorization();
            app.MapControllers();
            OpenBrowser("http://localhost:5119/swagger");
            app.Run();
        }

        private static void OpenBrowser(string url) {
            try {
                Process.Start(new ProcessStartInfo {
                    FileName = url,
                    UseShellExecute = true
                });
            } catch (Exception ex) {
                Console.WriteLine($"Unable to open browser: {ex.Message}");
            }
        }
    }
}
