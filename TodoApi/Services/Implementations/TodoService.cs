using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TodoApi.Models.Dto;
using TodoApi.Models;
using TodoApi.Services.Abstrations;
using TodoApi.Data;

public class TodoService : ITodoService
{
    private readonly AppDbContext _db;

    public TodoService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<TodoDto> AddTodoAsync(TodoDto todoDto, string userId)
    {
        var todo = new Todo
        {
            Title = todoDto.Title,
            Description = todoDto.Description,
            Status = TodoStatus.Pending,
            UserId = userId
        };

        _db.Todos.Add(todo);
        await _db.SaveChangesAsync();

        return MapToDto(todo);
    }

    public async Task<TodoDto> GetTodoByIdAsync(int id, string userId, bool isAdmin)
    {
        var todo = await _db.Todos
            .FirstOrDefaultAsync(t => t.Id == id && (isAdmin || t.UserId == userId));

        return todo == null ? null : MapToDto(todo);
    }

    public async Task<List<TodoDto>> GetAllTodosAsync()
    {
        var todos = await _db.Todos.ToListAsync();
        return todos.Select(MapToDto).ToList();
    }

    public async Task<List<TodoDto>> GetUserTodosAsync(string userId)
    {
        var todos = await _db.Todos.Where(t => t.UserId == userId).ToListAsync();
        return todos.Select(MapToDto).ToList();
    }

    public async Task<TodoDto> UpdateTodoAsync(int id, TodoDto todoDto, string userId, bool isAdmin)
    {
        if (!IsValidStatus(todoDto.Status))
            throw new ArgumentException("Invalid todo status value. Expected values (Pending, Completed, InProgress, Overdue, Cancelled)");

        var todo = await _db.Todos
            .FirstOrDefaultAsync(t => t.Id == id && (isAdmin || t.UserId == userId));

        if (todo == null)
            return null;

        todo.Title = todoDto.Title;
        todo.Description = todoDto.Description;
        todo.Status = Enum.Parse<TodoStatus>(todoDto.Status, true);

        _db.Todos.Update(todo);
        await _db.SaveChangesAsync();

        return MapToDto(todo);
    }

    public async Task<bool> DeleteTodoAsync(int id, string userId, bool isAdmin)
    {
        var todo = await _db.Todos
            .FirstOrDefaultAsync(t => t.Id == id && (isAdmin || t.UserId == userId));

        if (todo == null)
            return false;

        _db.Todos.Remove(todo);
        await _db.SaveChangesAsync();
        return true;
    }

    private TodoDto MapToDto(Todo todo)
    {
        return new TodoDto
        {
            Id = todo.Id,
            Title = todo.Title,
            Description = todo.Description,
            Status = todo.Status.ToString()
        };
    }

    private bool IsValidStatus(string status)
    {
        return Enum.TryParse(typeof(TodoStatus), status, true, out _);
    }

    private TodoStatus ParseStatus(string status)
    {
        return (TodoStatus)Enum.Parse(typeof(TodoStatus), status, true);
    }
}
