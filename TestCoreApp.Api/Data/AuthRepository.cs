using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TestCoreApp.Api.Models;

namespace TestCoreApp.Api.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _context;
        public AuthRepository(DataContext context)
        {
            _context = context;

        }
        public async Task<User> Login(string username, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Username == username);

            if(user == null)
                return null;
            
            if(!VerfyPasswordHAsh(password, user.PasswordHash, user.PasswordSalt))
                return null;

            return user;
        }

        private bool VerfyPasswordHAsh(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using(var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

                for(int i=0; i<computedHash.Length; i++)
                {
                    if(computedHash[i] != passwordHash[i]) return false;
                }
            }
            return true;
            
        }

        public async Task<User> Register(User user, string password)
        {
            byte[] passwordHash, passwrodSalt;
            CreatePasswordHash(password, out passwordHash, out passwrodSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwrodSalt;

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return user;

        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwrodSalt)
        {
            using(var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwrodSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
            
        }

        public async Task<bool> UserExists(string username)
        {
            if(await _context.Users.AnyAsync(x => x.Username == username))
                return true;

            return false;
        }
    }
}