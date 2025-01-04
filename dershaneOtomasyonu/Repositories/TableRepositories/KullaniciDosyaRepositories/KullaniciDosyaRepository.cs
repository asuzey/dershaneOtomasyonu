using dershaneOtomasyonu.Database;
using dershaneOtomasyonu.Database.Tables;
using dershaneOtomasyonu.Repositories.TableRepositories.KullaniciDersRepositories;
using Microsoft.EntityFrameworkCore;

namespace dershaneOtomasyonu.Repositories.TableRepositories.KullaniciDosyaRepositories
{
    public class KullaniciDosyaRepository : BaseRepository<KullaniciDosya>, IKullaniciDosyaRepository
    {
        public KullaniciDosyaRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<List<KullaniciDosya>> GetAllByKullaniciIdAsync(int kullaniciId)
        {
            return await _context.KullaniciDosyalari
                .Where(k => k.KullaniciId == kullaniciId)
                .Include(x => x.Dosya)
                .ToListAsync();
        }
    }
}
