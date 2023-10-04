using WebBL.Models;
using Common.DTO;
using ScheduleDAL;
using ScheduleDAL.Entities;
using Common.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace WebBL
{
    public interface ISubjectsService
    {
        Task<IdDTO> CreateSubject(SubjectModel model);
        Task EditSubject(Guid id, SubjectModel model);
        Task DeleteSubject(Guid id);
    }
    public class SubjectsService : ISubjectsService
    {
        private readonly ScheduleContext _context;
        public SubjectsService(ScheduleContext context)
        {
            _context = context;
        }
        public async Task<IdDTO> CreateSubject(SubjectModel model)
        {
            var subject = new Subject
            {
                Name = model.Name
            };

            try
            {
                await _context.Subjects.AddAsync(subject);
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
            return new IdDTO(subject.Id);
        }
        public async Task EditSubject(Guid id, SubjectModel model)
        {
            var subject = _context.Subjects.Find(id);
            if (subject == null)
            {
                throw new KeyNotFoundException();
            }

            subject.Name = model.Name;
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
        public async Task DeleteSubject(Guid id)
        {
            var subject = _context.Subjects.Find(id);
            if (subject == null)
            {
                throw new KeyNotFoundException();
            }
            _context.Subjects.Remove(subject);
            await _context.SaveChangesAsync();
        }
    }
}