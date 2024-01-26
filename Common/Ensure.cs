using System.Diagnostics.CodeAnalysis;

namespace Strooware.SalonReviews.Common;

public static class Ensure
{
   public static void NotNullOrEmpty([NotNull] string value)
   {
      if (string.IsNullOrEmpty(value))
      {
         throw new ArgumentNullException(nameof(value));
      }
   }

   public static void GreaterOrEqualTo(int value, int compare)
   {
      if (value < compare)
      {
         throw new ArgumentOutOfRangeException(nameof(value), value, $"Value should be equal to or greater then {compare}");
      }
   }
}