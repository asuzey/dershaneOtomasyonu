using dershaneOtomasyonu.Database;
using dershaneOtomasyonu.Database.Tables;
using dershaneOtomasyonu.DTO;
using Mapster;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dershaneOtomasyonu.Repositories.TableRepositories.RoleRepositories
{
    public class RoleRepository : BaseRepository<Role>, IRoleRepository
    {
        public RoleRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<List<RoleDto>> GetRolByRolAdiAsync(string roleName)
        {
            var roles = await _context.Roller
                .Where(x => x.RolAdi == roleName)
                .ToListAsync();
            return roles.Adapt<List<RoleDto>>();
        }
    }
}
