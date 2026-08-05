using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MediPoint.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class fixMig1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Prescription_Doctors_DoctorId1",
                table: "Prescription");

            migrationBuilder.DropIndex(
                name: "IX_Prescription_DoctorId1",
                table: "Prescription");

            migrationBuilder.DropColumn(
                name: "DoctorId1",
                table: "Prescription");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DoctorId1",
                table: "Prescription",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Prescription_DoctorId1",
                table: "Prescription",
                column: "DoctorId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Prescription_Doctors_DoctorId1",
                table: "Prescription",
                column: "DoctorId1",
                principalTable: "Doctors",
                principalColumn: "Id");
        }
    }
}
