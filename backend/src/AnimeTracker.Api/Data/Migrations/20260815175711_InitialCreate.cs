using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AnimeTracker.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AnimeCaches",
                columns: table => new
                {
                    AniListId = table.Column<int>(type: "integer", nullable: false),
                    TitleRomaji = table.Column<string>(type: "text", nullable: false),
                    TitleEnglish = table.Column<string>(type: "text", nullable: true),
                    TitleNative = table.Column<string>(type: "text", nullable: true),
                    CoverImageUrl = table.Column<string>(type: "text", nullable: true),
                    Format = table.Column<string>(type: "text", nullable: true),
                    EpisodesTotal = table.Column<int>(type: "integer", nullable: true),
                    Genres = table.Column<string[]>(type: "text[]", nullable: false),
                    SyncedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnimeCaches", x => x.AniListId);
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WatchEntries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AniListId = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Rating = table.Column<int>(type: "integer", nullable: true),
                    Review = table.Column<string>(type: "text", nullable: true),
                    EpisodesWatched = table.Column<int>(type: "integer", nullable: false),
                    StartedAt = table.Column<DateOnly>(type: "date", nullable: true),
                    FinishedAt = table.Column<DateOnly>(type: "date", nullable: true),
                    RewatchCount = table.Column<int>(type: "integer", nullable: false),
                    Favorite = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WatchEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WatchEntries_AnimeCaches_AniListId",
                        column: x => x.AniListId,
                        principalTable: "AnimeCaches",
                        principalColumn: "AniListId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WatchEntryTag",
                columns: table => new
                {
                    TagsId = table.Column<int>(type: "integer", nullable: false),
                    WatchEntriesId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WatchEntryTag", x => new { x.TagsId, x.WatchEntriesId });
                    table.ForeignKey(
                        name: "FK_WatchEntryTag_Tags_TagsId",
                        column: x => x.TagsId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WatchEntryTag_WatchEntries_WatchEntriesId",
                        column: x => x.WatchEntriesId,
                        principalTable: "WatchEntries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tags_Name",
                table: "Tags",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WatchEntries_AniListId",
                table: "WatchEntries",
                column: "AniListId");

            migrationBuilder.CreateIndex(
                name: "IX_WatchEntries_Status",
                table: "WatchEntries",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_WatchEntryTag_WatchEntriesId",
                table: "WatchEntryTag",
                column: "WatchEntriesId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WatchEntryTag");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropTable(
                name: "WatchEntries");

            migrationBuilder.DropTable(
                name: "AnimeCaches");
        }
    }
}
