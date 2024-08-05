using EmployeeManagement.Domain.Entities;
using System.Threading.Tasks;

namespace EmployeeManagement.Infrastructure.Repositories
{
    public interface IUserRepository
    {
        Task<User> GetUserByUsernameAsync(string username);
        Task<User> AddUserAsync(User user);
    }
}
