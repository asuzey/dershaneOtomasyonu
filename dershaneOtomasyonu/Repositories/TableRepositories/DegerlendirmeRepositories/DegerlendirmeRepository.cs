﻿using dershaneOtomasyonu.Database;
using dershaneOtomasyonu.Database.Tables;
using dershaneOtomasyonu.DTO;
using Mapster;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dershaneOtomasyonu.Repositories.TableRepositories.DegerlendirmeRepositories
{
    public class DegerlendirmeRepository : BaseRepository<Degerlendirme>, IDegerlendirmeRepository
    {
        public DegerlendirmeRepository(AppDbContext context) : base(context)
        {

        }

        public async Task<List<DegerlendirmeDto>> GetDegerlendirmelerAsDtoByKullaniciIdAsync(int kullaniciId)
        {
            var degerlendirme = await _context.Degerlendirmeler
                .Where(d => d.KullaniciId == kullaniciId)
                .ToListAsync();
            return degerlendirme.Adapt<List<DegerlendirmeDto>>();
        }

        public async Task<List<Degerlendirme>> GetDegerlendirmelerByKullaniciIdAsync(int kullaniciId)
        {
            return await _context.Degerlendirmeler
                .Where(d => d.KullaniciId == kullaniciId)
                .OrderBy(d => d.Id)
                .Include(d => d.Ders)
                .ToListAsync();
        }
    }
}
