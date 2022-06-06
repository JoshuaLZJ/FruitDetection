using Microsoft.EntityFrameworkCore.Migrations;

namespace MvcCoreUploadAndDisplayImage_Demo.Data.Migrations
{
    public partial class ObjectDetectionTaskClass : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ObjectDetectionTasks",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InputPicture = table.Column<string>(nullable: false),
                    ProcessedImage = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjectDetectionTasks", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ObjectDetectionTasks");
        }
    }
}
