using Lead2Buy.API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lead2Buy.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MetricsController : ControllerBase
    {
        private readonly AppDbContext _db;
        public MetricsController(AppDbContext db) => _db = db;

        /// <summary>
        /// Resumo do funil: quantos contatos estão em cada etapa.
        /// </summary>
        [HttpGet("funnel-summary")]
        public async Task<IActionResult> GetFunnelSummary()
        {
            var summary = await _db.Contacts
                .GroupBy(c => c.FunnelStageId)
                .Select(g => new
                {
                    stageId = g.Key,
                    count = g.Count()
                })
                .ToListAsync();

            return Ok(summary);
        }

        /// <summary>
        /// Atividades realizadas por usuário nos últimos X dias.
        /// </summary>
        [HttpGet("user-activities")]
        public async Task<IActionResult> GetUserActivities([FromQuery] int days = 30)
        {
            var since = DateTime.UtcNow.AddDays(-days);

            var activities = await _db.UserActivities
                .Where(a => a.OccurredAt >= since)
                .GroupBy(a => new { a.UserId, a.ActionType })
                .Select(g => new
                {
                    g.Key.UserId,
                    g.Key.ActionType,
                    count = g.Count()
                })
                .ToListAsync();

            return Ok(activities);
        }

        /// <summary>
        /// Taxa de conversão: quantos contatos chegaram à última etapa do funil.
        /// </summary>
        [HttpGet("conversion-rate")]
        public async Task<IActionResult> GetConversionRate()
        {
            var total = await _db.Contacts.CountAsync();
            if (total == 0) return Ok(new { conversionRate = 0 });

            // supondo que a última etapa do funil seja "Concluído"
            var lastStage = await _db.FunnelStages
                .OrderByDescending(f => f.Order)
                .FirstOrDefaultAsync();

            if (lastStage == null) return Ok(new { conversionRate = 0 });

            var converted = await _db.Contacts
                .CountAsync(c => c.FunnelStageId == lastStage.Id);

            var rate = (double)converted / total * 100;

            return Ok(new { conversionRate = rate });
        }
    }
}
