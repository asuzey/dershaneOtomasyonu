using dershaneOtomasyonu.Database;
using dershaneOtomasyonu.Database.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dershaneOtomasyonu.Repositories.TableRepositories.KullaniciNotRepositories
{
    public class KullaniciNotRepository : BaseRepository<KullaniciNot>, IKullaniciNotRepository
    {
        public KullaniciNotRepository(AppDbContext context) : base(context)
        {

        }
    }
}
