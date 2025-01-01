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

namespace dershaneOtomasyonu.Repositories.TableRepositories.DerslerRepositories
{
    public class DerslerRepository : BaseRepository<Ders>, IDerslerRepository
    {
        public DerslerRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<List<DerslerDto>> GetAllAsDtoAsync()
        {
            var dersler = await _context.Dersler.ToListAsync();
            return dersler.Adapt<List<DerslerDto>>();
        }
    }
}
