using dershaneOtomasyonu.Database.Tables;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace dershaneOtomasyonu.Repositories.TableRepositories.YoklamaRepositories
{
    public interface IYoklamaRepository : IBaseRepository<Yoklama>
    {
        Task<Yoklama> GetByKullaniciIdAndDersKayitIdAsync(int dersKayitId, int kullaniciId);

        Task<List<Yoklama>> GetByDersKayitIdAsync(int dersKayitId);
    }
}
