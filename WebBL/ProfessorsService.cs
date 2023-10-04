using WebBL.Models;
using Common.DTO;
using ScheduleDAL;
using ScheduleDAL.Entities;
using Common.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace WebBL
{
    public interface IProfessorsService
    {
        Task<IdDTO> CreateProfessor(ProfessorModel model);
        Task EditProfessor(Guid id, ProfessorModel model);
        Task DeleteProfessor(Guid id);
    }
    public class ProfessorsService : IProfessorsService
    {
        private readonly ScheduleContext _context;
        public ProfessorsService(ScheduleContext context)
        {
            _context = context;
        }
        public async Task<IdDTO> CreateProfessor(ProfessorModel model)
        {
            var professor = new Professor
            {
                FullName = model.FullName,
                ShortName = model.ShortName,
            };

            try
            {
                await _context.Professors.AddAsync(professor);
                await _context.SaveChangesAsync();
            }
            catch(DbUpdateException e) when(e.InnerException is SqlException)
            {
                var sqlEx = (SqlException)e.InnerException;
                switch (sqlEx.Number)
                {
                    case 2601: throw new DataConflictException(sqlEx.Number.ToString());
                    default: throw;
                }
            }
            return new IdDTO(professor.Id);
        }
        public async Task EditProfessor(Guid id, ProfessorModel model)
        {
            var professor = _context.Professors.Find(id);
            if (professor == null)
            {
                throw new KeyNotFoundException();
            }

            professor.FullName = model.FullName;
            professor.ShortName = model.ShortName;
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
        public async Task DeleteProfessor(Guid id)
        {
            var professor = _context.Professors.Find(id);
            if (professor == null)
            {
                throw new KeyNotFoundException();
            }
            _context.Professors.Remove(professor);
            await _context.SaveChangesAsync();
        }
    }
}