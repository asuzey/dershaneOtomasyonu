using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace dershaneOtomasyonu.Repositories
{
    public class QueuedBaseRepository<T> : IBaseRepository<T> where T : class
    {
        private readonly BaseRepository<T> _baseRepository;
        private readonly BlockingCollection<Func<Task>> _taskQueue = new();
        private readonly Thread _workerThread;
        private volatile bool _isRunning = true;

        public QueuedBaseRepository(BaseRepository<T> baseRepository)
        {
            _baseRepository = baseRepository;

            // İşleri sırayla gerçekleştiren thread'i başlat
            _workerThread = new Thread(ProcessQueue)
            {
                IsBackground = true // Uygulama kapanırken otomatik durması için
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
                        taskFunc().Wait(); // İşlemi sırayla gerçekleştir
                    }
                }
                catch (Exception ex)
                {
                    // Kuyruk işlem hatası fallback
                    Console.WriteLine($"Task işlem hatası: {ex.Message}");
                }
            }
        }

        public void StopProcessing()
        {
            _isRunning = false;
            _taskQueue.CompleteAdding();
            _workerThread.Join();
        }

        private Task<TOutput> EnqueueTask<TOutput>(Func<Task<TOutput>> taskFunc)
        {
            var tcs = new TaskCompletionSource<TOutput>();
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

        public Task<T> GetByIdAsync(int id)
        {
            return EnqueueTask(() => _baseRepository.GetByIdAsync(id));
        }

        public Task<IEnumerable<T>> GetAllAsync()
        {
            return EnqueueTask(() => _baseRepository.GetAllAsync());
        }

        public Task AddAsync(T entity)
        {
            return EnqueueTask(() => _baseRepository.AddAsync(entity));
        }

        public Task<T> UpdateAsync(T entity)
        {
            return EnqueueTask(() => _baseRepository.UpdateAsync(entity));
        }

        public Task DeleteAsync(int id)
        {
            return EnqueueTask(() => _baseRepository.DeleteAsync(id));
        }

        public IQueryable<T> Query()
        {
            // Query işlemleri doğrudan çağrılabilir, çünkü bunlar yalnızca veriyi okumayı içerir
            return _baseRepository.Query();
        }
    }
}
