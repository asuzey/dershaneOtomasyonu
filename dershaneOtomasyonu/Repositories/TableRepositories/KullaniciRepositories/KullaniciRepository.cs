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
    }

}
