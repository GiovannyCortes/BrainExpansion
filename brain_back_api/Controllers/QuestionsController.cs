using Microsoft.AspNetCore.Mvc;
using brain_back_domain.DTOs;
using brain_back_domain.Models;
using brain_back_domain.Entities;
using brain_back_domain.Enumerations;
using brain_back_application.Interfaces;

namespace brain_back_api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class QuestionsController : ControllerBase
    {
        private IQuestionService _service;

        public QuestionsController(IQuestionService service)
        {
            this._service = service;
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult<ApiResponse<string>>> Create([FromQuery] CreateQuestion questionToCreate)
        {
            bool created = await this._service.Create(questionToCreate);
            if (!created) {
                return BadRequest(new ApiResponse<string>
                {
                    Response = EApiResponse.Error, Message = "Error creating question"
                });
            }

            return Ok(new ApiResponse<string>
            {
                Response = EApiResponse.Success, Message = "Question created successfully"
            });
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<List<Question>>> GetPaginatedQuestions([FromQuery] GetQuestions filterToGetQuestions)
        {
            List<Question> questions = await this._service.GetPaginatedQuestions(filterToGetQuestions);
            return Ok(questions);
        }
    }
}