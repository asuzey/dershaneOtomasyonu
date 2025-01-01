using dershaneOtomasyonu.Database.Tables;
using dershaneOtomasyonu.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dershaneOtomasyonu.Repositories.TableRepositories.KullaniciDersRepositories
{
    public interface IKullaniciDersRepository : IBaseRepository<KullaniciDers>
    {
        Task<KullaniciDers> GetByKullaniciIdAndDersIdAsync(int kullaniciId, int dersId);
        Task<List<KullaniciDers>> GetAllByKullaniciIdAsync(int kullaniciId);
        Task DeleteByOgretmenIdAndDersIdAsync(int ogretmenId, int dersId);

    }
}
