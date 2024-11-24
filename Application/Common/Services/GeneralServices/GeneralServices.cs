using DataAccess;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Application.Common.Services.GeneralServices
{
    public class GeneralServices : IGeneralServices
    {
        string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        string passwordPattern = @"^[a-zA-Z0-9!@#\$%\^&\*\(\)_\+\-=\[\]{};':""\\|,.<>\/?]{8,}$";
        string usernamePattern = @"^[a-zA-Z0-9_-]{2,20}$";
        private readonly ApplicationDBContext dBContext;

        public GeneralServices(ApplicationDBContext dBContext)
        {
            this.dBContext = dBContext;
        }


        public bool CheckEmailFromat(string email)
        {
            return Regex.IsMatch(email, emailPattern, RegexOptions.IgnoreCase);
        }

        public bool CheckPasswordFormat(string password)
        {
            var regex = new Regex(passwordPattern);
            return regex.IsMatch(password);
        }

        public async Task<bool> CheckUserExists(Guid userId)
        {
            return await dBContext.Users.AnyAsync(u => u.UserId == userId);
        }

        public bool CheckUsernameFormat(string username)
        {
            return Regex.IsMatch(username, usernamePattern, RegexOptions.IgnoreCase);
        }

        public async Task<Domain.Entities.Group> GetGroup(Guid groupId)
        {
            return await dBContext.Groups.Where(gp => gp.Id == groupId).FirstOrDefaultAsync();
        }

        public async Task<User> GetUser(Guid userId)
        {
            return await dBContext.Users.Where(u => u.UserId == userId).FirstOrDefaultAsync();
        }
    }
}
