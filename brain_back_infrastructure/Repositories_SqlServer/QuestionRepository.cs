using System.Data;
using System.Data.Common;
using brain_back_domain.DTOs;
using brain_back_domain.Entities;
using brain_back_domain.Enumerations;
using brain_back_infrastructure.Data;
using brain_back_infrastructure.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace brain_back_infrastructure.Repositories_SqlServer
{
    public class QuestionRepository : IQuestionRepository
    {
        private BrainContext _context;

        public QuestionRepository(BrainContext context)
        {
            this._context = context;
        }

        public async Task<bool> Create(CreateQuestion questionToCreate)
        {
            using (DbCommand com = this._context.Database.GetDbConnection().CreateCommand())
            {
                com.CommandType = CommandType.StoredProcedure;
                com.CommandText = "SP_CREATE_QUESTION";

                com.Parameters.Add(new SqlParameter("@QuestionText", questionToCreate.Value));
                com.Parameters.Add(new SqlParameter("@TypeOption", questionToCreate.Type));
                com.Parameters.Add(new SqlParameter("@CategoryOption", questionToCreate.Category));
                com.Parameters.Add(new SqlParameter("@Explanation", questionToCreate.Explanation));
                com.Parameters.Add(new SqlParameter("@DifficultyLevel", questionToCreate.Difficulty));
                com.Parameters.Add(new SqlParameter("@CreatedBy", questionToCreate.CreatedById));
                com.Parameters.Add(new SqlParameter("@Success", questionToCreate.SuccessAnswer));
                com.Parameters.Add(new SqlParameter("@Ans_01", questionToCreate.Ans01));
                com.Parameters.Add(new SqlParameter("@Ans_02", questionToCreate.Ans02));
                com.Parameters.Add(new SqlParameter("@Ans_03", questionToCreate.Ans03));

                try
                {
                    if (com.Connection != null)
                    {
                        com.Connection.Open();
                        await com.ExecuteNonQueryAsync();
                        com.Connection.Close();
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al crear la pregunta: {ex.Message}");
                    return false;
                }
            }
        }

        public async Task<List<Question>> GetPaginatedQuestions(GetQuestions filterToGetQuestions)
        {
            using (DbCommand com = this._context.Database.GetDbConnection().CreateCommand())
            {
                com.CommandType = CommandType.StoredProcedure;
                com.CommandText = "SP_GET_FILTERED_QUESTIONS";

                com.Parameters.Add(new SqlParameter("@USER_ID", filterToGetQuestions.IdUser));
                com.Parameters.Add(new SqlParameter("@CATEGORY", filterToGetQuestions.Category));
                com.Parameters.Add(new SqlParameter("@PAGE_SIZE", filterToGetQuestions.PageSize));
                com.Parameters.Add(new SqlParameter("@PAGE_NUMBER", filterToGetQuestions.PageNumber));

                List<Question> questions = new List<Question>();
                if (com.Connection != null)
                {
                    await com.Connection.OpenAsync();
                    using (DbDataReader reader = await com.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            Question qs = new Question
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("id")),
                                Value = reader["question"].ToString() ?? string.Empty,
                                Type = (EQuestionType)reader.GetInt32(reader.GetOrdinal("type_option")),
                                Category = (EQuestionCategory)reader.GetInt32(reader.GetOrdinal("category_option")),
                                Explanation = reader["explanation"] is DBNull ? string.Empty : reader["explanation"].ToString()!,
                                Difficulty = (byte)reader.GetInt32(reader.GetOrdinal("difficulty_level")),
                                Stars = reader.GetInt32(reader.GetOrdinal("stars")),
                                Complaints = reader.GetInt32(reader.GetOrdinal("complaints"))
                            };
                            questions.Add(qs);
                        }

                        if (await reader.NextResultAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                Answer item = new Answer()
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                                    QuestionId = reader.GetInt32(reader.GetOrdinal("id_question")),
                                    Value = reader["value"].ToString() ?? string.Empty,
                                    IsCorrect = reader.GetBoolean(reader.GetOrdinal("is_correct"))
                                };
                                questions.FirstOrDefault(q => q.Id == item.QuestionId)?.Answers.Add(item);
                            }
                        }
                    }

                    com.Connection.Close();
                }

                return questions;
            }
        }
    }
}