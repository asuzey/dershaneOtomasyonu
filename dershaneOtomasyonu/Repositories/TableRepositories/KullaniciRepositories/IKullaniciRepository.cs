using dershaneOtomasyonu.Database.Tables;
using dershaneOtomasyonu.DTO;
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
        Task<Kullanici> GetByEmailAsync(string userName);
        Task<Kullanici> GetByUserNameAndPasswordAsync(string userName, string password);
        Task<IEnumerable<Kullanici>> GetByRoleIdAsync(int roleId);
        Task<List<KullaniciDto>> GetAllAsDtoAsync();
        Task<List<Kullanici>> GetAllTeachersAsync();
        Task<List<Kullanici>> GetAllStudentsAsync();
        Task<List<RaporYoklamaDto>> GetAllYoklamaRaporByOgrenciIdAsync(int ogrenciId);
    }
}
