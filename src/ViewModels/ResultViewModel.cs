namespace AccessTrackAPI.ViewModels;

// Classe genérica para padronizar a resposta da API
public class ResultViewModel<T>
{
    // Construtor que recebe tanto os dados quanto uma lista de erros
    public ResultViewModel(T data, List<string> errors)
    {
        Data = data;
        Errors = errors;
    }

    // Construtor que recebe apenas os dados (sem erros)
    public ResultViewModel(T data)
    {
        Data = data;
    }
    
    // Construtor que recebe apenas uma lista de erros (sem dados)
    public ResultViewModel(List<string> errors)
    {
        Errors = errors;
    }
    
    // Construtor que recebe um único erro e adiciona à lista de erros
    public ResultViewModel(string error)
    {
        Errors.Add(error);
    }
    
    // Propriedade para armazenar os dados da resposta
    public T Data { get; private set; }
    
    // Propriedade para armazenar os erros da resposta (inicializa como uma lista vazia)
    public List<string> Errors { get; private set; } = new(); // O C# consegue inferir que é uma List<string>
}