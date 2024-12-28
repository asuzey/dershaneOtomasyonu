using dershaneOtomasyonu.Database.Tables;
using dershaneOtomasyonu.Database;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }

}
