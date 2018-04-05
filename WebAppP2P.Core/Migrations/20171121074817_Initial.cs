using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace WebAppP2P.Core.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BlockChain",
                columns: table => new
                {
                    BlockHash = table.Column<string>(type: "TEXT", nullable: false),
                    BlockHashPrevious = table.Column<string>(type: "TEXT", nullable: true),
                    Nonce = table.Column<ulong>(type: "INTEGER", nullable: false),
                    Timestamp = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlockChain", x => x.BlockHash);
                    table.ForeignKey(
                        name: "FK_BlockChain_BlockChain_BlockHashPrevious",
                        column: x => x.BlockHashPrevious,
                        principalTable: "BlockChain",
                        principalColumn: "BlockHash",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    StoreId = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Content = table.Column<string>(type: "TEXT", nullable: true),
                    From = table.Column<string>(type: "TEXT", nullable: true),
                    FromKey = table.Column<string>(type: "TEXT", nullable: true),
                    IV = table.Column<string>(type: "TEXT", nullable: true),
                    Id = table.Column<string>(type: "TEXT", nullable: true),
                    Nonce = table.Column<ulong>(type: "INTEGER", nullable: false),
                    Timestamp = table.Column<long>(type: "INTEGER", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: true),
                    To = table.Column<string>(type: "TEXT", nullable: true),
                    ToKey = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.StoreId);
                });

            migrationBuilder.CreateTable(
                name: "Nodes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: true),
                    LastActiveTimestamp = table.Column<long>(type: "INTEGER", nullable: true),
                    Url = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Nodes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BlockMessages",
                columns: table => new
                {
                    BlockHash = table.Column<string>(type: "TEXT", nullable: false),
                    StoreId = table.Column<ulong>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlockMessages", x => new { x.BlockHash, x.StoreId });
                    table.ForeignKey(
                        name: "FK_BlockMessages_BlockChain_BlockHash",
                        column: x => x.BlockHash,
                        principalTable: "BlockChain",
                        principalColumn: "BlockHash",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BlockMessages_Messages_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Messages",
                        principalColumn: "StoreId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Statistics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IsSuccess = table.Column<bool>(type: "INTEGER", nullable: false),
                    Latency = table.Column<int>(type: "INTEGER", nullable: false),
                    NodeId = table.Column<int>(type: "INTEGER", nullable: false),
                    Timestamp = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Statistics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Statistics_Nodes_NodeId",
                        column: x => x.NodeId,
                        principalTable: "Nodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BlockChain_BlockHashPrevious",
                table: "BlockChain",
                column: "BlockHashPrevious");

            migrationBuilder.CreateIndex(
                name: "IX_BlockMessages_StoreId",
                table: "BlockMessages",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_Statistics_NodeId",
                table: "Statistics",
                column: "NodeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BlockMessages");

            migrationBuilder.DropTable(
                name: "Statistics");

            migrationBuilder.DropTable(
                name: "BlockChain");

            migrationBuilder.DropTable(
                name: "Messages");

            migrationBuilder.DropTable(
                name: "Nodes");
        }
    }
}
