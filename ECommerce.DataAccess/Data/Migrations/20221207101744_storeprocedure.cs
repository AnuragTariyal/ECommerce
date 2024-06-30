using Microsoft.EntityFrameworkCore.Migrations;

namespace ECommerce.DataAccess.Migrations
{
    public partial class storeprocedure : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"CREATE PROCEDURE CreateCoverType
	                            @Name varchar(50)
                            AS
	                            insert CoverTypes values(@Name)");

            migrationBuilder.Sql(@"CREATE PROCEDURE UpdateCoverType
	                                @id int,
	                                @Name varchar(50)
                                AS
	                                update CoverTypes set Name=@Name where Id=@id");

            migrationBuilder.Sql(@"CREATE PROCEDURE DeleteCoverType
	                            @id int
                            AS
	                            Delete CoverTypes where Id=@id");

            migrationBuilder.Sql(@"CREATE PROCEDURE GetCoverTypes
                                    AS
	                                    Select * from CoverTypes");

            migrationBuilder.Sql(@"CREATE PROCEDURE GetCoverType
	                            @id int
                            AS
	                            Select * from CoverTypes where Id=@id");

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
