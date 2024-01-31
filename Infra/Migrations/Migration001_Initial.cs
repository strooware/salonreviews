using FluentMigrator;

namespace Strooware.SalonReviews.Infra.Migrations;

[Migration(001)]
public class Migration001_Initial : Migration
{
   public override void Up()
   {
      Create.Table("reviews")
         .WithColumn("Id").AsInt32().PrimaryKey().Identity()
         .WithColumn("SalonId").AsString().ForeignKey("salons", "id")
         .WithColumn("NumStars").AsInt16()
         .WithColumn("Description").AsString()
         .WithColumn("Author").AsString()
         .WithColumn("ReviewDate").AsDateTime2();

      Create.Table("salons")
         .WithColumn("Id").AsString().PrimaryKey()
         .WithColumn("LastFetchedTime").AsDateTime2();
   }

   public override void Down()
   {
      Delete.Table("reviews");
      Delete.Table("salons");
   }
}