using dershaneOtomasyonu.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dershaneOtomasyonu.Repositories.TableRepositories.DegerlendirmeRepositories
{
    public interface IDegerlendirmeRepository
    {
        Task<List<DegerlendirmeDto>> GetDegerlendirmeByKullaniciIdAsync(int kullaniciId);
    }
}
