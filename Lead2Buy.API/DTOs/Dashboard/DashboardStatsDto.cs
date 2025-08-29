namespace Lead2Buy.API.Dtos.Dashboard
{
    public class DashboardStatsDto
    {
        public int TotalLeads { get; set; }
        public int Opportunities { get; set; }
        public int ConvertedLeads { get; set; }
        public double ConversionRate { get; set; } // Usamos double para a porcentagem
    }
}