using dershaneOtomasyonu.Database.Tables;
using dershaneOtomasyonu.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dershaneOtomasyonu.Repositories.TableRepositories.SinifRepositories
{
    public interface ISinifRepository : IBaseRepository<Sinif>
    {
        Task<List<SinifDto>> GetSinifByKodAsync(string kod);
        Task<List<SinifDto>> GetAllAsDtoAsync();
    }
}
