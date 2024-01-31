using Dapper;
using FluentResults;
using Microsoft.Data.Sqlite;
using Strooware.SalonReviews.Common;

namespace Strooware.SalonReviews.Infra.Repositories;

public interface ISalonRepository
{
   Task<Result<Salon>> AddSalonAsync(Salon Salon);
   Task<Result<Salon>> FindSalonAsync(string id);
}

public class SalonRepository(IContextFactory contextFactory) : ISalonRepository
{
   private readonly IContextFactory contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));

   public async Task<Result<Salon>> AddSalonAsync(Salon salon)
   {
      Ensure.NotNull(salon);

      using var context = contextFactory.Create();
      await context.QueryAsync("INSERT INTO `salons`(`id`, `lastFetchedTime`) VALUES(@Id, @LastFetchedTime)", salon);
      return salon;
   }

   public async Task<Result<Salon>> FindSalonAsync(string id)
   {
      Ensure.NotNullOrEmpty(id);

      using var context = contextFactory.Create();
      var salon = await context.QuerySingleOrDefaultAsync<Salon>("SELECT * FROM `salons` WHERE `id` = @id", new { id });

      return salon is null
         ? Result.Fail($"Unable to find salon for `{id}`")
         : Result.Ok(salon);
   }
}