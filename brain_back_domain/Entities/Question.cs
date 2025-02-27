using brain_back_domain.Enumerations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace brain_back_domain.Entities
{
    [Table("questions")]
    public class Question
    {
        [Key] [Column("id")]
        public int Id { get; set; }

        [Column("value")]
        public string Value { get; set; } = string.Empty;

        public EQuestionType Type {
            get => (EQuestionType)_type;
            set => _type = (byte)value;
        }
        [Column("type_option")]
        private byte _type;

        public EQuestionCategory Category {
            get => (EQuestionCategory)_category;
            set => _category = (byte)value;
        }
        [Column("category_option")]
        private byte _category;

        [NotMapped]
        public List<Answer> Answers { get; set; } = new List<Answer>();

        [Column("explanation")]
        public string Explanation { get; set; } = string.Empty;

        [Column("difficulty_level")]
        public byte Difficulty { get; set; }

        [NotMapped]
        public int Stars { 
            get => (int)(_stars);
            set => _stars = (Int16)value;
        }
        [Column("stars")]
        private Int16 _stars;

        [NotMapped]
        public int Complaints {
            get => (int)_complaints;
            set => _complaints = (Int16)value;
        }
        [Column("complaints")]
        private Int16 _complaints;
    }
}