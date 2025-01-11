using dershaneOtomasyonu.Database.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dershaneOtomasyonu.Repositories.TableRepositories.NotRepositories
{
    public interface INotRepository : IBaseRepository<Not>
    {
        Task<List<Not>> GetNotsByKullaniciIdAsync(int kullaniciId);
    }
}
