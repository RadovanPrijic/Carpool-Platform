﻿using CarpoolPlatformAPI.Models.Domain;
using CarpoolPlatformAPI.Models.DTO.Auth;
using CarpoolPlatformAPI.Models.DTO.Login;
using CarpoolPlatformAPI.Models.DTO.User;
using System.Linq.Expressions;

namespace CarpoolPlatformAPI.Services.IService
{
    public interface IUserService
    {
        Task<bool> isUserUnique(string email);
        Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO);
        Task<UserDTO> Register(RegistrationRequestDTO registrationRequestDTO);
        Task<List<UserDTO>> GetAllUsersAsync(Expression<Func<User, bool>>? filter = null, string? includeProperties = null,
            int pageSize = 0, int pageNumber = 1);
        Task<UserDTO?> GetUserAsync(Expression<Func<User, bool>> filter = null, string? includeProperties = null);
        Task<UserDTO?> UpdateUserAsync(string id, UserUpdateDTO userUpdateDTO);
        Task<UserDTO?> RemoveUserAsync(string id);
    }
}
