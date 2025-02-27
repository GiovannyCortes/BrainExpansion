using brain_back_domain.DTOs;
using brain_back_domain.Entities;

namespace brain_back_infrastructure.Interfaces
{
    public interface IQuestionRepository
    {
        Task<bool> Create(CreateQuestion questionToCreate);

        Task<List<Question>> GetPaginatedQuestions(GetQuestions filterToGetQuestions);
    }
}