using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IUserService
    {
        Task RegisterUserAsync(UserRegistrationDto dto);
        Task<TokenResponseDto> LoginUserAsync(UserLoginDto dto);
        Task<UserProfileDto> GetUserProfileAsync(string id);
        Task UpdateUserProfileAsync(string id, UserProfileDto dto);
        Task<TokenResponseDto> RefreshTokenAsync(string refreshToken);
    }
}
