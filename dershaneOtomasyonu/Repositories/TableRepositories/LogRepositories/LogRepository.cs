using dershaneOtomasyonu.Database;
using dershaneOtomasyonu.Database.Tables;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dershaneOtomasyonu.DTO;
using Mapster;
using Microsoft.VisualBasic.ApplicationServices;

namespace dershaneOtomasyonu.Repositories.TableRepositories.LogRepositories
{
    public class LogRepository : BaseRepository<LogEntry>, ILogRepository
    {
        public LogRepository(AppDbContext context) : base(context)
        {

        }

        public async Task<List<LogEntry>> GetLogsByUserId(int userId)
        {
            return await _context.Logs
                .Where(log => log.KullaniciId == userId)
                .Include(log => log.Kullanici)
                .ToListAsync();
        }


        public async Task<List<LogEntryDto>> GetAllAsDtoAsync()
        {
            return await _context.Logs
                .Select(log => new LogEntryDto
                {
                    KullaniciAdi = log.Kullanici != null ? log.Kullanici.KullaniciAdi : null,
                    Level = log.Level,
                    Message = log.Message,
                    Timestamp = log.Timestamp,
                })
                .ToListAsync();
        }
    }
}
