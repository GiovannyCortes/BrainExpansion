using brain_back_domain.DTOs;
using brain_back_domain.Entities;

namespace brain_back_application.Interfaces
{
    public interface IQuestionService
    {
        Task<bool> Create(CreateQuestion questionToCreate);
        Task<List<Question>> GetPaginatedQuestions(GetQuestions getter);
    }
}