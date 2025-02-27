using brain_back_domain.DTOs;
using brain_back_domain.Entities;

namespace brain_back_infrastructure.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetUserByUserName(string userName);

        Task<User?> GetUserByEmail(string email);

        Task<bool> RegisterUser(User newUser);

        Task<bool> DeleteUser(int userId);

        Task<bool?> UpdateUser(UpdateUser updateUser);

        Task<bool?> UpdateEmail(int userId, string email);

        Task<bool?> UpdateUserName(int userId, string userName);
    }
}