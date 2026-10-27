using CP5.Domain.Errors;
using CP5.Domain.Interfaces;
using RestSharp;

namespace CP5.Infrastructure.Service;

public class EnderecoService : IEnderecoService
{
    // RestSharp e timeout: mesma biblioteca utilizada pelo professor na aula.
    private readonly RestClient _client = new(new RestClientOptions("https://viacep.com.br/ws/")
    {
        Timeout = TimeSpan.FromSeconds(10)
    });

    public async Task<EnderecoModel> ObterPorCepAsync(string cep)
    {
        if (cep.Length != 8 || !cep.All(char.IsDigit))
            throw new ArgumentException("CEP deve conter oito dígitos.");

        RestResponse<EnderecoModel> response;
        try
        {
            response = await _client.ExecuteAsync<EnderecoModel>(new RestRequest($"{cep}/json/", Method.Get));
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            throw new ServicoExternoIndisponivelException("O serviço ViaCEP está indisponível.");
        }

        // HTTP 200 com { erro: true } significa que o CEP não existe.
        if (response.IsSuccessful && response.Data?.Erro == true)
            throw new NoContentException("CEP não encontrado.");

        // Erros de rede, timeout, respostas 5xx e payload ausente não são CEP inexistente.
        if (!response.IsSuccessful || response.Data is null)
            throw new ServicoExternoIndisponivelException("O serviço ViaCEP está indisponível.");

        return response.Data;
    }
}
