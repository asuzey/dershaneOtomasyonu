using dershaneOtomasyonu.Database.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dershaneOtomasyonu.Repositories.TableRepositories.KullaniciRepositories
{
    public interface IKullaniciRepository : IBaseRepository<Kullanici>
    {
        Task<Kullanici> GetByUserNameAsync(string userName);
        Task<Kullanici> GetByUserNameAndPasswordAsync(string userName, string password);
        Task<IEnumerable<Kullanici>> GetByRoleIdAsync(int roleId);
    }
}
