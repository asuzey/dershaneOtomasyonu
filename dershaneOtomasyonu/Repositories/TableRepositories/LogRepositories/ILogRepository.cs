using dershaneOtomasyonu.Database.Tables;
using dershaneOtomasyonu.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dershaneOtomasyonu.Repositories.TableRepositories.LogRepositories
{
    public interface ILogRepository : IBaseRepository<LogEntry>
    {
        Task<List<LogEntry>> GetLogsByUserId(int userId);
        Task<List<LogEntryDto>> GetAllAsDtoAsync();
    }

}
