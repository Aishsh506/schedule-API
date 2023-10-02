﻿using AccountBL.DTO;
using AccountBL.Models;
using AccountDAL;
using AccountDAL.Entities;
using Common.Exceptions;
using Common.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AccountBL
{
    public interface IAccountService
    {
        Task<TokenResponseDTO> Register(RegistrationModel data);
        Task<TokenResponseDTO?> Login(LoginModel data);
        Task<TokenResponseDTO?> Refresh(string token);
        Task<AccountDTO> GetAccount(string userId);
    }
    public class AccountService : IAccountService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly AccountContext _context;
        private readonly IOptions<JwtOptions> _jwtOptions;
        public AccountService(UserManager<AppUser> userManager, AccountContext context, IOptions<JwtOptions> jwtOptions)
        {
            _userManager = userManager;
            _context = context;
            _jwtOptions = jwtOptions;
        }
        public async Task<TokenResponseDTO> Register(RegistrationModel data)
        {
            var existingUser = await _userManager.FindByEmailAsync(data.Email);
            if (existingUser != null)
            {
                throw new DataConflictException();
            }

            var newUser = new AppUser()
            {
                Email = data.Email,
                UserName = data.Username
            };
            var result = await _userManager.CreateAsync(newUser, data.Password);
            if (!result.Succeeded)
            {
                var message = string.Join(" ", result.Errors.Where(e => e.Code.StartsWith("Password")).Select(e => e.Description));
                if (message.Length > 0)
                {
                    throw new InvalidPasswordException(message);
                }
                else throw new DataWriteException();
            };

            return await GenerateTokenResponseDTO(newUser, Array.Empty<string>());
        }
        public async Task<TokenResponseDTO?> Login(LoginModel data)
        {
            var user = await FindUserByEmail(data.Email);

            if (await _userManager.CheckPasswordAsync(user, data.Password))
            {
                var roles = (await _userManager.GetRolesAsync(user)).ToArray();
                await _userManager.RemoveAuthenticationTokenAsync(user, "App", "Refresh");
                return await GenerateTokenResponseDTO(user, roles);
            }
            else return null;
        }
        public async Task<TokenResponseDTO?> Refresh(string token)
        {

            var user = await FindUserById(GetUserIdFromToken(token));

            var storedToken = await _userManager.GetAuthenticationTokenAsync(user, "App", "Refresh");
            if (storedToken is null)
            {
                throw new KeyNotFoundException();
            }

            var tokenExpired = false;
            if (await _userManager.VerifyUserTokenAsync(user, "App", "Refresh", storedToken))
            {
                await _userManager.RemoveAuthenticationTokenAsync(user, "App", "Refresh");
                tokenExpired = true;
            }

            if (storedToken == token)
            {
                if (tokenExpired)
                {
                    throw new SecurityTokenExpiredException();
                }

                var roles = (await _userManager.GetRolesAsync(user)).ToArray();
                var accessToken = GetAccessToken(user, roles);
                return new TokenResponseDTO()
                {
                    AccessToken = new JwtSecurityTokenHandler().WriteToken(accessToken),
                    RefreshToken = storedToken,
                    Email = user.Email,
                    Role = roles.ToList()
                };
            }
            else
            {
                throw new KeyNotFoundException();
            }
        }
        public async Task<AccountDTO> GetAccount(string userId)
        {
            var user = await FindUserById(userId);
            return new AccountDTO(user);
        }
        private async Task<AppUser> FindUserById(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
            {
                throw new KeyNotFoundException();
            }
            return user;
        }
        private async Task<AppUser> FindUserByEmail(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null)
            {
                throw new KeyNotFoundException();
            }
            return user;
        }

        private string? GetUserIdFromToken(string token)
        {
            var jwt = new JwtSecurityTokenHandler().ReadToken(token) as JwtSecurityToken;
            if (jwt == null)
            {
                throw new InvalidDataException();
            }
            var emailClaim = jwt.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Uri);
            if (emailClaim == null)
            {
                throw new InvalidDataException();
            }
            return emailClaim.Value;
        }
        private async Task<TokenResponseDTO> GenerateTokenResponseDTO(AppUser user, string[] roles)
        {
            var accessToken = GetAccessToken(user, roles);
            var refreshToken = new JwtSecurityTokenHandler().WriteToken(GetRefreshToken(user));
            await _userManager.SetAuthenticationTokenAsync(user, "App", "Refresh", refreshToken);

            return new TokenResponseDTO()
            {
                AccessToken = new JwtSecurityTokenHandler().WriteToken(accessToken),
                RefreshToken = refreshToken,
                Email = user.Email,
                Role = roles.ToList()
            };
        }
        private JwtSecurityToken GetAccessToken(AppUser user, string[] roles)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Uri, user.Id),
                new Claim(ClaimTypes.Email, user.Email)
            };
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Value.Secret));

            var token = new JwtSecurityToken(
                issuer: _jwtOptions.Value.ValidIssuer,
                expires: DateTime.Now.AddSeconds(_jwtOptions.Value.AccessLifetime),
                claims: claims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256));

            return token;
        }
        private JwtSecurityToken GetRefreshToken(AppUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Uri, user.Id)
            };

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Value.Secret));

            var token = new JwtSecurityToken(
                issuer: _jwtOptions.Value.ValidIssuer,
                expires: DateTime.Now.AddSeconds(_jwtOptions.Value.RefreshLifetime),
                claims: claims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256));

            return token;
        }
    }
}
