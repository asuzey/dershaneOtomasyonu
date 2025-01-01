using dershaneOtomasyonu.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dershaneOtomasyonu.Repositories.TableRepositories.RoleRepositories
{
    public interface IRoleRepository
    {
        Task<List<RoleDto>> GetRolByRolAdiAsync(string roleName);
    }
}
