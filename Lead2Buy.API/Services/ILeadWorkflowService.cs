using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lead2Buy.API.Services
{
    public interface ILeadWorkflowService
    {
        Task MoveLeadToStageAsync(Guid contactId, Guid newStageId, Guid performedByUserId);
    }
}