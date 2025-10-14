using Lead2Buy.API.Data;
using Lead2Buy.API.Dtos.Dashboard;
using Lead2Buy.API.DTOs.Dashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Lead2Buy.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("statistics")]
        public async Task<IActionResult> GetStatistics()
        {
            var totalLeads = await _context.Contacts.CountAsync();

            // Buscar etapas relevantes
            var propostaStage = await _context.FunnelStages.FirstOrDefaultAsync(s => s.Name == "Proposta Apresentada");
            var negociacaoStage = await _context.FunnelStages.FirstOrDefaultAsync(s => s.Name == "Negociação");
            var convertidoStage = await _context.FunnelStages.FirstOrDefaultAsync(s => s.Name == "Convertido");

            var opportunities = 0;
            if (propostaStage != null || negociacaoStage != null)
            {
                opportunities = await _context.Contacts
                    .Where(c =>
                        (propostaStage != null && c.FunnelStageId == propostaStage.Id) ||
                        (negociacaoStage != null && c.FunnelStageId == negociacaoStage.Id))
                    .CountAsync();
            }

            var convertedLeads = 0;
            if (convertidoStage != null)
            {
                convertedLeads = await _context.Contacts
                    .Where(c => c.FunnelStageId == convertidoStage.Id)
                    .CountAsync();
            }

            var newContactsThisMonth = await _context.Contacts
                .Where(c => c.CreatedAt.Month == DateTime.UtcNow.Month && c.CreatedAt.Year == DateTime.UtcNow.Year)
                .CountAsync();

            var pendingTasks = await _context.CrmTasks
                .Where(t => !t.IsCompleted && t.DueDate.Date <= DateTime.UtcNow.Date)
                .CountAsync();

            double conversionRate = opportunities > 0 ? (double)convertedLeads / opportunities * 100 : 0;

            var stats = new DashboardStatsDto
            {
                TotalLeads = totalLeads,
                Opportunities = opportunities,
                ConvertedLeads = convertedLeads,
                ConversionRate = Math.Round(conversionRate, 2),
                PendingTasks = pendingTasks,
                NewContactsThisMonth = newContactsThisMonth
            };

            return Ok(stats);
        }

        [HttpGet("leads-over-time")]
        public async Task<IActionResult> GetLeadsOverTime()
        {
            var sixMonthsAgo = DateTime.UtcNow.AddMonths(-6);
            var leads = await _context.Contacts
                .Where(c => c.CreatedAt >= sixMonthsAgo)
                .GroupBy(c => new { c.CreatedAt.Year, c.CreatedAt.Month })
                .Select(g => new { Month = g.Key.Month, Year = g.Key.Year, Count = g.Count() })
                .OrderBy(g => g.Year).ThenBy(g => g.Month)
                .ToListAsync();

            var result = leads.Select(l => new LeadsOverTimeDto
            {
                Date = new DateTime(l.Year, l.Month, 1).ToString("MMM/yy", CultureInfo.InvariantCulture),
                Count = l.Count
            }).ToList();

            return Ok(result);
        }

        [HttpGet("performance-by-source")]
        public async Task<IActionResult> GetPerformanceBySource()
        {
            var performance = await _context.Contacts
                .Where(c => !string.IsNullOrEmpty(c.Source))
                .GroupBy(c => c.Source)
                .Select(g => new PerformanceBySourceDto { Source = g.Key ?? "Desconhecida", Count = g.Count() })
                .OrderByDescending(p => p.Count)
                .ToListAsync();

            return Ok(performance);
        }

        [HttpGet("recent-contacts")]
        public async Task<IActionResult> GetRecentContacts()
        {
            var recentContacts = await _context.Contacts
                .OrderByDescending(c => c.CreatedAt)
                .Take(5)
                .Select(c => new RecentContactDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Email = c.Email ?? "",
                    CreatedAt = c.CreatedAt
                })
                .ToListAsync();

            return Ok(recentContacts);
        }

        [HttpGet("upcoming-tasks")]
        public async Task<IActionResult> GetUpcomingTasks()
        {
            var upcomingTasks = await _context.CrmTasks
                .Include(t => t.Contact)
                .Where(t => !t.IsCompleted)
                .OrderBy(t => t.DueDate)
                .Take(5)
                .Select(t => new UpcomingTaskDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    DueDate = t.DueDate,
                    ContactName = t.Contact.Name
                })
                .ToListAsync();

            return Ok(upcomingTasks);
        }
    }
}
