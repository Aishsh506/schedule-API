using WebBL.Models;
using Common.DTO;
using ScheduleDAL;
using ScheduleDAL.Entities;
using Common.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace WebBL
{
    public interface IGroupsService
    {
        Task<IdDTO> CreateGroup(GroupModel model);
        Task EditGroup(Guid id, GroupModel model);
        Task DeleteGroup(Guid id);
    }
    public class GroupsService : IGroupsService
    {
        private readonly ScheduleContext _context;
        public GroupsService(ScheduleContext context)
        {
            _context = context;
        }
        public async Task<IdDTO> CreateGroup(GroupModel model)
        {
            var group = new Group
            {
                Name = model.Name
            };

            try
            {
                await _context.Groups.AddAsync(group);
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
            return new IdDTO(group.Id);
        }
        public async Task EditGroup(Guid id, GroupModel model)
        {
            var group = _context.Groups.Find(id);
            if (group == null)
            {
                throw new KeyNotFoundException();
            }

            group.Name = model.Name;
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
        public async Task DeleteGroup(Guid id)
        {
            var group = _context.Groups.Find(id);
            if (group == null)
            {
                throw new KeyNotFoundException();
            }
            _context.Groups.Remove(group);
            await _context.SaveChangesAsync();
        }
    }
}