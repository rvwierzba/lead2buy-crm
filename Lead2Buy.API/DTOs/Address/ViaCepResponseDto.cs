using System.Text.Json.Serialization;

namespace Lead2Buy.API.Dtos.Address
{
    public class ViaCepResponseDto
    {
        [JsonPropertyName("cep")]
        public string? Cep { get; set; }

        [JsonPropertyName("logradouro")]
        public string? Logradouro { get; set; }

        [JsonPropertyName("bairro")]
        public string? Bairro { get; set; }

        [JsonPropertyName("localidade")]
        public string? Localidade { get; set; }

        [JsonPropertyName("uf")]
        public string? Uf { get; set; }

        // A API do ViaCEP retorna 'true' nesta propriedade se o CEP for inválido
        [JsonPropertyName("erro")]
        public bool Erro { get; set; }
    }
}