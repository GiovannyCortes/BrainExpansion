using brain_back_domain.DTOs;
using brain_back_domain.Entities;
using brain_back_application.Interfaces;
using brain_back_infrastructure.Interfaces;

namespace brain_back_application.Services
{
    public class QuestionService : IQuestionService
    {
        private readonly IQuestionRepository _repository;

        public QuestionService(IQuestionRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> Create(CreateQuestion questionToCreate)
        {
            return await _repository.Create(questionToCreate);
        }

        public async Task<List<Question>> GetPaginatedQuestions(GetQuestions getter)
        {
            return await _repository.GetPaginatedQuestions(getter);
        }
    }
}