using Lead2Buy.API.Data;
using Lead2Buy.API.Dtos.Dashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lead2Buy.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // O Dashboard também será protegido
    public class DashboardController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/dashboard/statistics
        [HttpGet("statistics")]
        public async Task<IActionResult> GetStatistics()
        {
            // 1. Contagem total de contatos (Leads, Oportunidades, etc.)
            var totalLeads = await _context.Contacts.CountAsync();

            // 2. Contagem de Oportunidades
            // Baseado nos requisitos (RF009), um lead em negociação ou com proposta é uma oportunidade.
            // Vamos assumir que o status deles seria "Oportunidade".
            var opportunities = await _context.Contacts
                .CountAsync(c => c.Status == "Oportunidade");

            // 3. Contagem de Convertidos
            // Baseado nos requisitos (RF010), são vendas fechadas.
            // Vamos assumir o status "Convertido".
            var convertedLeads = await _context.Contacts
                .CountAsync(c => c.Status == "Convertido");

            // 4. Cálculo da Taxa de Conversão
            // (Convertidos / Total de Leads) * 100
            double conversionRate = 0;
            if (totalLeads > 0)
            {
                conversionRate = (double)convertedLeads / totalLeads * 100;
            }

            // 5. Monta o DTO de resposta
            var stats = new DashboardStatsDto
            {
                TotalLeads = totalLeads,
                Opportunities = opportunities,
                ConvertedLeads = convertedLeads,
                ConversionRate = Math.Round(conversionRate, 2) // Arredonda para 2 casas decimais
            };

            return Ok(stats);
        }

        // --- INÍCIO DO CÓDIGO DO GRÁFICO ---

        // GET: api/dashboard/leads-over-time
        [HttpGet("leads-over-time")]
        public async Task<IActionResult> GetLeadsOverTime()
        {
            var thirtyDaysAgo = DateTime.UtcNow.AddDays(-30);

            // 1. A consulta agora busca os grupos de dados brutos do banco
            var leadsData = await _context.Contacts
                .Where(c => c.CreatedAt >= thirtyDaysAgo)
                .GroupBy(c => c.CreatedAt.Date)
                .Select(group => new
                {
                    Date = group.Key, // Mantém a data como um objeto DateTime
                    Count = group.Count()
                })
                .ToListAsync(); // Executa a consulta no banco AQUI

            // 2. A formatação da data agora acontece na memória da API, depois de buscar os dados
            var formattedData = leadsData
                .OrderBy(d => d.Date)
                .Select(d => new LeadsOverTimeDto
                {
                    Date = d.Date.ToString("yyyy-MM-dd"), // A formatação acontece AQUI
                    Count = d.Count
                })
                .ToList();

            return Ok(formattedData);
        }

        // --- FIM DO CÓDIGO DO GRÁFICO ---
        
        // --- INÍCIO DO CÓDIGO DO GRÁFICO DE PIZZA ---

            // GET: api/dashboard/performance-by-source
            [HttpGet("performance-by-source")]
            public async Task<IActionResult> GetPerformanceBySource()
            {
                var performanceData = await _context.Contacts
                    .Where(c => !string.IsNullOrEmpty(c.Source)) // 1. Ignora contatos sem origem definida
                    .GroupBy(c => c.Source) // 2. Agrupa pela coluna "Source"
                    .Select(group => new PerformanceBySourceDto
                    {
                        Source = group.Key, // 3. Pega o nome da origem
                        Count = group.Count() // 4. Conta quantos contatos tem nessa origem
                    })
                    .OrderByDescending(dto => dto.Count) // 5. Ordena para mostrar os mais relevantes primeiro
                    .ToListAsync();

                return Ok(performanceData);
            }

        // --- FIM DO CÓDIGO DO GRÁFICO DE PIZZA ---
    }
}