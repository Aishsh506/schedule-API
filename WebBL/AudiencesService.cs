using WebBL.Models;
using Common.DTO;
using ScheduleDAL;
using ScheduleDAL.Entities;
using Common.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace WebBL
{
    public interface IAudiencesService
    {
        Task<IdDTO> CreateAudience(AudienceModel model);
        Task EditAudience(Guid id, AudienceEditModel model);
        Task DeleteAudience(Guid id);
    }
    public class AudiencesService : IAudiencesService
    {
        private readonly ScheduleContext _context;
        public AudiencesService(ScheduleContext context)
        {
            _context = context;
        }
        public async Task<IdDTO> CreateAudience(AudienceModel model)
        {
            var audience = new Audience
            {
                Name = model.Name,
                BuildingId = model.BuildingId
            };

            try
            {
                await _context.Audiences.AddAsync(audience);
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
            return new IdDTO(audience.Id);
        }
        public async Task EditAudience(Guid id, AudienceEditModel model)
        {
            var audience = _context.Audiences.Find(id);
            if (audience == null)
            {
                throw new KeyNotFoundException();
            }

            audience.Name = model.Name;

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
        public async Task DeleteAudience(Guid id)
        {
            var audience = _context.Audiences.Find(id);
            if (audience == null)
            {
                throw new KeyNotFoundException();
            }
            _context.Audiences.Remove(audience);
            await _context.SaveChangesAsync();
        }
    }
}