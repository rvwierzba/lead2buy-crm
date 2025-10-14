using System.Security.Claims;
using Lead2Buy.API.Data;
using Lead2Buy.API.Dtos.Task;
using Lead2Buy.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lead2Buy.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Todas as operações de tarefas exigem autenticação
    public class TasksController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TasksController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/tasks (retorna todas as tarefas DO USUÁRIO LOGADO)
        [HttpGet]
        public async Task<IActionResult> GetTasks()
        {
            var userId = GetUserId();
            var tasks = await _context.CrmTasks
                .Where(t => t.UserId == userId)
                .Include(t => t.Contact) // Inclui dados do contato associado
                .ToListAsync();

            return Ok(tasks);
        }

        // POST: api/tasks (cria uma nova tarefa PARA O USUÁRIO LOGADO)
        [HttpPost]
        public async Task<IActionResult> CreateTask(CreateTaskDto request)
        {
            var userId = GetUserId();

            // Validação: Verifica se o contato pertence ao usuário logado
            var contactExists = await _context.Contacts.AnyAsync(c => c.Id == request.ContactId);
            if (!contactExists)
            {
                return BadRequest("O contato especificado não existe.");
            }

            var newTask = new CrmTask
            {
                Title = request.Title,
                Description = request.Description,
                DueDate = DateTime.SpecifyKind(request.DueDate, DateTimeKind.Utc),
                ContactId = request.ContactId,
                UserId = userId
            };

            _context.CrmTasks.Add(newTask);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTasks), new { id = newTask.Id }, newTask);
        }

        // PUT: api/tasks/5 (atualiza uma tarefa DO USUÁRIO LOGADO)
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(Guid id, UpdateTaskDto request)
        {
            var userId = GetUserId();
            var task = await _context.CrmTasks.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (task == null)
            {
                return NotFound("Tarefa não encontrada ou não pertence a este usuário.");
            }

            task.Title = request.Title;
            task.Description = request.Description;
            task.DueDate = DateTime.SpecifyKind(request.DueDate.ToUniversalTime(), DateTimeKind.Utc);
            task.IsCompleted = request.IsCompleted;

            await _context.SaveChangesAsync();
            return Ok(task);
        }

        // DELETE: api/tasks/5 (deleta uma tarefa DO USUÁRIO LOGADO)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(Guid id)
        {
            var userId = GetUserId();
            var task = await _context.CrmTasks.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (task == null)
            {
                return NotFound("Tarefa não encontrada ou não pertence a este usuário.");
            }

            _context.CrmTasks.Remove(task);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Método auxiliar para pegar o ID do usuário a partir do token JWT
        private Guid GetUserId()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString))
            {
                throw new Exception("ID do usuário não encontrado no token.");
            }
            return Guid.Parse(userIdString);
        }
    }
}