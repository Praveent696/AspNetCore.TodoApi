using Microsoft.AspNetCore.Mvc;
using TodoApi.Models.Dto;

namespace TodoApi.Controllers
{
    [ApiController]
    public class ApiBaseController : ControllerBase
    {
        [NonAction]
        protected ApiResponse<T> CreateResponse<T>(bool status, T data, string message)
        {
            return new ApiResponse<T>(status, data, message);
        }

        [NonAction]
        protected IActionResult HandleErrorResponse(Exception ex, string message)
        {
            // Return a generic error response
            return StatusCode(500, CreateResponse<object>(false, null, message));
        }
    }
}
