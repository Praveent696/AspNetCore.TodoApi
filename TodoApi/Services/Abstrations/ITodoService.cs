using TodoApi.Models;
using TodoApi.Models.Dto;

namespace TodoApi.Services.Abstrations
{
    public interface ITodoService
    {
        Task<TodoDto> AddTodoAsync(TodoDto todoDto, string userId);
        Task<TodoDto> GetTodoByIdAsync(int id, string userId, bool isAdmin);
        Task<List<TodoDto>> GetAllTodosAsync();
        Task<List<TodoDto>> GetUserTodosAsync(string userId);
        Task<TodoDto> UpdateTodoAsync(int id, TodoDto todoDto, string userId, bool isAdmin);
        Task<bool> DeleteTodoAsync(int id, string userId, bool isAdmin);
    }
}
