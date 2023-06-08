﻿using Api.Database.Models;

namespace Api.Controllers.Models
{
    public struct CreateAreaQuery
    {
        public string AssetCode { get; set; }
        public string AreaName { get; set; }

        public Pose DefaultLocalizationPose { get; set; }
    }
}
