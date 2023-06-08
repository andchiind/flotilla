using Api.Controllers.Models;
using Api.Database.Context;
using Api.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Services
{
    public interface IAreaService
    {
        public abstract Task<IEnumerable<Area>> ReadAll();

        public abstract Task<Area?> ReadById(string id);

        public abstract Task<IEnumerable<Area>> ReadByAsset(string asset);

        public abstract Task<Area?> ReadByAssetAndName(string asset, string name);

        public abstract Task<Area> Create(CreateAreaQuery newArea);

        public abstract Task<Area> Create(CreateAreaQuery newArea, List<Pose> safePositions);

        public abstract Task<Area> Update(Area Area);

        public abstract Task<Area?> AddSafePosition(string asset, string name, SafePosition safePosition);

        public abstract Task<Area?> Delete(string id);

    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Globalization",
        "CA1309:Use ordinal StringComparison",
        Justification = "EF Core refrains from translating string comparison overloads to SQL"
    )]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Globalization",
        "CA1304:Specify CultureInfo",
        Justification = "Entity framework does not support translating culture info to SQL calls"
    )]
    public class AreaService : IAreaService
    {
        private readonly FlotillaDbContext _context;

        public AreaService(FlotillaDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Area>> ReadAll()
        {
            return await GetAreas().ToListAsync();
        }

        private IQueryable<Area> GetAreas()
        {
            return _context.Areas.Include(a => a.SafePositions);
        }

        public async Task<Area?> ReadById(string id)
        {
            return await GetAreas()
                .FirstOrDefaultAsync(Area => Area.Id.Equals(id));
        }

        public async Task<Area?> ReadByAssetAndName(string name)
        {
            return await _context.Areas.Where(a =>
                a.Name.ToLower().Equals(name.ToLower())
            ).Include(a => a.SafePositions).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Area>> ReadByAsset(string asset)
        {

            return await _context.Areas.Where(a =>
                a.Deck.Installation.Asset.ShortName.Equals(asset.ToLower())).Include(a => a.SafePositions).ToListAsync();
        }

        public async Task<Area?> ReadByAssetAndName(string asset, string name)
        {
            return await _context.Areas.Where(a =>
                a.Deck.Installation.Asset.ShortName.ToLower().Equals(asset.ToLower()) &&
                a.Name.ToLower().Equals(name.ToLower())
            ).Include(a => a.SafePositions).FirstOrDefaultAsync();
        }

        public async Task<Area> Create(CreateAreaQuery newArea, List<Pose> safePositions)
        {
            var sp = new List<SafePosition>();
            foreach (var p in safePositions)
            {
                sp.Add(new SafePosition(p));
            }
            var Area = new Area
            {
                Name = newArea.AreaName,
                DefaultLocalizationPose = newArea.DefaultLocalizationPose,
                SafePositions = sp
            };
            await _context.Areas.AddAsync(Area);
            await _context.SaveChangesAsync();
            return Area;
        }

        public async Task<Area> Create(CreateAreaQuery newArea)
        {
            var area = await Create(newArea, new List<Pose>());
            return area;
        }

        public async Task<Area?> AddSafePosition(string asset, string name, SafePosition safePosition)
        {
            var area = await ReadByAssetAndName(asset, name);
            if (area is null)
            {
                return null;
            }
            area.SafePositions.Add(safePosition);
            _context.Areas.Update(area);
            await _context.SaveChangesAsync();
            return area;
        }

        public async Task<Area> Update(Area area)
        {
            var entry = _context.Update(area);
            await _context.SaveChangesAsync();
            return entry.Entity;
        }

        public async Task<Area?> Delete(string id)
        {
            var area = await GetAreas()
                .FirstOrDefaultAsync(ev => ev.Id.Equals(id));
            if (area is null)
            {
                return null;
            }

            _context.Areas.Remove(area);
            await _context.SaveChangesAsync();

            return area;
        }
    }
}
