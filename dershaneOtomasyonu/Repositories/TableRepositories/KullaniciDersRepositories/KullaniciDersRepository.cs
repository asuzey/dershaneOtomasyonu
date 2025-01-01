using dershaneOtomasyonu.Database.Tables;
using dershaneOtomasyonu.Database;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dershaneOtomasyonu.DTO;
using Mapster;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace dershaneOtomasyonu.Repositories.TableRepositories.KullaniciDersRepositories
{
    public class KullaniciDersRepository : BaseRepository<KullaniciDers>, IKullaniciDersRepository
    {
        public KullaniciDersRepository(AppDbContext context) : base(context)
        {
        }

        public async Task DeleteByOgretmenIdAndDersIdAsync(int ogretmenId, int dersId)
        {
            var entity = await _dbSet.FirstOrDefaultAsync(x => x.KullaniciId == ogretmenId && x.DersId == dersId);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<KullaniciDers>> GetAllByKullaniciIdAsync(int kullaniciId)
        {
            return await _context.KullaniciDersleri
                .Where(k => k.KullaniciId == kullaniciId)
                .Include(x => x.Ders)
                .ToListAsync();
        }

        public async Task<KullaniciDers> GetByKullaniciIdAndDersIdAsync(int kullaniciId, int dersId)
        {
            return await _context.KullaniciDersleri
                .FirstOrDefaultAsync(k => k.KullaniciId == kullaniciId && k.DersId == dersId);
        }
    }

}
