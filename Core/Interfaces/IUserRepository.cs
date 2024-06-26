﻿using Core.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IUserRepository
    {
        IQueryable<UserEntity> GetAll();
        IQueryable<UserEntity> Users { get; }
        Task<IdentityResult> ChangePasswordAsync(UserEntity user, string currentPass, string newPass);
        Task<IdentityResult> CreateUserAsync(UserEntity user, string password);
        Task<IdentityResult> CreateUserAsync(UserEntity user);
        Task<bool> ValidateLoginAsync(string email, string password);
        Task<UserEntity> GetUserByIdAsync(string id);
        Task<UserEntity> GetUserByLoginAsync(string loginProvider, string providerKey);
        Task<IdentityResult> AddLoginAsync(UserEntity user, UserLoginInfo loginInfo);
        Task<IdentityResult> UpdateUserAsync(UserEntity model);
        Task<UserEntity> GetUserByEmailAsync(string email);
        Task<IList<string>> GetRolesAsync(UserEntity model);
        Task<IdentityResult> ConfirmEmailAsync(UserEntity model, string token);
        Task<string> GeneratePasswordResetTokenAsync(UserEntity model);
        Task<string> GenerateEmailConfirmationTokenAsync(UserEntity appUser);
        Task<List<UserEntity>> GetAllUsersAsync();
        Task<IdentityResult> AddRoleAsync(UserEntity model, string role);
        Task<IdentityResult> RemoveRolesAsync(UserEntity model, IList<string> role);
        Task<IdentityResult> DeleteUserAsync(UserEntity user);

    }
}
