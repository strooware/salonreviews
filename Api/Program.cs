using FluentMigrator.Runner;
using Strooware.SalonReviews.Infra;
using Strooware.SalonReviews.Infra.Migrations;
using Strooware.SalonReviews.Infra.Repositories;
using Strooware.SalonReviews.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
   .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
   .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);

builder.Services
// {{{ Setup Migrations
   .AddFluentMigratorCore()
   .ConfigureRunner(p => p
      .AddSQLite()
      .WithGlobalConnectionString(p => p.GetRequiredService<IContextFactory>().ConnectionString)
      .ScanIn(typeof(Migration001_Initial).Assembly).For.Migrations())
// }}}

// {{{ Setup Logging
   .AddLogging(p => p
      .AddConsole()
      .AddFluentMigratorConsole());
// }}}

// {{{ Database
builder.Services.AddTransient<IContextFactory, ContextFactory>();
builder.Services.AddTransient<ISalonRepository, SalonRepository>();
builder.Services.AddTransient<IReviewRepository, ReviewRepository>();
// }}}

builder.Services.AddHttpClient();
builder.Services.AddScoped<ISalonizedClient, SalonizedClient>();

var app = builder.Build();

// {{{ Startup
using var scope = app.Services.CreateScope();

scope.ServiceProvider.GetRequiredService<IMigrationRunner>()
   .MigrateUp();
// }}}

app.MapGet("/salon/{salonId}/reviews.json", async (ISalonRepository salonRepository, IReviewRepository reviewRepository, ISalonizedClient client, string salonId) =>
{
   var reviews = await client.GetReviewsAsync(salonId);

   var salon = await salonRepository.FindSalonAsync(salonId);
   if (salon.IsFailed)
   {
      salon = await salonRepository.AddSalonAsync(new Salon(salonId, DateTime.UtcNow.ToString()));
   }

   foreach (var review in reviews)
   {
      await reviewRepository.AddReviewAsync(new Strooware.SalonReviews.Infra.Review(
         salonId,
         review.NumStars,
         review.Description,
         review.Author,
         review.ReviewDate));
   }


   return Results.Ok(reviews);
});

app.Run();
