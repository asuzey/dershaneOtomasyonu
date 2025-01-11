using dershaneOtomasyonu.Database.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dershaneOtomasyonu.Repositories.TableRepositories.GorusmeRepositories
{
    public interface IGorusmeRepository : IBaseRepository<Gorusme>
    {
        Task<List<Gorusme>> GetGorusmeByKatilimciIdAsync(int kullaniciId);
        Task<List<Gorusme>> GetGorusmeByOlusturucuIdAsync(int olusturucuId);
        Task<List<Gorusme>> GetAktifGorusmelerByOlusturucuIdAsync(int olusturucuId);
        Task<List<Gorusme>> GetAktifGorusmelerByKatilimciIdAsync(int katilimciId);
        Task<List<Gorusme>> GetGorusmeByOlusturucuIdAndKullaniciIdAsync(int olusturucuId, int kullaniciId);
        Task<Gorusme> GetActiveGorusmeByOlusturucuIdAndKullaniciIdAsync(int olusturucuId, int kullaniciId);
        Task AddAsync(List<Gorusme> gorusme);
    }
}
