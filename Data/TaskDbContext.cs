using Microsoft.EntityFrameworkCore;
using TaskManagementAppAngular.Models.Domain;

namespace TaskManagementAppAngular.Data
{
    public class TaskDbContext : DbContext
    {

        public TaskDbContext(DbContextOptions options ) : base(options)
        {

        }

        public DbSet<TaskItem> TaskItems { get; set; }
    }
}
