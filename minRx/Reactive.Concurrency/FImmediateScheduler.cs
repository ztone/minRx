using System;

namespace minRx.Reactive.Concurrency
{
    public static class FImmediateScheduler
    {
        /// <summary>
        /// Schedules an action to be executed
        /// </summary>
        /// <param name="action">Action to execute</param>
        /// <returns>A disposable action</returns>
        public static Action Schedule(Action action)
        {
            action();
            return () => { };  //The disposable action
        }

        public static Func<Action,Action> Instance()
        {
            return Schedule;
        }
    }
}
