using Api.Controllers.Models;
using Api.Database.Models;
using Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("areas")]
    public class AreaController : ControllerBase
    {
        private readonly IAreaService _areaService;

        private readonly IMapService _mapService;

        private readonly ILogger<AreaController> _logger;

        public AreaController(
            ILogger<AreaController> logger,
            IMapService mapService,
            IAreaService areaService
        )
        {
            _logger = logger;
            _mapService = mapService;
            _areaService = areaService;
        }

        /// <summary>
        /// Add a new area
        /// </summary>
        /// <remarks>
        /// <para> This query adds a new area to the database </para>
        /// </remarks>
        [HttpPost]
        [Authorize(Roles = Role.Admin)]
        [ProducesResponseType(typeof(Area), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Area>> Create([FromBody] CreateAreaQuery area)
        {
            _logger.LogInformation("Creating new area");
            try
            {
                var existingArea = await _areaService.ReadByAssetAndName(area.AssetCode, area.AreaName);
                if (existingArea != null)
                {
                    _logger.LogWarning("An area for given name and asset already exists");
                    return Conflict($"Area already exists");
                }

                var newArea = await _areaService.Create(area);
                _logger.LogInformation(
                    "Succesfully created new area with id '{areaId}'",
                    newArea.Id
                );
                return CreatedAtAction(
                    nameof(GetAreaById),
                    new { id = newArea.Id },
                    newArea
                );
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while creating new area");
                throw;
            }
        }

        /// <summary>
        /// Add safe position to an area
        /// </summary>
        /// <remarks>
        /// <para> This query adds a new safe position to the database </para>
        /// </remarks>
        [HttpPost]
        [Authorize(Roles = Role.Admin)]
        [Route("{asset}/{installationName}/{deckName}/{areaName}/safe-position")]
        [ProducesResponseType(typeof(Area), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Area>> AddSafePosition(
            [FromRoute] string assetName,
            [FromRoute] string installationName,
            [FromRoute] string deckName,
            [FromRoute] string areaName,
            [FromBody] Pose safePosition
        )
        {
            _logger.LogInformation("Adding new safe position to {Asset}, {Installtion}, {Deck}, {Area}", assetName, installationName, deckName, areaName);
            try
            {
                var area = await _areaService.AddSafePosition(assetName, areaName, new SafePosition(safePosition));
                if (area != null)
                {
                    _logger.LogInformation("Succesfully added new safe position for asset '{assetId}' and name '{name}'", assetName, areaName);
                    return CreatedAtAction(nameof(GetAreaById), new { id = area.Id }, area); ;
                }
                else
                {
                    _logger.LogInformation("No area with asset {assetName}, installation {installationName}, deck {deckName} and name {areaName} could be found.", assetName, installationName, deckName, areaName);
                    return NotFound($"No area with asset {assetName}, installation {installationName}, deck {deckName} and name {areaName} could be found.");
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while creating or adding new safe zone");
                throw;
            }
        }

        /// <summary>
        /// Deletes the area with the specified id from the database.
        /// </summary>
        [HttpDelete]
        [Authorize(Roles = Role.Admin)]
        [Route("{id}")]
        [ProducesResponseType(typeof(Area), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Area>> DeleteArea([FromRoute] string id)
        {
            var area = await _areaService.Delete(id);
            if (area is null)
                return NotFound($"Area with id {id} not found");
            return Ok(area);
        }

        /// <summary>
        /// List all asset areas in the Flotilla database
        /// </summary>
        /// <remarks>
        /// <para> This query gets all asset areas </para>
        /// </remarks>
        [HttpGet]
        [Authorize(Roles = Role.Any)]
        [ProducesResponseType(typeof(IList<Area>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IList<Area>>> GetAreas()
        {
            try
            {
                var areas = await _areaService.ReadAll();
                return Ok(areas);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error during GET of areas from database");
                throw;
            }
        }

        /// <summary>
        /// Lookup area by specified id.
        /// </summary>
        [HttpGet]
        [Authorize(Roles = Role.Any)]
        [Route("{id}")]
        [ProducesResponseType(typeof(Area), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Area>> GetAreaById([FromRoute] string id)
        {
            try
            {
                var area = await _areaService.ReadById(id);
                if (area == null)
                    return NotFound($"Could not find area with id {id}");
                return Ok(area);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error during GET of areas from database");
                throw;
            }

        }

        /// <summary>
        /// Gets map metadata for localization poses belonging to area with specified id
        /// </summary>
        [HttpGet]
        [Authorize(Roles = Role.Any)]
        [Route("{id}/map-metadata")]
        [ProducesResponseType(typeof(MapMetadata), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<MapMetadata>> GetMapMetadata([FromRoute] string id)
        {
            var area = await _areaService.ReadById(id);
            if (area is null)
            {
                string errorMessage = $"Area not found for area with ID {id}";
                _logger.LogError("{ErrorMessage}", errorMessage);
                return NotFound(errorMessage);
            }

            MapMetadata? map;
            var positions = new List<Position>
            {
                area.DefaultLocalizationPose.Position
            };
            try
            {
                map = await _mapService.ChooseMapFromPositions(positions, area.Deck.Installation.Asset.AssetCode);
            }
            catch (ArgumentOutOfRangeException)
            {
                string errorMessage = $"Unable to find map for area with ID {id}";
                _logger.LogWarning("{ErrorMessage}", errorMessage);
                return NotFound(errorMessage);
            }

            if (map == null)
            {
                return NotFound("Could not find map for this area");
            }
            return Ok(map);
        }
    }
}
