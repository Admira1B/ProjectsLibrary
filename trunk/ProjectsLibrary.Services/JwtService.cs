using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ProjectsLibrary.Domain.Contracts.Services;
using ProjectsLibrary.Domain.Models.Entities;
using ProjectsLibrary.Domain.Models.Enums;
using ProjectsLibrary.Domain.Models.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ProjectsLibrary.Services {
    public class JwtService(IOptions<JwtOptions> options) : IJwtService {
        private readonly JwtOptions _options = options.Value;
        public string GenerateWebToken(Employee employee) {
            Claim[] claims =
            [
                new("employeeId", $"{employee.Id}"),
                new("employeeEmail", $"{employee.Email}"),
                new("employeeName", $"{employee.FirstName}  {employee.LastName}"),
                new("employeeRole", $"{employee.Role.RoleToString()}"),
            ];

            var signingCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey)),
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                signingCredentials: signingCredentials,
                expires: DateTime.UtcNow.AddHours(_options.ExpiresHours),
                claims: claims);

            var result = new JwtSecurityTokenHandler().WriteToken(token);

            return result;
        }
    }
}
