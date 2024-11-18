using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TodoApi.Models.Dto;
using TodoApi.Services.Abstrations;

namespace TodoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TodoController : ApiBaseController
    {
        private readonly ITodoService _todoService;
        private readonly IUserService _userService;

        public TodoController(ITodoService todoService, IUserService userService)
        {
            _todoService = todoService;
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var isAdmin = await _userService.IsAdmin(userId);
                var todos = isAdmin ? await _todoService.GetAllTodosAsync() : await _todoService.GetUserTodosAsync(userId);

                return Ok(CreateResponse<List<TodoDto>>(true, todos, "Todo List!"));
            }
            catch (Exception ex)
            {
                return HandleErrorResponse(ex, "An error occurred while fetching todos.");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var isAdmin = await _userService.IsAdmin(userId);
                var todo = await _todoService.GetTodoByIdAsync(id, userId, isAdmin);

                if (todo == null)
                {
                    return NotFound(CreateResponse<TodoDto>(false, null, "Todo not found!"));
                }

                return Ok(CreateResponse<TodoDto>(true, todo, "Todo information!"));
            }
            catch (Exception ex)
            {
                return HandleErrorResponse(ex, "An error occurred while fetching the todo.");
            }
        }

        [HttpPost()]
        public async Task<IActionResult> Create([FromBody] TodoDto todo)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var createdTodo = await _todoService.AddTodoAsync(todo, userId);

                return CreatedAtAction(nameof(GetById), new { id = createdTodo.Id }, createdTodo);
            }
            catch (Exception ex)
            {
                return HandleErrorResponse(ex, "An error occurred while creating the todo.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] TodoDto todo)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var isAdmin = await _userService.IsAdmin(userId);
                todo.Id = id;

                var updatedTodo = await _todoService.UpdateTodoAsync(id, todo, userId, isAdmin);

                if (updatedTodo == null)
                {
                    return NotFound(CreateResponse<TodoDto>(false, null, "Todo not found!"));
                }

                return Ok(CreateResponse<TodoDto>(true, updatedTodo, "Todo updated successfully!"));
            }
            catch (Exception ex)
            {
                return HandleErrorResponse(ex, ex.Message ?? "An error occurred while updating the todo.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var isAdmin = await _userService.IsAdmin(userId);

                var isDeleted = await _todoService.DeleteTodoAsync(id, userId, isAdmin);

                if (!isDeleted)
                {
                    return NotFound(CreateResponse<bool>(false, false, "Todo not found!"));
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return HandleErrorResponse(ex, "An error occurred while deleting the todo.");
            }
        }
    }
}
