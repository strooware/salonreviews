namespace Strooware.SalonReviews.Infra;

public record Salon(string Id, string LastFetchedTime);
public record Review(string SalonId, int NumStars, string Description, string Author, string ReviewDate);
