using FluentAssertions;
using Xunit;

namespace Strooware.SalonReviews.Common.Tests;

public class EnsureTests
{
   [Fact]
   public void NotNullOrEmpty_WithEmptyString_ShouldThrow()
   {
      // Arrange
      // nothing to do..

      // Act
      var act = () => Ensure.NotNullOrEmpty(string.Empty);

      // Assert
      act.Should().Throw<ArgumentNullException>();
   }

   [Fact]
   public void NotNullOrEmpty_WithRandomString_ShouldNotThrow()
   {
      // Arrange
      // nothing to do..

      // Act
      var act = () => Ensure.NotNullOrEmpty("this is some random string");

      // Assert
      act.Should().NotThrow();
   }

   [Theory]
   [InlineData(0, 0)]
   [InlineData(1, 0)]
   public void GreaterOrEqualTo_WithHigherOrEqualValue_ShouldNotThrow(int value, int compare)
   {
      // Arrange
      // nothing to do..

      // Act
      var act = () => Ensure.GreaterOrEqualTo(value, compare);

      // Arrange
      act.Should().NotThrow();
   }

   [Fact]
   public void GreaterOrEqualTo_WithLowerValue_ShouldNotThrow()
   {
      // Arrange
      // nothing to do..

      // Act
      var act = () => Ensure.GreaterOrEqualTo(0, 1);

      // Arrange
      act.Should().Throw<ArgumentOutOfRangeException>();
   }
}