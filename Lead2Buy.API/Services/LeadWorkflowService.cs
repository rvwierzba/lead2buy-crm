using Lead2Buy.API.Data;
using Lead2Buy.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Lead2Buy.API.Services
{
 
    public class LeadWorkflowService : ILeadWorkflowService
    {
        private readonly AppDbContext _db;

        public LeadWorkflowService(AppDbContext db)
        {
            _db = db;
        }

        public async Task MoveLeadToStageAsync(Guid contactId, Guid newStageId, Guid performedByUserId)
        {
            var contact = await _db.Contacts.FindAsync(contactId);
            if (contact == null)
                throw new InvalidOperationException("Contato não encontrado");

            // Atualiza etapa
            contact.FunnelStageId = newStageId;

            // Busca responsável configurado para a etapa
            var responsible = await _db.StageResponsibilities
                .Where(x => x.FunnelStageId == newStageId && x.Active)
                .OrderByDescending(x => x.AssignedAt)
                .Select(x => x.User)
                .FirstOrDefaultAsync();

            contact.ResponsibleUserId = responsible?.Id;

            await _db.SaveChangesAsync();
        }
    }
}
