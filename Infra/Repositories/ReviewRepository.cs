using Dapper;
using Strooware.SalonReviews.Common;

namespace Strooware.SalonReviews.Infra.Repositories;

public interface IReviewRepository
{
   Task AddReviewAsync(Review review);
   Task<IEnumerable<Review>> GetReviewsAsync(string salonId);
}

public class ReviewRepository(IContextFactory contextFactory) : IReviewRepository
{
   private readonly IContextFactory contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));

   public async Task AddReviewAsync(Review review)
   {
      Ensure.NotNull(review);

      using var context = contextFactory.Create();
      await context.QueryAsync("INSERT INTO `reviews`(`salonId`, `numStars` ,`description` ,`author` ,`reviewDate`) VALUES(@SalonId, @NumStars, @Description, @Author, @ReviewDate)", review);
   }

   public async Task<IEnumerable<Review>> GetReviewsAsync(string salonId)
   {
      Ensure.NotNullOrEmpty(salonId);

      using var context = contextFactory.Create();
      return await context.QueryAsync<Review>("SELECT * FROM `reviews` WHERE salonId = @salonId", new { salonId });
   }
}