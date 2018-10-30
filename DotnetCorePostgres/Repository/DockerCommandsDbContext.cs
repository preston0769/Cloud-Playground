using Microsoft.EntityFrameworkCore;
using DotnetCorePostgres.Models;

namespace DotnetCorePostgres.Repository
{
    public class DockerCommandsDbContext : DbContext
    {
        public DbSet<DockerCommand> DockerCommands { get; set; }

        public DockerCommandsDbContext(DbContextOptions<DockerCommandsDbContext> options) : base(options) { }
    }
}