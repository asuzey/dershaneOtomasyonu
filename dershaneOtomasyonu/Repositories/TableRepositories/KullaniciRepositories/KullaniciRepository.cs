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

namespace dershaneOtomasyonu.Repositories.TableRepositories.KullaniciRepositories
{
    public class KullaniciRepository : BaseRepository<Kullanici>, IKullaniciRepository
    {
        public KullaniciRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Kullanici> GetByUserNameAsync(string userName)
        {
            return await _context.Kullanicilar
                .FirstOrDefaultAsync(k => k.KullaniciAdi == userName);
        }

        public async Task<IEnumerable<Kullanici>> GetByRoleIdAsync(int roleId)
        {
            return await _context.Kullanicilar
                .Where(k => k.RoleId == roleId)
                .ToListAsync();
        }

        public async Task<Kullanici> GetByUserNameAndPasswordAsync(string userName, string password)
        {
            return await _context.Kullanicilar
                .FirstOrDefaultAsync(k => k.KullaniciAdi == userName && k.Sifre == password);
        }

        public async Task<Kullanici> GetByEmailAsync(string email)
        {
            return await _context.Kullanicilar
                .FirstOrDefaultAsync(k => k.Email == email);
        }

        public async Task<List<KullaniciDto>> GetAllAsDtoAsync()
        {
            var kullanicilar = await _context.Kullanicilar.ToListAsync();
            return kullanicilar.Adapt<List<KullaniciDto>>();
        }

        public async Task<List<Kullanici>> GetAllTeachersAsync()
        {
            var role = await _context.Roller.FirstOrDefaultAsync(x => x.RolAdi == "Öğretmen");
            List<Kullanici> kullanicilar;
            if (role != null)
            {
                kullanicilar = await _context.Kullanicilar.Where(x => x.RoleId == role.Id).ToListAsync();
            }
            else
            {
                kullanicilar = new List<Kullanici>();
            }
            return kullanicilar;
        }

        public async Task<List<Kullanici>> GetAllStudentsAsync()
        {
            var role = await _context.Roller.FirstOrDefaultAsync(x => x.RolAdi == "Öğrenci");
            List<Kullanici> kullanicilar;
            if (role != null)
            {
                kullanicilar = await _context.Kullanicilar.Where(x => x.RoleId == role.Id).ToListAsync();
            }
            else
            {
                kullanicilar = new List<Kullanici>();
            }
            return kullanicilar;
        }

        public async Task<List<RaporYoklamaDto>> GetAllYoklamaRaporByOgrenciIdAsync(int ogrenciId)
        {
            // Öğrenci bilgisi ve sınıf ID'si
            var ogrenci = await _context.Kullanicilar
                .Where(k => k.Id == ogrenciId)
                .Select(k => new { k.Adi, k.Soyadi, k.SinifId })
                .FirstOrDefaultAsync()
                ?? throw new Exception("Öğrenci bulunamadı.");

            // Öğrencinin sınıfındaki son 14 ders kaydı (tarihe göre sıralı)
            var sonDersKayitlari = await _context.DersKayitlari
                .Include(d => d.Ders) // Ders navigation property’sini dahil et
                .Where(d => d.SinifId == ogrenci.SinifId)
                .OrderByDescending(d => d.BaslangicTarihi)
                .Take(14)
                .Select(d => new { d.Id, d.BaslangicTarihi, d.Ders.Adi }) // Ders adını da seç
                .ToListAsync();

            // Öğrencinin yoklama verileri
            var yoklamaVerileri = await _context.Yoklamalar
                .Where(y => y.KullaniciId == ogrenciId && sonDersKayitlari.Select(d => d.Id).Contains(y.DersKayitId))
                .Select(y => new { y.DersKayitId, y.KatilmaTarihi.Date })
                .ToListAsync();

            // Son 14 ders kaydı üzerinden rapor oluştur
            return sonDersKayitlari
                .OrderBy(d => d.BaslangicTarihi) // Eski tarihler önce gelsin
                .Select((ders, index) => new RaporYoklamaDto
                {
                    Tarih = ders.BaslangicTarihi,
                    AdiSoyadi = $"{ogrenci.Adi} {ogrenci.Soyadi}",
                    DersAdi = ders.Adi, // Ders adını doldur
                    XValue = index, // Dersin sırası (0-13)
                    YValue = ders.Id, // Ders ID
                    Katildi = yoklamaVerileri.Any(y => y.DersKayitId == ders.Id) // Katılım durumu
                })
                .ToList();
        }

    }

}
