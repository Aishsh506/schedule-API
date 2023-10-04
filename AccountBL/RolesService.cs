using AccountDAL;
using AccountDAL.Entities;
using Microsoft.AspNetCore.Identity;
using Common.Enums;

namespace AccountBL
{
    public interface IRolesService
    {
        Task AddStudentRole(string userId, Guid groupId);
        Task AddProfessorRole(string userId, Guid professorId);
        Task AddEditorRole(string userId);
        Task RemoveRole(string userId, Roles role);
    }
    public class RolesService : IRolesService
    {
        private readonly UserManager<AppUser> _userManager;
        public RolesService(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task AddStudentRole(string userId, Guid groupId)
        {
            var user = await FindUser(userId);

            await _userManager.AddToRoleAsync(user, Roles.Student.ToString());
            user.GroupId = groupId;
            await _userManager.UpdateAsync(user);
        }
        public async Task AddProfessorRole(string userId, Guid professorId)
        {
            var user = await FindUser(userId);

            await _userManager.AddToRoleAsync(user, Roles.Professor.ToString());
            user.GroupId = professorId;
            await _userManager.UpdateAsync(user);
        }
        public async Task AddEditorRole(string userId)
        {
            var user = await FindUser(userId);

            await _userManager.AddToRoleAsync(user, Roles.Editor.ToString());
        }
        public async Task RemoveRole(string userId, Roles role)
        {
            var user = await FindUser(userId);

            await _userManager.RemoveFromRoleAsync(user, role.ToString());
            switch(role)
            {
                case Roles.Student: user.GroupId = null; break;
                case Roles.Professor: user.ProfessorId = null; break;
                case Roles.Editor: break;
            }
            await _userManager.UpdateAsync(user);
        }
        private async Task<AppUser> FindUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                throw new KeyNotFoundException();
            }
            return user;
        }
    }
}
