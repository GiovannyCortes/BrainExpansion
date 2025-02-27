using brain_back_domain.Enumerations;

namespace brain_back_domain.Models
{
    public class ApiResponse<T>
    {
        public EApiResponse Response { get; set; } = EApiResponse.None;

        public string Message { get; set; } = string.Empty;

        public T? Data { get; set; }
    }
}