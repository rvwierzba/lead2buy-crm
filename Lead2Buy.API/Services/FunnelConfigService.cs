using Lead2Buy.API.Models;
using Lead2Buy.API.Data;
using Microsoft.EntityFrameworkCore;


namespace Lead2Buy.API.Services
{
    public class FunnelConfigService : IFunnelConfigService
    {
        private readonly AppDbContext _db;
    public FunnelConfigService(AppDbContext db) => _db = db;

    public Task<List<FunnelStage>> GetStagesAsync() =>
        _db.FunnelStages.OrderBy(s => s.Order).ToListAsync();

    public async Task<FunnelStage> CreateStageAsync(string name, Guid order, bool isFinal = false)
    {
        var stage = new FunnelStage { Name = name, Order = order, IsFinal = isFinal };
        _db.FunnelStages.Add(stage);
        await _db.SaveChangesAsync();
        return stage;
    }

    public async Task AssignResponsibleAsync(Guid stageId, Guid userId)
    {
        // desativar antigas
        var olds = await _db.StageResponsibilities
            .Where(x => x.FunnelStageId == stageId && x.Active).ToListAsync();
        foreach (var o in olds) o.Active = false;

        _db.StageResponsibilities.Add(new StageResponsibility
        {
            FunnelStageId = stageId,
            UserId = userId,
            Active = true
        });
        await _db.SaveChangesAsync();
    }

    public async Task<User?> GetResponsibleForStageAsync(Guid stageId)
    {
        var resp = await _db.StageResponsibilities
            .Where(x => x.FunnelStageId == stageId && x.Active)
            .OrderByDescending(x => x.AssignedAt)
            .FirstOrDefaultAsync();

        if (resp == null) return null;
        return await _db.Users.FindAsync(resp.UserId);
    }
    }
}