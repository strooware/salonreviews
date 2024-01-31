using System.Data;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;

namespace Strooware.SalonReviews.Infra;

public interface IContextFactory
{
   string ConnectionString { get; }
   IDbConnection Create();
}

public class ContextFactory(IConfiguration configuration) : IContextFactory
{
   private readonly IConfiguration configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
   public string ConnectionString => configuration.GetConnectionString("testing")!;
   public IDbConnection Create() => new SqliteConnection(ConnectionString);
}
