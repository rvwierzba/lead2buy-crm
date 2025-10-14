using System.ComponentModel.DataAnnotations;

namespace Lead2Buy.API.Models
{
    public class FunnelStage
{
    public Guid Id { get; set; } = new Guid();

    [Required, MaxLength(50)]
    public string Name { get; set; } = string.Empty; // Comercial, Agendamento, etc.

    public Guid Order { get; set; } // posição no funil

    public bool IsFinal { get; set; } = false; // Convertido/Remarketing podem ser finais
}

}