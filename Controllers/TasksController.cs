using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System.Drawing.Printing;
using TaskManagementAppAngular.Models.Domain;
using TaskManagementAppAngular.Models.DTO;
using TaskManagementAppAngular.Repositories.Interface;

namespace TaskManagementAppAngular.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly ITaskRepository taskRepository;

        public TasksController(ITaskRepository taskRepository)
        {
            this.taskRepository = taskRepository;
        }
       
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] AddTaskRequest addTaskRequest)
        {
            //Map DTO to domain model

            var taskitem = new TaskItem
            { 
                Title = addTaskRequest.Title,
                Description = addTaskRequest.Description,
                DueDate = addTaskRequest.DueDate,
                AssignedTo = addTaskRequest.AssignedTo,
                IsCompleted = addTaskRequest.IsCompleted,
            };


          var addedTask =  await taskRepository.AddAsync(taskitem);


            return Ok(new { 
                Message = "Task added successfully!",
                Task = addedTask
            });


        }

        [HttpGet]
        public async Task<IActionResult> List(
           string? searchQuery,
           string? sortBy,
           string? sortDirection,
           int pagesize = 3,
           int pageNumber = 1)
        {
            var totalRecords = await taskRepository.CountAsync();
            var totalPages = Math.Ceiling((decimal)totalRecords / pagesize);

            if (pageNumber > totalPages)
            {
                pageNumber = (int)totalPages;
            }
            if (pageNumber < 1)
            {
                pageNumber = 1;
            }

            var taskitems = await taskRepository.GetAllAsync(searchQuery, sortBy, sortDirection, pageNumber, pagesize);

            var response = new
            {
                TotalPages = totalPages,
                PageNumber = pageNumber,
                PageSize = pagesize,
                TaskItems = taskitems
            };

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTask(int id)
        {
            var taskitem = await taskRepository.GetAsync(id);
            if (taskitem == null)
            {
                return NotFound();
            }

            return Ok(taskitem);

        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(int id, [FromBody] EditTaskRequest editTaskRequest)
        {


            if (id != editTaskRequest.Id)
            {
                return BadRequest();
            }

            var taskItemDomainModel = new TaskItem
            {
                Id = editTaskRequest.Id,
                Title = editTaskRequest.Title,
                Description = editTaskRequest.Description,
                DueDate = editTaskRequest.DueDate,
                AssignedTo = editTaskRequest.AssignedTo,
                IsCompleted = editTaskRequest.IsCompleted,
            };

            var updatedTask = await taskRepository.UpdateAsync(taskItemDomainModel);

            if (updatedTask == null)
            {
                return NotFound();
            }

            return Ok(new { 
                Message = "Task updated successfully!",
                Task = updatedTask
            });
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deletedTask = await taskRepository.DeleteAsync(id);

            if (deletedTask == null)
            {
                return NotFound();
            }

            return Ok(new { 
                Message = "Task deleted successfully!",
                Task = deletedTask
            });
        }


        [HttpGet("exportToExcel")]
        public async Task<IActionResult> ExportToExcel([FromBody] ExportRequest exportRequest)
        {
            if (exportRequest == null || exportRequest.SelectedIds == null || !exportRequest.SelectedIds.Any())
            {
                return BadRequest(new { Message = "No tasks selected for export." });
            }

            var taskIds = exportRequest.SelectedIds;
            var tasks = await taskRepository.GetAllAsync();

            var taskItemsList = tasks.Where(t => taskIds.Contains(t.Id)).ToList();

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Tasks");

                worksheet.Cells[1, 1].Value = "Id";
                worksheet.Cells[1, 2].Value = "Title";
                worksheet.Cells[1, 3].Value = "Description";
                worksheet.Cells[1, 4].Value = "DueDate";
                worksheet.Cells[1, 5].Value = "AssignedTo";
                worksheet.Cells[1, 6].Value = "IsCompleted";

                for (int i = 0; i < taskItemsList.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = taskItemsList[i].Id;
                    worksheet.Cells[i + 2, 2].Value = taskItemsList[i].Title;
                    worksheet.Cells[i + 2, 3].Value = taskItemsList[i].Description;
                    worksheet.Cells[i + 2, 4].Value = taskItemsList[i].DueDate.HasValue ? taskItemsList[i].DueDate.Value.ToString("yyyy-MM-dd") : "N/A";
                    worksheet.Cells[i + 2, 5].Value = taskItemsList[i].AssignedTo;
                    worksheet.Cells[i + 2, 6].Value = taskItemsList[i].IsCompleted ? "Yes" : "No";
                }

                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;

                var fileName = $"Tasks_{System.DateTime.Now:yyyyMMddHHmmss}.xlsx";
                var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                return File(stream, contentType, fileName);
            }
        }


    }
}
