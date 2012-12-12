using System;
using System.Threading;
using System.Threading.Tasks;

namespace minRx.Reactive.Concurrency
{
    public static class FThreadPoolScheduler
    {
        public static Action Schedule(Action action)
        {
            var token = new CancellationTokenSource();
            Task.Factory.StartNew(() =>
                                      {
                                          if(!token.IsCancellationRequested)
                                              action();
                                      }, token.Token);
            return () =>
                       {
                           token.Cancel();
                           Console.WriteLine("Dispose theadpool");
                       };
        }

        public static Func<Action,Action> Default()
        {
            return Schedule;
        }
    }
}
