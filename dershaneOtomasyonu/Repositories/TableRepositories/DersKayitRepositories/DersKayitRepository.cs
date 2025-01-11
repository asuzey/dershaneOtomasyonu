using dershaneOtomasyonu.Database;
using dershaneOtomasyonu.Database.Tables;
using dershaneOtomasyonu.DTO;
using dershaneOtomasyonu.Repositories.TableRepositories.DegerlendirmeRepositories;
using Mapster;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dershaneOtomasyonu.Repositories.TableRepositories.DersKayitRepositories
{
    public class DersKayitRepository : BaseRepository<DersKayit>, IDersKayitRepository
    {

        public DersKayitRepository(AppDbContext context) : base(context)
        {

        }

        public async Task<DersKayit> GetActiveDersBySinifAndOgretmenIdAsync(int sinifId, int ogretmenId)
        {
            return await _context.DersKayitlari.FirstOrDefaultAsync(x => x.SinifId == sinifId && x.KullaniciId == ogretmenId && x.Durum == true);
        }

        public Task<List<DersKayit>> GetActiveDerslerByOgretmenIdAsync(int ogretmenId)
        {
            return _context.DersKayitlari.Where(x => x.KullaniciId == ogretmenId && x.Durum == true).ToListAsync();
        }

        public Task<List<DersKayit>> GetActiveDerslerBySinifIdAsync(int sinifId)
        {
            return _context.DersKayitlari.Include(x => x.Kullanici).Include(x => x.Sinif).Where(x => x.SinifId == sinifId && x.Durum == true).ToListAsync();
        }
    }
}
