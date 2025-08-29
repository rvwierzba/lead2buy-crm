using System.Text.Json;
using Lead2Buy.API.Dtos.Address;
using Microsoft.AspNetCore.Mvc;

namespace Lead2Buy.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AddressController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public AddressController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // GET: api/address/cep/01001000
        [HttpGet("cep/{cep}")]
        public async Task<IActionResult> GetAddressByCep(string cep)
        {
            // 1. Cria um cliente HTTP a partir do Factory
            var httpClient = _httpClientFactory.CreateClient();

            // 2. Monta a URL da requisição para a API do ViaCEP
            var viaCepUrl = $"https://viacep.com.br/ws/{cep}/json/";

            try
            {
                // 3. Faz a chamada GET para a API externa
                var response = await httpClient.GetAsync(viaCepUrl);

                // 4. Verifica se a chamada foi bem-sucedida
                if (response.IsSuccessStatusCode)
                {
                    // 5. Lê o conteúdo da resposta como um stream
                    var contentStream = await response.Content.ReadAsStreamAsync();

                    // 6. Desserializa o JSON para o nosso DTO
                    var viaCepResponse = await JsonSerializer.DeserializeAsync<ViaCepResponseDto>(contentStream);

                    // 7. O ViaCEP retorna sucesso mas com uma flag 'erro' para CEPs inexistentes
                    if (viaCepResponse != null && viaCepResponse.Erro)
                    {
                        return NotFound("CEP não encontrado.");
                    }

                    return Ok(viaCepResponse);
                }
                else
                {
                    // Retorna um erro caso a API do ViaCEP esteja fora ou retorne erro
                    return BadRequest("Não foi possível consultar o CEP.");
                }
            }
            catch (Exception ex)
            {
                // Retorna um erro 500 caso haja uma exceção na chamada
                return StatusCode(500, $"Erro interno ao consultar o CEP: {ex.Message}");
            }
        }
    }
}