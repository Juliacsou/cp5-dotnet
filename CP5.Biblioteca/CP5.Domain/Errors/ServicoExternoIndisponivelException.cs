namespace CP5.Domain.Errors;

// Erro específico para distinguir indisponibilidade do ViaCEP de CEP inexistente.
public class ServicoExternoIndisponivelException : Exception
{
    public ServicoExternoIndisponivelException(string message) : base(message) { }
}
