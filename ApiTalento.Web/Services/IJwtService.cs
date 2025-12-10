namespace ApiTalento.Web.Services
{
    public interface IJwtService
    {
        string GenerateToken(int employeeId, string email, string documentNumber);
        int? ValidateToken(string token);
    }
}

