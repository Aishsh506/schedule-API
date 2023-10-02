namespace AccountBL.DTO
{
    public class TokenResponseDTO
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string Email { get; set; }
        public List<string> Role { get; set; }
        //public GroupDTO? Group { get; set; }
        //public ProfessorDTO? Professor { get; set; }
    }
}
