using WebBL.Models;
using Common.DTO;
using ScheduleDAL;
using ScheduleDAL.Entities;
using Common.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace WebBL
{
    public interface IBuildingsService
    {
        Task<IdDTO> CreateBuilding(BuildingModel model);
        Task EditBuilding(Guid id, BuildingModel model);
        Task DeleteBuilding(Guid id);
    }
    public class BuildingsService : IBuildingsService
    {
        private readonly ScheduleContext _context;
        public BuildingsService(ScheduleContext context)
        {
            _context = context;
        }
        public async Task<IdDTO> CreateBuilding(BuildingModel model)
        {
            var building = new Building
            {
                Name = model.Name
            };

            try
            {
                await _context.Buildings.AddAsync(building);
                await _context.SaveChangesAsync();
            }
            catch(DbUpdateException e) when(e.InnerException is SqlException)
            {
                var sqlEx = (SqlException)e.InnerException;
                switch (sqlEx.Number)
                {
                    case 2601: throw new DataConflictException(sqlEx.Number.ToString());
                    case 547: throw new KeyNotFoundException(sqlEx.Message);
                    default: throw;
                }
            }
            return new IdDTO(building.Id);
        }
        public async Task EditBuilding(Guid id, BuildingModel model)
        {
            var building = _context.Buildings.Find(id);
            if (building == null)
            {
                throw new KeyNotFoundException();
            }

            building.Name = model.Name;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException e) when (e.InnerException is SqlException)
            {
                var sqlEx = (SqlException)e.InnerException;
                switch (sqlEx.Number)
                {
                    case 2601: throw new DataConflictException(sqlEx.Number.ToString());
                    default: throw;
                }
            }
        }
        public async Task DeleteBuilding(Guid id)
        {
            var building = _context.Buildings.Find(id);
            if (building == null)
            {
                throw new KeyNotFoundException();
            }
            _context.Buildings.Remove(building);
            await _context.SaveChangesAsync();
        }
    }
}