using Lead2Buy.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Lead2Buy.API.Data;

namespace Lead2Buy.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CalendarController : ControllerBase
    {
        private readonly AppDbContext _db;
        public CalendarController(AppDbContext db) => _db = db;

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserCalendar(Guid userId, [FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            var tasks = await _db.CrmTasks
                .Where(t => t.UserId == userId && t.DueDate >= from && t.DueDate <= to)
                .Include(t => t.Contact)
                .OrderBy(t => t.DueDate)
                .Select(t => new
                {
                    id = t.Id,
                    title = t.Title,
                    description = t.Description,
                    start = t.DueDate,
                    end = t.DueDate,
                    isCompleted = t.IsCompleted,
                    contactId = t.ContactId,
                    contactName = t.Contact != null ? t.Contact.Name : null
                })
                .ToListAsync();

            return Ok(tasks);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTask([FromBody] CrmTask task)
        {
            task.Id = Guid.Empty; // Postgres generates via gen_random_uuid()
            task.CreatedAt = DateTime.UtcNow;

            _db.CrmTasks.Add(task);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUserCalendar),
                new { userId = task.UserId, from = task.DueDate, to = task.DueDate }, task);
        }

        [HttpPut("{taskId}/complete")]
        public async Task<IActionResult> CompleteTask(Guid taskId)
        {
            var task = await _db.CrmTasks.FindAsync(taskId);
            if (task == null) return NotFound();

            task.IsCompleted = true;
            await _db.SaveChangesAsync();

            _db.UserActivities.Add(new UserActivity
            {
                UserId = task.UserId,
                ContactId = task.ContactId,
                OccurredAt = DateTime.UtcNow,
                ActionType = "TaskCompleted",
                Details = task.Title
            });
            await _db.SaveChangesAsync();

            return Ok(task);
        }

        [HttpDelete("{taskId}")]
        public async Task<IActionResult> DeleteTask(Guid taskId)
        {
            var task = await _db.CrmTasks.FindAsync(taskId);
            if (task == null) return NotFound();

            _db.CrmTasks.Remove(task);
            await _db.SaveChangesAsync();

            return NoContent();
        }
    }
}
