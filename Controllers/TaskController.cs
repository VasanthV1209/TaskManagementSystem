using Microsoft.AspNetCore.Mvc;
using TaskManagementSystem.Models;
using System.Collections.Generic;
using System.Linq;
using Task = TaskManagementSystem.Models.Task;
using TaskStatus = TaskManagementSystem.Models.TaskStatus;

namespace TaskManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : Controller
    {
        private static List<Task> Tasks = new List<Task>
        {
            new Task { Id = 1, Title = "Testing Task", Description = "For Testing", Status = TaskStatus.Pending, DueDate = DateTime.Now.AddDays(1) }
        };


        [HttpGet]
        public IActionResult GetTasks([FromQuery] TaskStatus? status, [FromQuery] DateTime? dueDate, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var query = Tasks.AsQueryable();

            if (status.HasValue)
            {
                query = query.Where(t => t.Status == status.Value);
            }

            if (dueDate.HasValue)
            {
                query = query.Where(t => t.DueDate <= dueDate.Value);
            }

            var paginatedTasks = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            return Ok(paginatedTasks);
        }

        [HttpGet("{id}")]
        public IActionResult GetTask(int id)
        {
            var task = Tasks.FirstOrDefault(t => t.Id == id);
            if (task == null)
            {
                return NotFound(new { Error = "Task Not Found", Message = $"No task found with ID {id}" });
            }
            return Ok(task);
        }

        [HttpPost]
        public IActionResult CreateTask([FromBody] Task task)
        {
            if (task == null || string.IsNullOrEmpty(task.Title) || task.DueDate == default)
            {
                return BadRequest(new { Error = "Invalid Task Data", Message = "Title and DueDate are required." });
            }

            task.Id = Tasks.Any() ? Tasks.Max(t => t.Id) + 1 : 1;
            task.Status = task.Status == 0 ? TaskStatus.Pending : task.Status;
            Tasks.Add(task);

            return CreatedAtAction(nameof(GetTask), new { id = task.Id }, task);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateTask(int id, [FromBody] Task task)
        {
            if (task == null || string.IsNullOrEmpty(task.Title) || task.DueDate == default)
            {
                return BadRequest(new { Error = "Invalid Task Data", Message = "Title and DueDate are required." });
            }

            var existingTask = Tasks.FirstOrDefault(t => t.Id == id);
            if (existingTask == null)
            {
                return NotFound(new { Error = "Task Not Found", Message = $"No task found with ID {id}" });
            }

            existingTask.Title = task.Title;
            existingTask.Description = task.Description;
            existingTask.Status = task.Status;
            existingTask.DueDate = task.DueDate;

            return Ok(existingTask);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteTask(int id)
        {
            var task = Tasks.FirstOrDefault(t => t.Id == id);
            if (task == null)
            {
                return NotFound(new { Error = "Task Not Found", Message = $"No task found with ID {id}" });
            }

            Tasks.Remove(task);
            return NoContent();
        }
    }
}
