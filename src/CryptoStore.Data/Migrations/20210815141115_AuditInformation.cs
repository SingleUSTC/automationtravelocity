
﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CryptoStore.Data.Migrations
{
    public partial class AuditInformation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(