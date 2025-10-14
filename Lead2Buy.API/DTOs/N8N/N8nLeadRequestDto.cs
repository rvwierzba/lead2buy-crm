
namespace Lead2Buy.API.DTOs.N8N
{
    public class N8nLeadRequestDto
    {
        public string Name { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Source { get; set; } = "evolution"; // origem padrão
        public string? Interest { get; set; }
    }
}