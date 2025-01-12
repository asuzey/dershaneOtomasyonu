using dershaneOtomasyonu.Database.Tables;
using dershaneOtomasyonu.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dershaneOtomasyonu.Repositories.TableRepositories.DegerlendirmeRepositories
{
    public interface IDegerlendirmeRepository : IBaseRepository<Degerlendirme>
    {
        Task<List<DegerlendirmeDto>> GetDegerlendirmelerAsDtoByKullaniciIdAsync(int kullaniciId);
        Task<List<Degerlendirme>> GetDegerlendirmelerByKullaniciIdAsync(int kullaniciId);
    }
}
