﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Api.Controllers.Models;
#pragma warning disable CS8618
namespace Api.Database.Models
{
    public class InspectionFinding
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }

        public DateTime InspectionDate { get; set; }

        public string Area { get; set; }

        public string IsarStepId { get; set; }

        public string Findings { get; set; }

        public InspectionFinding(InspectionFindingsQuery createInspectionFindingQuery)
        {
            InspectionDate = createInspectionFindingQuery.InspectionDate;
            Area = createInspectionFindingQuery.Area;
            IsarStepId = createInspectionFindingQuery.IsarStepId;
            Findings = createInspectionFindingQuery.Findings;
        }

        public InspectionFinding()
        {
            InspectionDate = DateTime.UtcNow;
            Area = "string";
            IsarStepId = "string";
            Findings = "string";
        }
    }

}