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

        public async Task<List<Gorusme>> GetGorusmeByOlusturucuIdAsync(int olusturucuId)
        {
            var gorusme = await _context.Gorusmeler
                .Where(d => d.OlusturucuId == olusturucuId)
                .ToListAsync();
            return gorusme;
        }

        public async Task<List<Gorusme>> GetGorusmeByOlusturucuIdAndKullaniciIdAsync(int olusturucuId , int kullaniciId)
        {
            var gorusme = await _context.Gorusmeler
                .Where(d => d.OlusturucuId == olusturucuId && d.KatilimciId == kullaniciId)
                .ToListAsync();
            return gorusme;
        }

        public async Task<Gorusme> GetActiveGorusmeByOlusturucuIdAndKullaniciIdAsync(int olusturucuId, int kullaniciId)
        {
            var gorusme = await _context.Gorusmeler
                .FirstOrDefaultAsync(d => d.OlusturucuId == olusturucuId && d.KatilimciId == kullaniciId && d.Durum == true);
            return gorusme;
        }

        public Task AddAsync(List<Gorusme> gorusme)
        {
            throw new NotImplementedException();
        }
    }
}
