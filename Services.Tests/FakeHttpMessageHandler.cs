using System.Net;

namespace Strooware.SalonReviews.Services.Tests;

public class FakeHttpMessageHandler : HttpMessageHandler
{
   protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
   {
      var fileName = request.RequestUri!.Host.Split(".").First();
      var contents = await File.ReadAllTextAsync($"./TestPages/{fileName}.html", cancellationToken);

      return new(HttpStatusCode.OK)
      {
         Content = new StringContent(contents)
      };
   }
}