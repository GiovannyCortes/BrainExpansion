using brain_back_domain.DTOs;
using brain_back_domain.Entities;

namespace brain_back_application.Interfaces
{
    public interface IUserService
    {
        Task<bool> RegisterUser(User user);
        Task<bool> CheckExistingName(string name);
        Task<bool> CheckExistingEmail(string email);
        Task<string> AuthenticateUser(string emailName, string password);
        bool VerifyPassword(string storedHash, string storedSalt, string inputPassword);

        Task<bool?> UpdateUser(UpdateUser updateUser);
        Task<bool?> UpdateEmail(int userId, string email);
        Task<bool?> UpdateUserName(int userId, string userName);
    }
}