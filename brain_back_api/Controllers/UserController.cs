using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using brain_back_domain.DTOs;
using brain_back_domain.Models;
using brain_back_domain.Entities;
using brain_back_domain.Enumerations;
using brain_back_application.Helpers;
using brain_back_application.Interfaces;

namespace brain_back_api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly HelperOAuthToken _helperOAuthToken;

        public UserController(IUserService userService, HelperOAuthToken helperOAuthToken)
        {
            this._userService = userService;
            this._helperOAuthToken = helperOAuthToken;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Login loginData)
        {
            var token = await _userService.AuthenticateUser(loginData.EmailOrName, loginData.Password);
            if (String.IsNullOrEmpty(token)) return Unauthorized(new ApiResponse<string>
            {
                Response = EApiResponse.Error,
                Message = "Usuario o contraseña incorrectos."
            });
            return Ok(new ApiResponse<string>
            {
                Response = EApiResponse.Success, Message = "Login successful", Data = token
            });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] CreateUser registerData)
        {
            // Verificar si el nombre de usuario ya existe
            bool isExistingName = await _userService.CheckExistingName(registerData.UserName);
            if (isExistingName)
            {
                return BadRequest(new ApiResponse<string>
                {
                    Response = EApiResponse.Conflit,
                    Message = "Username already exists"
                });
            }

            // Verificar si el email ya existe
            bool isExistingEmail = await _userService.CheckExistingEmail(registerData.Email);
            if (isExistingEmail)
            {
                return BadRequest(new ApiResponse<string>
                {
                    Response = EApiResponse.Conflit,
                    Message = "Email already exists"
                });
            }

            // Generar el salt
            string salt = _helperOAuthToken.GenerateSalt();

            // Generar el hash de la contraseña con el salt
            string hashedPassword = _helperOAuthToken.EncryptPassword(registerData.Password, salt);

            User newUser = new User
            {
                Email = registerData.Email,
                UserName = registerData.UserName,
                LastName = registerData.LastName,
                FirstName = registerData.FirstName,
                Salt = salt,
                Hash = hashedPassword
            };

            // Guardar el usuario en la base de datos
            bool isSaved = await _userService.RegisterUser(newUser);
            if (!isSaved)
            {
                return BadRequest(new ApiResponse<string>
                {
                    Response = EApiResponse.Conflit, Message = "Error creating user"
                });
            }

            return Created("", new ApiResponse<string>
            {
                Response = EApiResponse.Success, Message = "User created successfully"
            });
        }

        [HttpGet("verify/name")]
        public async Task<IActionResult> VerifyName([FromQuery] string name)
        {
            bool isExistingName = await _userService.CheckExistingName(name);
            string message = isExistingName ? "Username already exists" : "Username available";
            return Ok(new ApiResponse<bool>
            {
                Response = EApiResponse.Success, Data = isExistingName, Message = message
            });
        }

        [HttpGet("verify/email")]
        public async Task<IActionResult> VerifyEmail([FromQuery] string email)
        {
            string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (!Regex.IsMatch(email, emailPattern))
            {
                return BadRequest(new ApiResponse<string>
                {
                    Response = EApiResponse.Error, Message = "Invalid email format"
                });
            }

            bool isExistingEmail = await _userService.CheckExistingEmail(email);
            string message = isExistingEmail ? "Username already exists" : "Username available";
            return Ok(new ApiResponse<bool>
            {
                Response = EApiResponse.Success, Data = isExistingEmail, Message = message
            });
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update([FromBody] UpdateUser updateUser)
        {
            bool? isUpdated = await _userService.UpdateUser(updateUser);
            switch (isUpdated)
            {
                case null:
                    return BadRequest(new ApiResponse<string>
                    {
                        Response = EApiResponse.Error, Message = "User not found"
                    });
                case false:
                    return BadRequest(new ApiResponse<string>
                    {
                        Response = EApiResponse.Error, Message = "Error updating user"
                    });
                case true:
                    return Ok(new ApiResponse<string>
                    {
                        Response = EApiResponse.Success, Message = "User updated successfully"
                    });
            }
        }

        [HttpPut("update/email")]
        public async Task<IActionResult> UpdateEmail([FromBody] int userId, string updateEmail)
        {
            bool? isUpdated = await _userService.UpdateEmail(userId, updateEmail);
            switch (isUpdated)
            {
                case null:
                    return BadRequest(new ApiResponse<string>
                    {
                        Response = EApiResponse.Error, Message = "User not found"
                    });
                case false:
                    return BadRequest(new ApiResponse<string>
                    {
                        Response = EApiResponse.Error, Message = "Error updating email"
                    });
                case true:
                    return Ok(new ApiResponse<string>
                    {
                        Response = EApiResponse.Success, Message = "Email updated successfully"
                    });
            }
        }

        [HttpPut("update/userName")]
        public async Task<IActionResult> UpdateUserName([FromBody] int userId, string updateUserName)
        {
            bool? isUpdated = await _userService.UpdateUserName(userId, updateUserName);
            switch (isUpdated)
            {
                case null:
                    return BadRequest(new ApiResponse<string>
                    {
                        Response = EApiResponse.Error, Message = "User not found"
                    });
                case false:
                    return BadRequest(new ApiResponse<string>
                    {
                        Response = EApiResponse.Error, Message = "Error updating userName"
                    });
                case true:
                    return Ok(new ApiResponse<string>
                    {
                        Response = EApiResponse.Success, Message = "Username updated successfully"
                    });
            }
        }


    }
}