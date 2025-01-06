using dershaneOtomasyonu.Database.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dershaneOtomasyonu.Repositories.TableRepositories.DersKayitRepositories
{
    public interface IDersKayitRepository : IBaseRepository<DersKayit>
    {//GetActiveDerslerBySinifAndOgretmenIdAsync

        Task<DersKayit> GetActiveDersBySinifAndOgretmenIdAsync(int sinifId, int ogretmenId);
        Task<List<DersKayit>> GetActiveDerslerByOgretmenIdAsync(int ogretmenId);
    }
}
