namespace CP5.Application.Dtos;
public record LinkDto(string Rel, string Href, string Method);
public class PageResponseDto<T>
{
 public IEnumerable<T> Items { get; set; } = new List<T>();
 public int Pagina { get; set; }
 public int Tamanho { get; set; }
 public int TotalRegistros { get; set; }
 public int TotalPaginas { get; set; }
 public List<LinkDto> Links { get; set; } = new();
}
public class RecursoDto<T> { public T? Dados { get; set; } public List<LinkDto> Links { get; set; } = new(); }
