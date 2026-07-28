using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Api.Migrations
{
    /// <inheritdoc />
    public partial class RemoveInspectionsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MissionTasks_Inspections_InspectionId",
                table: "MissionTasks");

            migrationBuilder.DropTable(
                name: "Inspections");

            migrationBuilder.DropIndex(
                name: "IX_MissionTasks_InspectionId",
                table: "MissionTasks");

            migrationBuilder.RenameColumn(
                name: "InspectionId",
                table: "MissionTasks",
                newName: "AcousticInspectionMetadata_DetectionType");

            migrationBuilder.AddColumn<float>(
                name: "AcousticInspectionMetadata_FrequencyFrom",
                table: "MissionTasks",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "AcousticInspectionMetadata_FrequencyTo",
                table: "MissionTasks",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AcousticInspectionMetadata_Roi_Height",
                table: "MissionTasks",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AcousticInspectionMetadata_Roi_Width",
                table: "MissionTasks",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AcousticInspectionMetadata_Roi_X",
                table: "MissionTasks",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AcousticInspectionMetadata_Roi_Y",
                table: "MissionTasks",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "AcousticInspectionMetadata_SnrValueThreshold",
                table: "MissionTasks",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SensorType",
                table: "MissionTasks",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<float>(
                name: "TargetPosition_X",
                table: "MissionTasks",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "TargetPosition_Y",
                table: "MissionTasks",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "TargetPosition_Z",
                table: "MissionTasks",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "VideoDuration",
                table: "MissionTasks",
                type: "real",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AcousticInspectionMetadata_FrequencyFrom",
                table: "MissionTasks");

            migrationBuilder.DropColumn(
                name: "AcousticInspectionMetadata_FrequencyTo",
                table: "MissionTasks");

            migrationBuilder.DropColumn(
                name: "AcousticInspectionMetadata_Roi_Height",
                table: "MissionTasks");

            migrationBuilder.DropColumn(
                name: "AcousticInspectionMetadata_Roi_Width",
                table: "MissionTasks");

            migrationBuilder.DropColumn(
                name: "AcousticInspectionMetadata_Roi_X",
                table: "MissionTasks");

            migrationBuilder.DropColumn(
                name: "AcousticInspectionMetadata_Roi_Y",
                table: "MissionTasks");

            migrationBuilder.DropColumn(
                name: "AcousticInspectionMetadata_SnrValueThreshold",
                table: "MissionTasks");

            migrationBuilder.DropColumn(
                name: "SensorType",
                table: "MissionTasks");

            migrationBuilder.DropColumn(
                name: "TargetPosition_X",
                table: "MissionTasks");

            migrationBuilder.DropColumn(
                name: "TargetPosition_Y",
                table: "MissionTasks");

            migrationBuilder.DropColumn(
                name: "TargetPosition_Z",
                table: "MissionTasks");

            migrationBuilder.DropColumn(
                name: "VideoDuration",
                table: "MissionTasks");

            migrationBuilder.RenameColumn(
                name: "AcousticInspectionMetadata_DetectionType",
                table: "MissionTasks",
                newName: "InspectionId");

            migrationBuilder.CreateTable(
                name: "Inspections",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    AnalysisTypes = table.Column<string[]>(type: "text[]", nullable: true),
                    InspectionType = table.Column<string>(type: "text", nullable: false),
                    IsarInspectionId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    VideoDuration = table.Column<float>(type: "real", nullable: true),
                    AcousticInspectionMetadata_DetectionType = table.Column<string>(type: "text", nullable: true),
                    AcousticInspectionMetadata_FrequencyFrom = table.Column<float>(type: "real", nullable: true),
                    AcousticInspectionMetadata_FrequencyTo = table.Column<float>(type: "real", nullable: true),
                    AcousticInspectionMetadata_SnrValueThreshold = table.Column<float>(type: "real", nullable: true),
                    AcousticInspectionMetadata_Roi_Height = table.Column<int>(type: "integer", nullable: true),
                    AcousticInspectionMetadata_Roi_Width = table.Column<int>(type: "integer", nullable: true),
                    AcousticInspectionMetadata_Roi_X = table.Column<int>(type: "integer", nullable: true),
                    AcousticInspectionMetadata_Roi_Y = table.Column<int>(type: "integer", nullable: true),
                    InspectionTarget_X = table.Column<float>(type: "real", nullable: false),
                    InspectionTarget_Y = table.Column<float>(type: "real", nullable: false),
                    InspectionTarget_Z = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inspections", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MissionTasks_InspectionId",
                table: "MissionTasks",
                column: "InspectionId");

            migrationBuilder.AddForeignKey(
                name: "FK_MissionTasks_Inspections_InspectionId",
                table: "MissionTasks",
                column: "InspectionId",
                principalTable: "Inspections",
                principalColumn: "Id");
        }
    }
}
