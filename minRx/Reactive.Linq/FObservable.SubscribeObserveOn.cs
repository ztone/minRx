using System;
using System.ComponentModel;
using System.Threading;

namespace minRx.Reactive.Linq
{
    public partial class FObservable
    {
        /// <summary>
        /// Wraps the source sequence in order to run its subscription logic on
        /// the specified scheduler.  This subscription returns a disposable function for the
        /// scheduler.
        /// </summary>
        /// <typeparam name="T">Sequence element type</typeparam>
        /// <param name="source">The source sequence</param>
        /// <param name="scheduler">The scheduler to preform subscription on</param>
        /// <returns>A FObservable sequence</returns>
        public static Func<Action<Maybe<T>>, Action> SubscribeOn<T>(this Func<Action<Maybe<T>>, Action> source, Func<Action, Action> scheduler)
        {
            return observer => scheduler(() => source(observer));
        }

        /// <summary>
        /// Wraps the source sequence in order to run its observer callbacks on the ISynchronizeInvoke context
        /// </summary>
        /// <typeparam name="T">The source element type</typeparam>
        /// <param name="source">The source sequence</param>
        /// <param name="control">A class that implements ISyncronizedInvoke interface to notify observers on</param>
        /// <returns>The source sequence</returns>
        public static Func<Action<Maybe<T>>, Action> ObserveOn<T>(this Func<Action<Maybe<T>>, Action> source, ISynchronizeInvoke control)
        {
            /* Direct implementation:
             *
             * return o => observer(x =>
             * {
             *     if (control.InvokeRequired)
             *     {
             *         control.Invoke(new EventHandler(delegate { o(x); }), null);
             *     }
             *     else
             *     {
             *         o(x);
             *     }
             * });
             */

            return source.ForEach(action =>
            {
                lock (control)
                {
                    if (control.InvokeRequired)
                    {
                        control.Invoke(new EventHandler(delegate { action(); }), null);
                    }
                    else
                    {
                        action();
                    }
                }
            });
        }

        /// <summary>
        /// Wraps the source sequence in order to run its observer callbacks on the ISynchronizeInvoke context
        /// </summary>
        /// <typeparam name="T">The source element type</typeparam>
        /// <param name="source">The source sequence</param>
        /// <param name="context">Synchronization context to notify observers on</param>
        /// <returns>The source sequence</returns>
        public static Func<Action<Maybe<T>>, Action> ObserveOn<T>(this Func<Action<Maybe<T>>, Action> source, SynchronizationContext context)
        {
            return source.ForEach(action => context.Post(delegate { action(); }, null));
        }
    }
}
