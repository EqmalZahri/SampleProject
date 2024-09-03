using Microsoft.EntityFrameworkCore;
using Rbac_IctJohor.Models;
using Rbac_IctJohor.Repositories;

namespace Rbac_IctJohor.Services
{
    public interface IJobTitleService
    {
        Task<JobTitle> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    }
    public class JobTitleService : IJobTitleService
    {
        private readonly AppDbContext _dbContext;

        public JobTitleService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<JobTitle> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            var result = await _dbContext.JobTitles.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            return result;
        }
    }
}
