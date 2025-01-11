using dershaneOtomasyonu.Database;
using dershaneOtomasyonu.Database.Tables;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dershaneOtomasyonu.Repositories.TableRepositories.YoklamaRepositories
{
    public class YoklamaRepository : BaseRepository<Yoklama>, IYoklamaRepository
    {
        public YoklamaRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Yoklama> GetByKullaniciIdAndDersKayitIdAsync(int dersKayitId, int kullaniciId)
        {
            return await _context.Yoklamalar
                .FirstOrDefaultAsync(y => y.DersKayitId == dersKayitId && y.KullaniciId == kullaniciId);
                
        }
    }
}
