using BorrowdungAPI.Models.Entities;

namespace BorrowdungAPI.Services
{
    public interface IJwtService
    {
        string GenerateToken(User user);
        DateTime GetTokenExpiry();
    }
}
