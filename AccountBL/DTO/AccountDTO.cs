using AccountDAL.Entities;

namespace AccountBL.DTO
{
    public class AccountDTO
    {
        public AccountDTO(AppUser user)
        {
            Email = user.Email;
            Username = user.UserName;
            GroupId = user.GroupId;
            ProfessorId = user.ProfessorId;
        }
        public string? Email { get; set; }
        public string? Username { get; set; }
        public List<string>? Role { get; set; }
        public Guid? GroupId { get; set; }
        public Guid? ProfessorId { get; set; }
    }
}
