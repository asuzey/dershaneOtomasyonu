using dershaneOtomasyonu.Database.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dershaneOtomasyonu.Repositories.TableRepositories.KullaniciDosyaRepositories
{
    public interface IKullaniciDosyaRepository : IBaseRepository<KullaniciDosya>
    {
        Task<List<KullaniciDosya>> GetAllByKullaniciIdAsync(int kullaniciId);
    }
}
