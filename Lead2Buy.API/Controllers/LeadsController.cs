using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Lead2Buy.API.Data;
using Lead2Buy.API.DTOs.Timeline;

namespace Lead2Buy.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LeadsController : ControllerBase
    {
        private readonly AppDbContext _db;
        public LeadsController(AppDbContext db) => _db = db;

        /// <summary>
        /// Retorna a timeline consolidada de um lead (tarefas, interações, atividades).
        /// </summary>
        [HttpGet("{contactId}/timeline")]
public async Task<IActionResult> GetTimeline(Guid contactId)
{
    var interactions = await _db.Interactions
        .Where(i => i.ContactId == contactId)
        .Select(i => new TimelineItemDto
        {
            Kind = "interaction",
            When = i.CreatedAt, // usa CreatedAt no lugar de OccurredAt
            Title = i.Type,
            Details = i.Notes
        })
        .ToListAsync();

    var tasks = await _db.CrmTasks
        .Where(t => t.ContactId == contactId)
        .Select(t => new TimelineItemDto
        {
            Kind = "task",
            When = t.DueDate,
            Title = t.Title,
            Details = t.Description
        })
        .ToListAsync();

    var activities = await _db.UserActivities
        .Where(a => a.ContactId == contactId)
        .Select(a => new TimelineItemDto
        {
            Kind = "activity",
            When = a.OccurredAt,
            Title = a.ActionType,
            Details = a.Details
        })
        .ToListAsync();

    // Agora todos são List<TimelineItem>, então o Concat funciona
    var timeline = interactions
        .Concat(tasks)
        .Concat(activities)
        .OrderByDescending(x => x.When)
        .ToList();

    return Ok(timeline);
}

        
    }
}
