using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace brain_back_domain.Entities
{
    [Table("users")]
    public class User
    {
        [Key] [Column("id")]
        public int Id { get; set; }

        [Column("email")]
        public string Email { get; set; } = string.Empty;

        [Column("first_name")]
        public string FirstName { get; set; } = string.Empty;

        [Column("last_name")]
        public string LastName { get; set; } = string.Empty;
        
        [Column("user_name")]
        public string UserName { get; set; } = string.Empty;

        [Column("hash")]
        public string Hash { get; set; } = string.Empty;

        [Column("salt")]
        public string Salt { get; set; } = string.Empty;

        [Column("email_times")]
        public byte EmailTimes { get; set; } = 0;
        
        [Column("email_last_day")]
        public DateTime EmailLastDay { get; set; }
        
        [Column("user_name_times")]
        public byte UserNameTimes { get; set; } = 0;
        
        [Column("user_name_last_day")]
        public DateTime UserNameLastDay { get; set; }
    }
}