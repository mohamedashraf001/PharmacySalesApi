using AutoMapper;
using BusinessLayer.Requests;
using BusinessLayer.Responses;
using DataAccessLayer.Data;
using DataAccessLayer.Entities;
using HMS.Api.App.Options;
using HMS.API.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using SharedClasses.Responses;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Http;
using System;
using System.Security.Cryptography;

namespace LMSApi.App.Controllers.Auth
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;
        private readonly ILogger<AuthController> _logger;
        private readonly JwtOptions _jwtOptions;
        private readonly IMapper _mapper;
        private readonly PasswordHasher<User> _passwordHasher = new PasswordHasher<User>();

        public AuthController(AppDbContext appDbContext, ILogger<AuthController> logger, JwtOptions jwtOptions, IMapper mapper)
        {
            _appDbContext = appDbContext;
            _logger = logger;
            _jwtOptions = jwtOptions;
            _mapper = mapper;
        }

        [HttpPost]
        [Route("login")]
        public async Task<ActionResult<ApiResponse<object>>> Login(loginRequest request)
        {
            var user = await _appDbContext.Users.Include(u => u.Roles)
                .ThenInclude(r => r.Permissions)
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null)
                return NotFound(ApiResponseFactory.Create("User not found", 404, false));

            if (!VerifyPassword(user.Password, request.Password))
                return BadRequest(ApiResponseFactory.Create("Invalid password", 400, false));

            var token = GenerateJwtToken(user, _jwtOptions);
            var refreshToken = GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7); // Set expiry to 7 days
            await _appDbContext.SaveChangesAsync();

            return Ok(ApiResponseFactory.Create(new { Token = token, RefreshToken = refreshToken }, "Login successful", 200, true));
        }

        [HttpPost]
        [Route("register")]
        public async Task<ActionResult<ApiResponse<object>>> Register(UserRequest request)
        {
            // Check if user already exists
            var existingUser = await _appDbContext.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (existingUser != null)
                return BadRequest(ApiResponseFactory.Create("User already exists", 400, false));

            var userRole = await _appDbContext.Roles.Include(u => u.Permissions).SingleOrDefaultAsync(r => r.Name == "User");
            if (userRole == null)
            {
                userRole = new Role { Name = "User" };
                await _appDbContext.Roles.AddAsync(userRole);
                await _appDbContext.SaveChangesAsync();
            }

            // Manually map UserRequest to User
            User user = new User
            {
                Name = request.Name,
                Email = request.Email,
                Password = _passwordHasher.HashPassword(new User(), request.Password),
                Roles = new List<Role> { userRole },
                RefreshToken = GenerateRefreshToken(),
                RefreshTokenExpiry = DateTime.UtcNow.AddDays(7)
            };

            await _appDbContext.Users.AddAsync(user);
            await _appDbContext.SaveChangesAsync();

            return Ok(ApiResponseFactory.Create(new { Token = GenerateJwtToken(user, _jwtOptions), RefreshToken = user.RefreshToken }, "User registered successfully", 201, true));
        }
       
        [HttpPost]
        [Route("refresh")]
        public async Task<ActionResult<ApiResponse<object>>> Refresh(string refreshToken)
        {
            var user = await _appDbContext.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);

            if (user == null || user.RefreshTokenExpiry <= DateTime.UtcNow)
            {
                return BadRequest(ApiResponseFactory.Create("Invalid or expired refresh token", 400, false));
            }

            // Generate new access and refresh tokens
            var newToken = GenerateJwtToken(user, _jwtOptions);
            var newRefreshToken = GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
            await _appDbContext.SaveChangesAsync();

            return Ok(ApiResponseFactory.Create(new { Token = newToken, RefreshToken = newRefreshToken }, "Token refreshed successfully", 200, true));
        }

        private string GenerateRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }

        private string GenerateJwtToken(User user, JwtOptions jwtOptions)
        {
            var userPermissions = user.Roles.SelectMany(r => r.Permissions).Select(p => p.RouteName).ToList();

            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name)
            };

            foreach (var permission in userPermissions)
            {
                claims.Add(new Claim("permissions", permission));
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = jwtOptions.Issuer,
                Audience = jwtOptions.Audience,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key)), SecurityAlgorithms.HmacSha256),
                Subject = new ClaimsIdentity(claims)
            };

            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(securityToken);
        }

        private bool VerifyPassword(string hashedPassword, string providedPassword)
        {
            var result = _passwordHasher.VerifyHashedPassword(null, hashedPassword, providedPassword);
            return result == PasswordVerificationResult.Success;
        }
    }
}
