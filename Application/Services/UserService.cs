using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;
        private readonly JwtTokenGenerator _jwtTokenGenerator;


        public UserService(IUnitOfWork unitOfWork, UserManager<User> userManager, JwtTokenGenerator jwtTokenGenerator)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public async Task RegisterUserAsync(UserRegistrationDto dto)
        {
            var user = new User
            {
                UserName = dto.Username,
                Email = dto.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                Type = dto.Role
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
            {
                throw new Exception("User registration failed");
            }
            await _userManager.AddToRoleAsync(user, dto.Role.ToString());
            await _unitOfWork.CompleteAsync();
            var refreshToken = _jwtTokenGenerator.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);
            await _userManager.UpdateAsync(user);

        }

        public async Task<TokenResponseDto> LoginUserAsync(UserLoginDto dto)
        {
            var user = await _userManager.FindByNameAsync(dto.Username);
            if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password))
            {
                throw new Exception("Invalid credentials");
            }

            var accessToken = await _jwtTokenGenerator.GenerateAccessToken(user);
            var refreshToken = _jwtTokenGenerator.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);
            await _userManager.UpdateAsync(user);

            return new TokenResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }




        public async Task<UserProfileDto> GetUserProfileAsync(string id)
        {
            var user = await _unitOfWork.Users.GetUserByIdAsync(id);
            return new UserProfileDto
            {
                Id = user.Id,
                Username = user.UserName,
                Email = user.Email
            };
        }

        public async Task UpdateUserProfileAsync(string id, UserProfileDto dto)
        {
            var user = await _unitOfWork.Users.GetUserByIdAsync(id);
            user.UserName = dto.Username;
            user.Email = dto.Email;

            await _userManager.UpdateAsync(user);
            await _unitOfWork.CompleteAsync();
        }




        public async Task<TokenResponseDto> RefreshTokenAsync(string token)
        {
            var user = await _userManager.Users.SingleOrDefaultAsync(u => u.RefreshToken == token && u.RefreshTokenExpiryTime > DateTime.Now);

            if (user == null)
            {
                throw new Exception("Invalid refresh token");
            }

            var accessToken = await _jwtTokenGenerator.GenerateAccessToken(user);
            var refreshToken = _jwtTokenGenerator.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);
            await _userManager.UpdateAsync(user);

            return new TokenResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }
    }



}
