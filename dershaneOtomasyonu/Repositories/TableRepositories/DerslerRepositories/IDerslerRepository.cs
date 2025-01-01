using dershaneOtomasyonu.Database.Tables;
using dershaneOtomasyonu.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dershaneOtomasyonu.Repositories.TableRepositories.DerslerRepositories
{
    public interface IDerslerRepository : IBaseRepository<Ders>
    {
        Task<List<DerslerDto>> GetAllAsDtoAsync();
    }
}
