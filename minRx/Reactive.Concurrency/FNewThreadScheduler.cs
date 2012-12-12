using System;
using System.Threading;

namespace minRx.Reactive.Concurrency
{
    public static class FNewThreadScheduler
    {
        /// <summary>
        /// Schedules an action to be executed
        /// </summary>
        /// <param name="action">Action to execute</param>
        /// <returns>A disposable action</returns>
        public static Action Schedule(Action action)
        {
            var t = new Thread(() => action());
            Console.WriteLine("Start new thread: " + Thread.CurrentThread.ManagedThreadId);
            t.Start();
            return () =>
            {
                t.Abort();
                Console.WriteLine("Dispose new thread: " + Thread.CurrentThread.ManagedThreadId);
            };  //The disposable action
        }

        public static Func<Action, Action> Default()
        {
            return Schedule;
        }
        //public static Action<Action<Action>> Default()
        //{
        //    return unitOfWork =>
        //    {
        //        Thread t = null;
        //        t = new Thread(() => unitOfWork(() =>
        //                                        {
        //                                            Console.WriteLine("Dispose new thread: " + Thread.CurrentThread.ManagedThreadId);
        //                                            if (t != null) 
        //                                                t.Abort();
        //                                        }));
        //        Console.WriteLine("Start new thread: " + Thread.CurrentThread.ManagedThreadId);
        //        t.Start();
        //    };
        //}

        
    }
}
