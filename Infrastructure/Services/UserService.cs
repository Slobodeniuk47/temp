﻿using AutoMapper;
using Core.Constants;
using Core.Entities.Identity;
using Core.Interfaces;
using Google.Apis.Auth;
using Infrastructure.Helpers;
using Infrastructure.Interfaces;
using Infrastructure.Models.DTO.AccountDTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private IJwtTokenService _jwtTokenService;
        private readonly IMapper _mapper;

        public UserService(IUserRepository userRepository, IJwtTokenService jwtTokenService, IMapper mapper)
        {
            _userRepository = userRepository;
            _jwtTokenService = jwtTokenService;
            _mapper = mapper;
        }
        public async Task<ServiceResponse> RegisterUserAsync(UserCreateDTO model)
        {
            var checkEmail = await _userRepository.GetUserByEmailAsync(model.Email);
            if (checkEmail != null)
            {
                return new ServiceResponse
                {
                    Message = "Email already exists",
                    IsSuccess = false
                };
            }
            if (model.Password != model.ConfirmPassword)
            {
                return new ServiceResponse
                {
                    Message = "Passwords do not match!",
                    IsSuccess = false,
                };
            }
            var user = _mapper.Map<UserEntity>(model);
            user.Image = await ImageHelper.SaveImageAsync(model.Image, DirectoriesInProject.UserImages);


            var result = await _userRepository.CreateUserAsync(user, model.Password);

            if (!result.Succeeded)
            {
                return new ServiceResponse
                {
                    Message = "Is not successed!",
                    IsSuccess = false,
                    Errors = result.Errors.Select(e => e.Description)
                };
            }
            var role = model.Role != null ? model.Role : "User";
            await _userRepository.AddRoleAsync(user, role);

            var token = await _jwtTokenService.CreateTokenAsync(user);
            return new ServiceResponse
            {
                Message = "User successfully created!",
                Payload = token,
                IsSuccess = true
            };
        }
        public async Task<ServiceResponse> EditUserAsync(UserEditDTO model)
        {
            var oldUser = await _userRepository.GetUserByIdAsync(model.id.ToString());
            var user = _mapper.Map(model, oldUser);


            if (user != null)
            {
                var checkEmail = await _userRepository.GetUserByEmailAsync(model.Email);
                if (checkEmail != null && checkEmail.Id != model.id)
                {
                    return new ServiceResponse
                    {
                        Message = "Email already exists",
                        IsSuccess = false
                    };
                }

                if (model.ImageUpload != null)
                {
                    ImageHelper.DeleteImage(oldUser.Image, DirectoriesInProject.UserImages);
                    user.Image = await ImageHelper.SaveImageAsync(model.ImageUpload, DirectoriesInProject.UserImages);
                }
                if (model.Role != null)
                {

                    var oldRoles = await _userRepository.GetRolesAsync(oldUser);
                    await _userRepository.RemoveRolesAsync(user, oldRoles);

                    var role = model.Role != null ? model.Role : Roles.User;
                    await _userRepository.AddRoleAsync(user, role);
                }

                await _userRepository.UpdateUserAsync(user);
                return new ServiceResponse
                {
                    Message = "The user was successfully updated!",
                    IsSuccess = true,
                    Payload = "ok"
                };
            }

            return new ServiceResponse
            {
                Message = "The user was not successfully updated!",
                IsSuccess = false,
            };
        }

        public async Task<ServiceResponse> LoginUserAsync(LoginDTO model)
        {
            var user = await _userRepository.GetUserByEmailAsync(model.Email);

            if (user == null)
            {
                return new ServiceResponse
                {
                    Message = "The user is not registered.",
                    IsSuccess = false
                };
            }

            var result = await _userRepository.ValidateLoginAsync(model.Email, model.Password);
            if (!result)
            {
                return new ServiceResponse
                {
                    Message = "Incorrect password.",
                    IsSuccess = false
                };
            }

            var tokens = await _jwtTokenService.CreateTokenAsync(user);

            return new ServiceResponse
            {
                Message = "Authentication was successful.",
                Payload = tokens,
                IsSuccess = true,
            };
        }
        public async Task<ServiceResponse> GoogleExternalLogin(ExternalLoginDTO model)
        {
            //Install packet Google.Apis.Auth
            //So that the backend checks whether the user is authorized through Google
            var settings = new GoogleJsonWebSignature.ValidationSettings()
            {
                Audience = new List<string>()
                {
                    "85911906235-mpbk79c4do3jhbf2drgemm9q2n2sd6ca.apps.googleusercontent.com"
                }
            };

            var payload = await GoogleJsonWebSignature.ValidateAsync(model.Token, settings);
            if (payload != null)
            {
                var info = new UserLoginInfo(model.Provider, payload.Subject, model.Provider);
                var user = await _userRepository.GetUserByLoginAsync(info.LoginProvider, info.ProviderKey);
                if (user == null)
                {
                    user = await _userRepository.GetUserByEmailAsync(payload.Email);
                    if (user == null)
                    {
                        user = new UserEntity()
                        {
                            Email = payload.Email,
                            UserName = payload.Email,
                            FirstName = payload.GivenName,
                            LastName = payload.FamilyName,
                            Image = payload.Picture,
                            IsGoogle = true
                        };
                        var resultCreate = await _userRepository.CreateUserAsync(user);
                        if (!resultCreate.Succeeded)
                        {
                            return new ServiceResponse
                            {
                                IsSuccess = false,
                                Message = "Something went wrong"
                            };
                        }
                        if (user.Permissions == null)
                        {
                            await _userRepository.AddRoleAsync(user, Roles.User);
                        }
                    }
                    var resultAddLogin = await _userRepository.AddLoginAsync(user, info);
                    if (!resultAddLogin.Succeeded)
                    {
                        return new ServiceResponse
                        {
                            IsSuccess = false,
                            Message = "Something went wrong"
                        };
                    }

                }

                var result = new { Id = user.Id, email = user.Email, firstname = user.FirstName, lastname = user.LastName, phoneNumber = user.PhoneNumber, image = user.Image };
                var token = await _jwtTokenService.CreateTokenAsync(user);
                return new ServiceResponse
                {
                    IsSuccess = true,
                    Payload = token
                };
            }
            return new ServiceResponse
            {
                IsSuccess = false,
                Message = "Something went wrong"
            };

        }
        public async Task<ServiceResponse> ConfirmEmailAsync(string userId, string token)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
                return new ServiceResponse
                {
                    IsSuccess = false,
                    Message = "User does not exist."
                };

            var decodedToken = WebEncoders.Base64UrlDecode(token);
            string normalToken = Encoding.UTF8.GetString(decodedToken);

            var result = await _userRepository.ConfirmEmailAsync(user, normalToken);

            if (result.Succeeded)
                return new ServiceResponse
                {
                    Message = "The email was successfully verified!",
                    IsSuccess = true,
                };

            return new ServiceResponse
            {
                IsSuccess = false,
                Message = "The email has not been verified.",
                Errors = result.Errors.Select(e => e.Description)
            };
        }

        public async Task<ServiceResponse> GetAllUsersAsync()
        {
            var users = await _userRepository.Users.Select(user => new UserItemDTO
            {
                Id = user.Id,
                Firstname = user.FirstName,
                Lastname = user.LastName,
                Email = user.Email,
                phoneNumber = user.PhoneNumber,
                Image = user.Image,
                ImageURL = user.IsGoogle == false ? $"{DirectoriesInProject.Api}/{DirectoriesInProject.UserImages}/{user.Image}" : user.Image,
                Permissions = user.Permissions.Select(perm => new PermissionItemDTO { RoleName = perm.Role.Name }).ToList()
            }).ToListAsync();
            return new ServiceResponse
            {
                Message = "All users successfully loaded.",
                IsSuccess = true,
                Payload = users
            };
        }
        public async Task<ServiceResponse> GetUserByIdAsync(long id)
        {
            var users = await _userRepository.Users.Where(user => user.Id == id)
            .Select(user => new UserItemDTO
            {
                Id = user.Id,
                Firstname = user.FirstName,
                Lastname = user.LastName,
                Email = user.Email,
                phoneNumber = user.PhoneNumber,
                Image = user.Image,
                ImageURL = user.IsGoogle == false ? $"{DirectoriesInProject.Api}/{DirectoriesInProject.UserImages}/{user.Image}" : user.Image,
                Permissions = user.Permissions.Select(perm => new PermissionItemDTO { RoleName = perm.Role.Name }).ToList()
            }).ToListAsync();

            if (users != null)
            {
                return new ServiceResponse()
                {
                    Payload = users[0],
                    Message = "User found",
                    IsSuccess = true,
                };
            }
            return new ServiceResponse()
            {
                Message = "User does not exist",
                IsSuccess = false,
            };
        }
        public async Task<ServiceResponse> DeleteUserByIdAsync(long id)
        {
            var user = await _userRepository.GetUserByIdAsync(id.ToString());
            if (user != null)
            {
                ImageHelper.DeleteImage(user.Image, DirectoriesInProject.UserImages);
                await _userRepository.DeleteUserAsync(user);

                return new ServiceResponse()
                {
                    IsSuccess = false,
                    Message = "The User has been deleted"
                };
            }
            return new ServiceResponse()
            {
                IsSuccess = true,
                Message = "User does not exist"
            };
        }
    }
}
