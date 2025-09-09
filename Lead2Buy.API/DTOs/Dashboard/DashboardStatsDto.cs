namespace Lead2Buy.API.Dtos.Dashboard
{
    public class DashboardStatsDto
    {
       public int TotalLeads { get; set; }
        public int Opportunities { get; set; }
        public int ConvertedLeads { get; set; }
        public double ConversionRate { get; set; }
        public int PendingTasks { get; set; }
                
        public int NewContactsThisMonth { get; set; }
    }
}