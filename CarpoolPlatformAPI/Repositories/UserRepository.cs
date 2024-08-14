using AutoMapper;
using CarpoolPlatformAPI.Data;
using CarpoolPlatformAPI.Models.Domain;
using CarpoolPlatformAPI.Models.DTO.Auth;
using CarpoolPlatformAPI.Models.DTO.Login;
using CarpoolPlatformAPI.Models.DTO.User;
using CarpoolPlatformAPI.Repositories.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CarpoolPlatformAPI.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        private readonly CarpoolPlatformDbContext _db;

        public UserRepository(CarpoolPlatformDbContext db) : base(db)
        {
            _db = db;
        }
    }
}
