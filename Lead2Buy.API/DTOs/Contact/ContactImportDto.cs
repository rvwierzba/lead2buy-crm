namespace Lead2Buy.API.Dtos.Contact
{
    // DTO flexível que espelha o modelo Contact, mas com todos os campos como opcionais
    public class ContactImportDto
    {
        // Campos principais que ainda esperamos, mas não quebram se ausentes
        public string? Name { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? Source { get; set; }

        // Todos os outros campos possíveis no CSV, como opcionais
        public string? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Cep { get; set; }
        public string? Street { get; set; }
        public string? Number { get; set; }
        public string? Neighborhood { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Observations { get; set; }
        public string? Status { get; set; }
    }
}