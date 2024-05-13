namespace CommunicationsApp.Application.DTOs.ViewModels;

public record CursorPaginatedList<TKey, TModel>(TKey NextCursor, IList<TModel> Values);
