using brain_back_domain.Enumerations;

namespace brain_back_domain.DTOs
{
    public class CreateQuestion
    {
        public string Value { get; set; } = string.Empty;

        public EQuestionType Type { get; set; }

        public EQuestionCategory Category { get; set; }

        public string Explanation { get; set; } = string.Empty;

        public byte Difficulty { get; set; } = 0;

        public int CreatedById { get; set; } = 0;

        public string SuccessAnswer { get; set; } = string.Empty;

        public string Ans01 { get; set; } = string.Empty;

        public string Ans02 { get; set; } = string.Empty;

        public string Ans03 { get; set; } = string.Empty;
    }
}