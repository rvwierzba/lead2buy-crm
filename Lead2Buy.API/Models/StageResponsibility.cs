using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lead2Buy.API.Models
{
    public class StageResponsibility
    {
         public Guid Id { get; set; } = new Guid();

        public Guid FunnelStageId { get; set; }
        public FunnelStage? FunnelStage { get; set; }

        public Guid UserId { get; set; }
        public User? User { get; set; }

        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
        public bool Active { get; set; } = true; // permite trocar sem perder histórico
    }
}