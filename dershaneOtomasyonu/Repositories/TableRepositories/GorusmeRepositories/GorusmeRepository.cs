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

        public async Task<List<Gorusme>> GetGorusmeByKatilimciIdAsync(int ogrenciId)
        {
            var gorusme = await _context.Gorusmeler
                .Where(d => d.KatilimciId == ogrenciId)
                .ToListAsync();
            return gorusme;
        }

        public async Task<List<Gorusme>> GetGorusmeByOlusturucuIdAsync(int olusturucuId)
        {
            var gorusme = await _context.Gorusmeler
                .Include(d => d.Katilimci)
                .Where(d => d.OlusturucuId == olusturucuId)
                .ToListAsync();
            return gorusme;
        }
        public async Task<List<Gorusme>> GetAktifGorusmelerByOlusturucuIdAsync(int olusturucuId)
        {
            return await _context.Gorusmeler
                .Where(d => d.OlusturucuId == olusturucuId && d.Durum == true)
                .ToListAsync();
        }
        public async Task<List<Gorusme>> GetAktifGorusmelerByKatilimciIdAsync(int katilimciId)
        {
            return await _context.Gorusmeler
                .Include(o => o.Olusturucu)
                .Where(d => d.KatilimciId == katilimciId && d.Durum == true)
                .ToListAsync();
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
