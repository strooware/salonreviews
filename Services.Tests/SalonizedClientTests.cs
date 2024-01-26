using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Strooware.SalonReviews.Services.Tests;

public class SalonizedClientTests
{
	private readonly IHttpClientFactory clientFactoryMock;
	private readonly SalonizedClient client;

	public SalonizedClientTests()
	{
		clientFactoryMock = Substitute.For<IHttpClientFactory>();
		clientFactoryMock.CreateClient().Returns(p => new HttpClient(new FakeHttpMessageHandler()));
		client = new SalonizedClient(clientFactoryMock);
	}

	[Theory]
	[InlineData("no_reviews_no_pages", 0)]
	[InlineData("single_review_no_pages", 1)]
	[InlineData("ten_reviews_no_pages", 10)]
	[InlineData("two_reviews_two_pages", 2)]
	public async Task GetReviewsAsync_GetReviewsAsync(string salonId, int reviewCount)
	{
		// Arrange
		// nothing to do..

		// Act
		var result = await client.GetReviewsAsync(salonId);

		// Assert
		result.Should().HaveCount(reviewCount);
	}
}