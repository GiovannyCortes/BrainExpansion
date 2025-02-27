using Npgsql;
using System.Data;
using System.Data.Common;
using brain_back_domain.DTOs;
using brain_back_domain.Entities;
using brain_back_infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using brain_back_infrastructure.Interfaces;
using brain_back_domain.Enumerations;
using NpgsqlTypes;

namespace brain_back_infrastructure.Repositories_PostGress
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

                // Cambio de SqlParameter a NpgsqlParameter
                com.Parameters.Add(new NpgsqlParameter("@QuestionText", questionToCreate.Value));
                com.Parameters.Add(new NpgsqlParameter("@TypeOption", questionToCreate.Type));
                com.Parameters.Add(new NpgsqlParameter("@CategoryOption", questionToCreate.Category));
                com.Parameters.Add(new NpgsqlParameter("@Explanation", questionToCreate.Explanation));
                com.Parameters.Add(new NpgsqlParameter("@DifficultyLevel", questionToCreate.Difficulty));
                com.Parameters.Add(new NpgsqlParameter("@CreatedBy", questionToCreate.CreatedById));
                com.Parameters.Add(new NpgsqlParameter("@Success", questionToCreate.SuccessAnswer));
                com.Parameters.Add(new NpgsqlParameter("@Ans_01", questionToCreate.Ans01));
                com.Parameters.Add(new NpgsqlParameter("@Ans_02", questionToCreate.Ans02));
                com.Parameters.Add(new NpgsqlParameter("@Ans_03", questionToCreate.Ans03));

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
            var questions = new List<Question>();

            try
            {
                // Utilizar la conexión del DbContext
                var connection = _context.Database.GetDbConnection();

                // Usar el comando con parámetros para evitar inyecciones de SQL
                var query = @"
                    SELECT * FROM sp_get_filtered_questions(
                        @USER_ID, 
                        @CATEGORY, 
                        @PAGE_NUMBER, 
                        @PAGE_SIZE
                );";

                using (var command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.Text;
                    command.CommandText = query;

                    // Agregar los parámetros para evitar inyecciones SQL
                    command.Parameters.Add(new NpgsqlParameter("@USER_ID", filterToGetQuestions.IdUser));
                    command.Parameters.Add(new NpgsqlParameter("@CATEGORY", NpgsqlDbType.Integer) { Value = (int)filterToGetQuestions.Category });
                    command.Parameters.Add(new NpgsqlParameter("@PAGE_SIZE", filterToGetQuestions.PageSize));
                    command.Parameters.Add(new NpgsqlParameter("@PAGE_NUMBER", filterToGetQuestions.PageNumber));

                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var question = new Question
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("id")),
                                Value = reader["question"].ToString() ?? string.Empty,
                                Type = (EQuestionType)reader.GetInt32(reader.GetOrdinal("type_option")),
                                Category = (EQuestionCategory)reader.GetInt32(reader.GetOrdinal("category_option")),
                                Explanation = reader["explanation"]?.ToString() ?? string.Empty,
                                Difficulty = (byte)reader.GetInt32(reader.GetOrdinal("difficulty_level")),
                                Stars = reader.GetInt32(reader.GetOrdinal("stars")),
                                Complaints = reader.GetInt32(reader.GetOrdinal("complaints"))
                            };
                            questions.Add(question);
                        }
                    }
                }

                // Obtención de las respuestas para cada pregunta
                var questionIds = questions.Select(q => q.Id).ToList();
                var answers = await _context.Answers.Where(a => questionIds.Contains(a.QuestionId)).ToListAsync();

                foreach (Question q in questions)
                {
                    q.Answers = answers.Where(a => a.QuestionId == q.Id).ToList();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            return questions;
        }

    }
}