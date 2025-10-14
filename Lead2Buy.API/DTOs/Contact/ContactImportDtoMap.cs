using CsvHelper.Configuration;
using Lead2Buy.API.Dtos.Contact;

public class ContactImportDtoMap : ClassMap<ContactImportDto>
{
    public ContactImportDtoMap()
    {
        // Mapeia todas as colunas e as marca como opcionais
        Map(m => m.Name).Optional();
        Map(m => m.PhoneNumber).Optional();
        Map(m => m.Email).Optional();
        Map(m => m.Source).Optional();
        Map(m => m.Gender).Optional();
        Map(m => m.DateOfBirth).Optional();
        Map(m => m.Cep).Optional();
        Map(m => m.Street).Optional();
        Map(m => m.Number).Optional();
        Map(m => m.Neighborhood).Optional();
        Map(m => m.City).Optional();
        Map(m => m.State).Optional();
        Map(m => m.Observations).Optional();
        Map(m => m.Status).Name("Status").Name("FunnelStage");
    }
}