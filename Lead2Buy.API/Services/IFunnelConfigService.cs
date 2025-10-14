using Lead2Buy.API.Models;

namespace Lead2Buy.API.Services
{
    public interface IFunnelConfigService
    {
         Task<List<FunnelStage>> GetStagesAsync();
        Task<FunnelStage> CreateStageAsync(string name, Guid order, bool isFinal = false);
        Task AssignResponsibleAsync(Guid stageId, Guid userId);
        Task<User?> GetResponsibleForStageAsync(Guid stageId);
    }
}