using Lead2Buy.API.Data;
using Lead2Buy.API.DTOs.BI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lead2Buy.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InsightsController : ControllerBase
    {
        private readonly AppDbContext _db;
        public InsightsController(AppDbContext db) => _db = db;

        [HttpGet("overview")]
        public async Task<IActionResult> GetOverview([FromQuery] int days = 30)
        {
            var since = DateTime.UtcNow.AddDays(-days);

            var totalContacts = await _db.Contacts.CountAsync();

            var lastStage = await _db.FunnelStages
                .OrderByDescending(f => f.Order)
                .FirstOrDefaultAsync();

            var converted = lastStage != null
                ? await _db.Contacts.CountAsync(c => c.FunnelStageId == lastStage.Id)
                : 0;

            var activities = await _db.UserActivities
                .Where(a => a.OccurredAt >= since)
                .GroupBy(a => a.ActionType)
                .Select(g => new { g.Key, Count = g.Count() })
                .ToListAsync();

            var insights = new List<InsightDto>();

            if (totalContacts == 0)
            {
                insights.Add(new InsightDto
                {
                    Category = "Leads",
                    Message = "Nenhum lead cadastrado ainda."
                });
            }
            else
            {
                var conversionRate = (double)converted / totalContacts * 100;

                insights.Add(new InsightDto
                {
                    Category = "Conversão",
                    Message = $"Taxa de conversão atual: {conversionRate:F1}% ({converted} de {totalContacts} leads).",
                    Value = conversionRate
                });

                var topActivity = activities.OrderByDescending(a => a.Count).FirstOrDefault();
                if (topActivity != null)
                {
                    insights.Add(new InsightDto
                    {
                        Category = "Atividades",
                        Message = $"A atividade mais realizada nos últimos {days} dias foi: {topActivity.Key} ({topActivity.Count} vezes).",
                        Value = topActivity.Count
                    });
                }

                if (conversionRate < 10)
                {
                    insights.Add(new InsightDto
                    {
                        Category = "Alerta",
                        Message = "Atenção: a taxa de conversão está baixa. Reveja as etapas do funil."
                    });
                }
                else if (conversionRate > 50)
                {
                    insights.Add(new InsightDto
                    {
                        Category = "Parabéns",
                        Message = "Excelente! Mais da metade dos leads estão chegando à etapa final do funil."
                    });
                }
            }

            return Ok(insights);
        }
    }
}
