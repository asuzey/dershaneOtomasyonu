using dershaneOtomasyonu.Database;
using dershaneOtomasyonu.Database.Tables;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace dershaneOtomasyonu.Repositories.TableRepositories.YoklamaRepositories
{
    public class YoklamaRepository : BaseRepository<Yoklama>, IYoklamaRepository
    {
        private readonly BlockingCollection<Func<Task>> _taskQueue = new();
        private readonly Thread _workerThread;
        private volatile bool _isRunning = true;

        public YoklamaRepository(AppDbContext context) : base(context)
        {
            // Arka plan thread'i başlat
            _workerThread = new Thread(ProcessQueue)
            {
                IsBackground = true
            };
            _workerThread.Start();
        }

        private void ProcessQueue()
        {
            while (_isRunning || !_taskQueue.IsCompleted)
            {
                try
                {
                    if (_taskQueue.TryTake(out var taskFunc, Timeout.Infinite))
                    {
                        taskFunc().Wait(); // Görevleri sırayla çalıştır
                    }
                }
                catch (Exception ex)
                {
                    // Kuyruk işlem hatası için fallback
                    Console.WriteLine($"Servis kuyruğunda hata: {ex.Message}");
                }
            }
        }

        public void StopProcessing()
        {
            _isRunning = false;
            _taskQueue.CompleteAdding();
            _workerThread.Join();
        }

        private Task EnqueueTask(Func<Task> taskFunc)
        {
            var tcs = new TaskCompletionSource();
            _taskQueue.Add(async () =>
            {
                try
                {
                    await taskFunc();
                    tcs.SetResult();
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            });
            return tcs.Task;
        }

        private Task<T> EnqueueTask<T>(Func<Task<T>> taskFunc)
        {
            var tcs = new TaskCompletionSource<T>();
            _taskQueue.Add(async () =>
            {
                try
                {
                    var result = await taskFunc();
                    tcs.SetResult(result);
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            });
            return tcs.Task;
        }

        public Task<List<Yoklama>> GetByDersKayitIdAsync(int dersKayitId)
        {
            return EnqueueTask(async () =>
            {
                return await _context.Yoklamalar
                    .Where(y => y.DersKayitId == dersKayitId && y.AyrilmaTarihi == null)
                    .ToListAsync();
            });
        }

        public Task<Yoklama> GetByKullaniciIdAndDersKayitIdAsync(int dersKayitId, int kullaniciId)
        {
            return EnqueueTask(async () =>
            {
                return await _context.Yoklamalar
                    .FirstOrDefaultAsync(y => y.DersKayitId == dersKayitId && y.KullaniciId == kullaniciId);
            });
        }
    }
}
