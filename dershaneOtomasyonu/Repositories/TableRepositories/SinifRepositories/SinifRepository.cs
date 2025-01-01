using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dershaneOtomasyonu.Database;
using dershaneOtomasyonu.Database.Tables;
using dershaneOtomasyonu.DTO;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace dershaneOtomasyonu.Repositories.TableRepositories.SinifRepositories
{
    public class SinifRepository : BaseRepository<Sinif>, ISinifRepository
    {
        public SinifRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<List<SinifDto>> GetSinifByKodAsync(string kod)
        {
            var sinif = await _context.Siniflar
                .Where(s => s.Kodu == kod)
                .ToListAsync();
            return sinif.Adapt<List<SinifDto>>();
        }

        public async Task<List<SinifDto>> GetAllAsDtoAsync()
        {
            var siniflar = await _context.Siniflar.ToListAsync();
            return siniflar.Adapt<List<SinifDto>>();
        }
    }
}
