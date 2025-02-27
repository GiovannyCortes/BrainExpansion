using brain_back_domain.Entities;
using brain_back_application.Helpers;
using brain_back_application.Interfaces;
using brain_back_infrastructure.Interfaces;
using brain_back_domain.DTOs;

namespace brain_back_application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly HelperOAuthToken _helperOAuthToken;

        public UserService(IUserRepository userRepository, HelperOAuthToken helperOAuthToken)
        {
            _userRepository = userRepository;
            _helperOAuthToken = helperOAuthToken;
        }

        public async Task<bool> RegisterUser(User user)
        {
            return await _userRepository.RegisterUser(user);
        }

        public async Task<bool> CheckExistingName(string name)
        {
            User? response = await _userRepository.GetUserByUserName(name);
            return response != null;
        }

        public async Task<bool> CheckExistingEmail(string email)
        {
            User? response = await _userRepository.GetUserByEmail(email);
            return response != null;
        }

        public async Task<string> AuthenticateUser(string emailName, string password)
        {
            User? user;
            if (emailName.Contains("@"))
                user = await _userRepository.GetUserByEmail(emailName);
            else
                user = await _userRepository.GetUserByUserName(emailName);

            if (user == null) return string.Empty;

            // Verificar que el hash de la contraseña coincida con la base de datos
            if (!VerifyPassword(user.Hash, user.Salt, password)) return null ?? "";

            var token = _helperOAuthToken.GenerateJwtToken(user.Id.ToString());
            return token ?? "";
        }

        public bool VerifyPassword(string storedHash, string storedSalt, string inputPassword)
        {
            var hashedPassword = _helperOAuthToken.EncryptPassword(inputPassword, storedSalt);
            return hashedPassword == storedHash;
        }


        public async Task<bool?> UpdateUser(UpdateUser updateUser)
        {
            return await _userRepository.UpdateUser(updateUser);
        }

        public async Task<bool?> UpdateEmail(int userId, string email)
        {
            return await _userRepository.UpdateEmail(userId, email);
        }

        public async Task<bool?> UpdateUserName(int userId, string userName)
        {
            return await _userRepository.UpdateUserName(userId, userName);
        }
    }
}