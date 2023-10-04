namespace AccountBL.DTO
{
    public class TokenResponseDTO
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string? Email { get; set; }
        public List<string>? Role { get; set; }
        public Guid? Group { get; set; }
        public Guid? Professor { get; set; }
    }
}
