using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DB.Migrations
{
    public partial class Center_Table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Centers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 50, nullable: true),
                    Title = table.Column<string>(maxLength: 100, nullable: true),
                    Desc = table.Column<string>(maxLength: 500, nullable: true),
                    City = table.Column<string>(maxLength: 50, nullable: true),
                    PhoneNo = table.Column<string>(maxLength: 15, nullable: true),
                    CommercialRegister = table.Column<string>(maxLength: 50, nullable: true),
                    Email = table.Column<string>(maxLength: 50, nullable: true),
                    IsGovernmental = table.Column<bool>(nullable: false),
                    Longitude = table.Column<decimal>(nullable: true),
                    Latitude = table.Column<decimal>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: true),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    LastUpdateDate = table.Column<DateTime>(nullable: true),
                    LastUpdatedBy = table.Column<int>(nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Centers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CenterImages",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CenterId = table.Column<int>(nullable: false),
                    Image = table.Column<byte[]>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: true),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    LastUpdateDate = table.Column<DateTime>(nullable: true),
                    LastUpdatedBy = table.Column<int>(nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CenterImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CenterImages_Centers_CenterId",
                        column: x => x.CenterId,
                        principalTable: "Centers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "CreationDate" },
                values: new object[] { "731e2e6e-d3e2-4cd9-a1d6-20fc34b6f66b", new DateTime(2020, 8, 16, 9, 43, 39, 522, DateTimeKind.Local).AddTicks(6186) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ConcurrencyStamp", "CreationDate" },
                values: new object[] { "ef9867d2-a45b-45c2-a159-bef628927511", new DateTime(2020, 8, 16, 9, 43, 39, 529, DateTimeKind.Local).AddTicks(2425) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ConcurrencyStamp", "CreationDate" },
                values: new object[] { "77400a00-b501-417d-aea5-362faded4417", new DateTime(2020, 8, 16, 9, 43, 39, 529, DateTimeKind.Local).AddTicks(2619) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "ConcurrencyStamp", "CreationDate" },
                values: new object[] { "a0a3acbe-c6b4-46a1-9f53-d9c549ce48aa", new DateTime(2020, 8, 16, 9, 43, 39, 529, DateTimeKind.Local).AddTicks(2689) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "ccfdf3a6-a7c6-437f-ae49-1a2d201dd527", "AQAAAAEAACcQAAAAENZd0Wv1oPzF6wusS8VkTPai5Y0DZCyKM5Z4vTLkhhWBTlJvxUSzTmwN5ezGj6PPNQ==", "7b56cc9a-d795-46b1-86b9-c2722b838db2" });

            migrationBuilder.CreateIndex(
                name: "IX_CenterImages_CenterId",
                table: "CenterImages",
                column: "CenterId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CenterImages");

            migrationBuilder.DropTable(
                name: "Centers");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "CreationDate" },
                values: new object[] { "351f53ea-b6ae-4573-aeb4-5f39b3efb86d", new DateTime(2020, 8, 13, 14, 13, 11, 370, DateTimeKind.Local).AddTicks(7704) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ConcurrencyStamp", "CreationDate" },
                values: new object[] { "af171e94-186d-422c-9ff8-6e763f91c8bf", new DateTime(2020, 8, 13, 14, 13, 11, 374, DateTimeKind.Local).AddTicks(8957) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ConcurrencyStamp", "CreationDate" },
                values: new object[] { "1ae36fb7-6168-4604-bb60-441b331aef7d", new DateTime(2020, 8, 13, 14, 13, 11, 374, DateTimeKind.Local).AddTicks(9063) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "ConcurrencyStamp", "CreationDate" },
                values: new object[] { "7954eca2-5aba-4543-85ff-5dafb34a7651", new DateTime(2020, 8, 13, 14, 13, 11, 374, DateTimeKind.Local).AddTicks(9091) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "82a0f002-ca43-4dea-8aac-381c57b99001", "AQAAAAEAACcQAAAAEDIyAdWFH3IO7Mzd9hIH9hNb6YFTxRGj6iYM3KfYqvIj40tktvOkr1WCj9T6d95hrQ==", "e2326766-9fd0-4b52-b732-43968ac8deae" });
        }
    }
}
