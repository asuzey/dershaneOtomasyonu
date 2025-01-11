using dershaneOtomasyonu.Database.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dershaneOtomasyonu.Repositories.TableRepositories.YoklamaRepositories
{
    public interface IYoklamaRepository : IBaseRepository<Yoklama>
    {
        Task<Yoklama> GetByKullaniciIdAndDersKayitIdAsync(int dersKayitId, int kullaniciId);
    }
}
