using Strooware.SalonReviews.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpClient();
builder.Services.AddScoped<ISalonizedClient, SalonizedClient>();

var app = builder.Build();

app.MapGet("/salon/{salonId}/reviews.json", async (ISalonizedClient client, string salonId) =>
{
   var reviews = await client.GetReviewsAsync(salonId).ConfigureAwait(false);
   return Results.Ok(reviews);
});

app.Run();
