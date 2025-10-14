namespace Lead2Buy.API.DTOs.BI
{
    public class InsightDto
    {
        public string Category { get; set; } = string.Empty; // ex: "Conversão", "Atividades"
        public string Message { get; set; } = string.Empty;  // insight em linguagem natural
        public double? Value { get; set; }    
    }
}