using AccountDAL.Entities;

namespace AccountBL.DTO
{
    public class AccountDTO
    {
        public AccountDTO(AppUser user)
        {
            Email = user.Email;
            Username = user.UserName;
        }
        public string Email { get; set; }
        public string? Username { get; set; }
    }
}
