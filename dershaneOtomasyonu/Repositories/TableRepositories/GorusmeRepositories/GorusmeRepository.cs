using dershaneOtomasyonu.Database;
using dershaneOtomasyonu.Database.Tables;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dershaneOtomasyonu.Repositories.TableRepositories.GorusmeRepositories
{
    public class GorusmeRepository : BaseRepository<Gorusme>, IGorusmeRepository
    {
        public GorusmeRepository(AppDbContext context) : base(context)
        {

        }

        public async Task<List<Gorusme>> GetGorusmeByKullaniciIdAsync(int kullaniciId)
        {
            var gorusme = await _context.Gorusmeler
                .Where(d => d.KatilimciId == kullaniciId)
                .ToListAsync();
            return gorusme;
        }

        public async Task<List<Gorusme>> GetGorusmeByOlusturucuIdAsync(string oda)
        {
            var gorusme = await _context.Gorusmeler
                .Where(d => d.Oda == oda)
                .ToListAsync();
            return gorusme;
        }

        public async Task<List<Gorusme>> GetGorusmeByOlusturucuIdAsync(string oda)
        {
            var gorusme = await _context.Gorusmeler
                .Where(d => d.Oda == oda && d.)
                .ToListAsync();
            return gorusme;
        }
    }
}
