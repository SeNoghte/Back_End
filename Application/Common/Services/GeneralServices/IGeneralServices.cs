using Domain.Entities;

namespace Application.Common.Services.GeneralServices
{
    public interface IGeneralServices
    {
        public bool CheckEmailFromat(string email);
        public bool CheckPasswordFormat(string password);
        public Task<bool> CheckUserExists(Guid userId);
        public Task<Group> GetGroup(Guid groupId);
    }
}