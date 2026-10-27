using CP5.Domain.Entities;
using CP5.Domain.Errors;
using CP5.Domain.Interfaces;
using RestSharp;
namespace CP5.Infrastructure.Service;
public class EnderecoService:IEnderecoService
{
 public async Task<EnderecoModel> ObterPorCepAsync(string cep)
 {
  if(cep.Length!=8 || !cep.All(char.IsDigit))throw new ArgumentException("CEP deve conter oito dígitos.");
  var client=new RestClient("https://viacep.com.br/ws/");
  var response=await client.ExecuteAsync<EnderecoModel>(new RestRequest($"{cep}/json/",Method.Get));
  if(!response.IsSuccessful || response.Data==null || response.Data.Erro)throw new NoContentException("CEP não encontrado ou serviço indisponível.");
  return response.Data;
 }
}
