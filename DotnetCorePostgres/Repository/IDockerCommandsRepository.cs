using System.Collections.Generic;
using System.Threading.Tasks;

using DotnetCorePostgres.Models;

namespace DotnetCorePostgres.Repository
{
    public interface IDockerCommandsRepository
    {     
        Task<List<DockerCommand>> GetDockerCommandsAsync();
        
        Task InsertDockerCommandAsync(DockerCommand command);
    }
}