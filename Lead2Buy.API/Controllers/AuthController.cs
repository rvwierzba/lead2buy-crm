using Lead2Buy.API.Data;
using Lead2Buy.API.Dtos.User;
using Lead2Buy.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace Lead2Buy.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("register")] // Rota: POST /api/auth/register
        public async Task<IActionResult> Register(RegisterUserDto request)
        {
            // 1. Verifica se já existe um usuário com o mesmo email
            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            {
                return BadRequest("Um usuário com este e-mail já existe.");
            }

            // 2. Cria o hash da senha
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            // 3. Cria a nova entidade de Usuário
            var newUser = new User
            {
                Name = request.Name,
                Email = request.Email,
                PasswordHash = passwordHash
            };

            // 4. Adiciona o novo usuário ao banco de dados e salva
            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            // 5. Retorna uma resposta de sucesso
            return Ok("Usuário registrado com sucesso.");
        }

        [HttpPost("login")] // Rota: POST /api/auth/login
        public async Task<IActionResult> Login(LoginUserDto request)
        {
            // 1. Procura o usuário pelo email
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

            // 2. Verifica se o usuário existe e se a senha está correta
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return Unauthorized("Credenciais inválidas.");
            }

            // 3. Se as credenciais estiverem corretas, cria o token JWT
            string token = CreateToken(user);

            // 4. Retorna o token
            return Ok(new { token });
        }


        private string CreateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email)
            };

            // CORREÇÃO AQUI: Usando _configuration para ler a chave do appsettings.json
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration.GetSection("Jwt:Key").Value!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            // E CORREÇÃO AQUI TAMBÉM
            var token = new JwtSecurityToken(
                issuer: _configuration.GetSection("Jwt:Issuer").Value,
                audience: _configuration.GetSection("Jwt:Audience").Value,
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds
            );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }


    }
}