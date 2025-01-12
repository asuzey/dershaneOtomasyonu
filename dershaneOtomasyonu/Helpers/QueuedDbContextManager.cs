using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace dershaneOtomasyonu.Helpers
{
    

    public class QueuedDbContextManager<TContext> where TContext : DbContext
    {
        private readonly TContext _dbContext;
        private readonly BlockingCollection<Func<Task>> _taskQueue = new();
        private readonly Thread _workerThread;
        private volatile bool _isRunning = true;

        public QueuedDbContextManager(TContext dbContext)
        {
            _dbContext = dbContext;

            // İşlemleri sırayla gerçekleştiren thread'i başlat
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
                    Console.WriteLine($"Db işlem hatası: {ex.Message}");
                }
            }
        }

        public void StopProcessing()
        {
            _isRunning = false;
            _taskQueue.CompleteAdding();
            _workerThread.Join();
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

        public Task<T> ExecuteAsync<T>(Func<TContext, Task<T>> dbOperation)
        {
            return EnqueueTask(() => dbOperation(_dbContext));
        }

        public Task ExecuteAsync(Func<TContext, Task> dbOperation)
        {
            return EnqueueTask(() => dbOperation(_dbContext));
        }
    }

}
