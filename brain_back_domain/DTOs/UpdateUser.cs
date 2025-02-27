namespace brain_back_domain.DTOs
{
    public class UpdateUser
    {
        public int Id { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;
    }
}