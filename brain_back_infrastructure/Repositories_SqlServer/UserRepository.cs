using brain_back_domain.DTOs;
using brain_back_domain.Entities;
using brain_back_infrastructure.Data;
using brain_back_infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace brain_back_infrastructure.Repositories_SqlServer
{
    public class UserRepository : IUserRepository
    {
        
        private BrainContext _context;

        public UserRepository(BrainContext context) {
            this._context = context;
        }

        public async Task<User?> GetUserByUserName(string userName)
        {
            return await this._context.Users.FirstOrDefaultAsync(u => u.UserName.ToLower() == userName.ToLower());
        }

        public async Task<User?> GetUserByEmail(string email)
        {
            return await this._context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
        }

        public async Task<bool> RegisterUser(User newUser)
        {
            try
            {
                var lastUser = await _context.Users.OrderByDescending(u => u.Id).FirstOrDefaultAsync();
                newUser.Id = (lastUser?.Id ?? 0) + 1;

                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public async Task<bool> DeleteUser(int userId)
        {
            this._context.Users.Remove(new User { Id = userId });
            return await this._context.SaveChangesAsync() > 0;
        }

        public async Task<bool?> UpdateUser(UpdateUser updateUser)
        {
            User? user = this._context.Users.FirstOrDefault(u => u.Id == updateUser.Id);
            if (user == null) return null;

            user.FirstName = updateUser.FirstName;
            user.LastName = updateUser.LastName;

            this._context.Users.Update(user);
            return await this._context.SaveChangesAsync() > 0;
        }

        public async Task<bool?> UpdateEmail(int userId, string email)
        {
            User? user = this._context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null) return null;
            if (user.EmailTimes >= 3) return false;

            long lastDay = user.EmailLastDay.Ticks;
            long today = DateTime.Now.Ticks;

            if (lastDay == 0 || lastDay < today - 12960000000000)
            {
                user.EmailTimes++;
                user.Email = email;
                user.EmailLastDay = DateTime.Now;
                return await this._context.SaveChangesAsync() < 0;
            }

            return false;
        }

        public async Task<bool?> UpdateUserName(int userId, string userName)
        {
            User? user = this._context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null) return null;
            if (user.UserNameTimes >= 3) return false;

            long lastDay = user.UserNameLastDay.Ticks;
            long today = DateTime.Now.Ticks;

            if (lastDay == 0 || lastDay < today - 12960000000000)
            {
                user.UserNameTimes++;
                user.UserName = userName;
                user.UserNameLastDay = DateTime.Now;
                return await this._context.SaveChangesAsync() < 0;
            }

            return false;
        }
    }
}