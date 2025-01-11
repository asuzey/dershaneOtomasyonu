using dershaneOtomasyonu.Database;
using dershaneOtomasyonu.Database.Tables;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dershaneOtomasyonu.Repositories.TableRepositories.NotRepositories
{
    public class NotRepository : BaseRepository<Not>, INotRepository
    {
        public NotRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<List<Not>> GetNotsByKullaniciIdAsync(int kullaniciId)
        {
            var nots = await _context.Notlar
                .Where(k => k.KullaniciNotlari.Any(kn => kn.KullaniciId == kullaniciId))
                .Include(x => x.KullaniciNotlari)
                .ToListAsync();

            return nots;
        }
    }
}
