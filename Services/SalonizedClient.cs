using System.Web;
using HtmlAgilityPack;
using Strooware.SalonReviews.Common;

namespace Strooware.SalonReviews.Services;

public interface ISalonizedClient
{
   Task<IEnumerable<Review>> GetReviewsAsync(string salonId);
}

public class SalonizedClient(IHttpClientFactory clientFactory) : ISalonizedClient
{
   private readonly IHttpClientFactory clientFactory = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));

   public async Task<IEnumerable<Review>> GetReviewsAsync(string salonId)
   {
      Ensure.NotNullOrEmpty(salonId);
      var reviewPage1 = await GetReviewsPageAsync(salonId, 1).ConfigureAwait(false);
      var numPages = ParseNumPagesFromReviewPage(reviewPage1);

      var reviewPages = await Task.WhenAll(Enumerable.Range(2, numPages - 1)
         .Select(i => GetReviewsPageAsync(salonId, i)))
         .ConfigureAwait(false);

      var reviews = new List<Review>();
      reviews.AddRange(ParseReviewsFromReviewPage(reviewPage1));
      reviews.AddRange(reviewPages
         .Select(ParseReviewsFromReviewPage)
         .SelectMany(p => p));
      return reviews;
   }

   private async Task<HtmlDocument> GetReviewsPageAsync(string salonId, int page)
   {
      Ensure.GreaterOrEqualTo(page, 1);

      using var client = clientFactory.CreateClient();
      var url = $"https://{salonId}.salonized.com/reviews?page={page}";

      var doc = new HtmlDocument();
      var resp = await client.GetStringAsync(url).ConfigureAwait(false);
      doc.LoadHtml(resp);

      return doc;
   }

   private static int ParseNumPagesFromReviewPage(HtmlDocument doc)
   {
      var lastPageQuery = HttpUtility.ParseQueryString(doc.DocumentNode
         .SelectSingleNode("//*[@class=\"arrow\"][last()]/a")?
         .GetAttributeValue("href", "/reviews?page=1")
         .Split("?").Last() ?? "page=1");

      if (int.TryParse(lastPageQuery.Get("page"), out var numPages))
      {
         return numPages;
      }
      else
      {
         return 1;
      }
   }

   private static IEnumerable<Review> ParseReviewsFromReviewPage(HtmlDocument doc) => doc.DocumentNode?.SelectNodes("//*[@class=\"review\"]")?
     .Select(ParseReviewFromReviewNode) ?? [];

   private static Review ParseReviewFromReviewNode(HtmlNode node)
   {
      var numStars = node.SelectSingleNode("*[@class=\"review-header\"]/*[@class=\"stars\"]")
         .GetAttributeValue("title", 0);
      var description = node.SelectSingleNode("*[@class=\"review-body\"]")?
         .InnerText.Trim() ?? "Geen review achtergelaten";
      var author = node.SelectSingleNode("*[@class=\"review-details\"]/b")
         .InnerText.Trim();
      var reviewDate = node.SelectSingleNode("*[@class=\"review-details\"]/b/following-sibling::text()")
         .InnerText.Replace("\n-\n", "").Trim();

      return new Review(numStars, description, author, reviewDate);
   }
}