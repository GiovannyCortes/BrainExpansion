using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace brain_back_domain.Entities
{
    [Table("answers")]
    public class Answer
    {
        [Key] [Column("id")]
        public int Id { get; set; }
        
        [Column("id_question")]
        public int QuestionId { get; set; }

        [Column("value")]
        public string Value { get; set; } = string.Empty;

        [Column("is_correct")]
        public bool IsCorrect { get; set; } = false;
    }
}