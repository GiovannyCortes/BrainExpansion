using brain_back_domain.Enumerations;

namespace brain_back_domain.DTOs
{
    public class GetQuestions
    {
        public int IdUser { get; set; } = 0;
        
        public EQuestionCategory Category { get; set; } = EQuestionCategory.NoCategory;
        
        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 30; 
    }
}