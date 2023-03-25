﻿using Microsoft.EntityFrameworkCore.Migrations;

namespace CryptoStore.Data.Migrations
{
    public partial class ServiceExplainProperty : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ServiceExplain",
                table: "Services",
                type: "nvarchar(max)",
                nullable: false,