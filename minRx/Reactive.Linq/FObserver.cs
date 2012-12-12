using System;

namespace minRx.Reactive.Linq
{
    public static class FObserver 
    {
        /// <summary>
        /// Wrapper for the OnNext behaviour
        /// </summary>
        /// <typeparam name="T">The observer type</typeparam>
        /// <param name="observer">The observer method</param>
        /// <param name="value">Value for the observer</param>
        public static void OnNext<T>(this Action<Maybe<T>> observer, T value)
        {
            observer(new Maybe<T>.Some(value));
        }

        /// <summary>
        /// Wrapper for the OnCompleted behaviour.  To be called then the sequence has ended.
        /// </summary>
        /// <typeparam name="T">The observer type</typeparam>
        /// <param name="observer">The observer method</param>
        public static void OnCompleted<T>(this Action<Maybe<T>> observer)
        {
            observer(new Maybe<T>.None());
        }

        /// <summary>
        /// Wrapper for the OnException behaviour.  To be called then the sequence throws an exception.
        /// </summary>
        /// <typeparam name="T">The observer type</typeparam>
        /// <param name="observer">The observer method</param>
        /// <param name="ex">The exception that was thrown</param>
        public static void OnException<T>(this Action<Maybe<T>> observer, Exception ex)
        {
            observer(new Maybe<T>.Ex(ex));
        }
    }
}
