using System;

namespace minRx.Reactive.Linq
{
    public static partial class FObservable
    {
        /// <summary>
        /// Subscribes an element handler to the observable sequence
        /// </summary>
        /// <typeparam name="T">The source element type</typeparam>
        /// <param name="source">The source sequence</param>
        /// <param name="onNext">An action to invoke for each element in observable sequence</param>
        public static Action Subscribe<T>(this Func<Action<Maybe<T>>, Action> source, Action<T> onNext)
        {
            return Subscribe(source, onNext, () => { });
        }

        /// <summary>
        /// Subscribes an element handlers to the observable sequence
        /// </summary>
        /// <typeparam name="T">The source element type</typeparam>
        /// <param name="source">The source sequence</param>
        /// <param name="onNext">An action to invoke for each element in observable sequence</param>
        /// <param name="onCompleted">The action to invoke when the sequence is finished</param>
        public static Action Subscribe<T>(this Func<Action<Maybe<T>>, Action> source, Action<T> onNext, Action onCompleted)
        {
            return Subscribe(source, onNext, () => { }, ex => { });
        }

        /// <summary>
        /// Subscribes an element handlers to the observable sequence
        /// </summary>
        /// <typeparam name="T">The source element type</typeparam>
        /// <param name="source">The source sequence</param>
        /// <param name="onNext">An action to invoke for each element in observable sequence</param>
        /// <param name="onException">The action to invoke when an exception occures in the sequence</param>
        public static Action Subscribe<T>(this Func<Action<Maybe<T>>, Action> source, Action<T> onNext, Action<Exception> onException)
        {
            return Subscribe(source, onNext, () => { }, onException);
        }

        /// <summary>
        /// Subscribes an element handler to the observable sequence
        /// </summary>
        /// <typeparam name="T">The source element type</typeparam>
        /// <param name="source">The source sequence</param>
        /// <param name="onNext">An action to invoke for each element in observable sequence</param>
        public static Action SubscribeSafe<T>(this Func<Action<Maybe<T>>, Action> source, Action<T> onNext)
        {
            return SubscribeSafe(source, onNext, () => { });
        }

        /// <summary>
        /// Subscribes an element handlers to the observable sequence
        /// </summary>
        /// <typeparam name="T">The source element type</typeparam>
        /// <param name="source">The source sequence</param>
        /// <param name="onNext">An action to invoke for each element in observable sequence</param>
        /// <param name="onCompleted">The action to invoke when the sequence is finished</param>
        public static Action SubscribeSafe<T>(this Func<Action<Maybe<T>>, Action> source, Action<T> onNext, Action onCompleted)
        {
            return Subscribe(source, onNext, () => { }, ex => { }, true);
        }

        /// <summary>
        /// Subscribes an element handlers to the observable sequence
        /// </summary>
        /// <typeparam name="T">The source element type</typeparam>
        /// <param name="source">The source sequence</param>
        /// <param name="onNext">An action to invoke for each element in observable sequence</param>
        /// <param name="onException">The action to invoke when an exception occures in the sequence</param>
        public static Action SubscribeSafe<T>(this Func<Action<Maybe<T>>, Action> source, Action<T> onNext, Action<Exception> onException)
        {
            return Subscribe(source, onNext, () => { }, onException, true);
        }

        /// <summary>
        /// Subscribes an element handlers to the observable sequence
        /// </summary>
        /// <typeparam name="T">The source element type</typeparam>
        /// <param name="source">The source sequence</param>
        /// <param name="onNext">An action to invoke for each element in observable sequence</param>
        /// <param name="onCompleted">The action to invoke when the sequence is finished</param>
        /// <param name="onException">The action to invoke when an exception occures in the sequence</param>
        public static Action SubscribeSafe<T>(this Func<Action<Maybe<T>>, Action> source, Action<T> onNext, Action onCompleted, Action<Exception> onException)
        {
            return Subscribe(source, onNext, onCompleted, onException, true);
        }

        /// <summary>
        /// Subscribes an element handlers to the observable sequence
        /// </summary>
        /// <typeparam name="T">The source element type</typeparam>
        /// <param name="source">The source sequence</param>
        /// <param name="onNext">An action to invoke for each element in observable sequence</param>
        /// <param name="onCompleted">The action to invoke when the sequence is finished</param>
        /// <param name="onException">The action to invoke when an exception occures in the sequence</param>
        /// <param name="subscribeSafe">If true then Exception is encapsulated and forwared to the exception method else the exception is thrown on this thread.</param>
        private static Action Subscribe<T>(this Func<Action<Maybe<T>>, Action> source, 
            Action<T> onNext, Action onCompleted, Action<Exception> onException, bool subscribeSafe = false)
        {
            var stop = false;
            return source(maybe =>
            {
                try
                {
                    if(stop) { return; }  // if oncompleted has been called

                    if (maybe is Maybe<T>.Some)
                    {
                        onNext(maybe.Value);
                    }
                    else if (maybe is Maybe<T>.Ex)
                    {
                        if (subscribeSafe)
                            onException(maybe.Exception);
                        else
                            throw maybe.Exception;
                    }
                    else
                    {
                        onCompleted();
                        stop = true;
                    }
                }
                catch (Exception ex)
                {
                    if (subscribeSafe)
                        onException(ex);
                    else
                        throw;
                }
            });
        }

        /* Subscribe with priority
        private static readonly object _waitLock = new object();
        private static Action Subscribe<T>(this Func<Action<Maybe<T>>, Action> source, 
            Action<T> onNext, Action onCompleted, Action<Exception> onException, bool subscribeSafe = false)
        {
            var priority = 0;
            var stop = false;
            return source(maybe =>
            {
                try
                {
                    if(stop)
                    {
                        return;
                    }

                    lock (_waitLock)
                    {
                        while (priority < maybe.Priority)
                        {
                            Monitor.Wait(_waitLock);
                        }
                    }

                    lock (_waitLock)
                    {
                        priority = maybe.Priority + 1;

                        if (maybe is Maybe<T>.Some)
                        {
                            onNext(maybe.Value);
                        }
                        else if (maybe is Maybe<T>.Ex)
                        {
                            if (subscribeSafe)
                                onException(maybe.Exception);
                            else
                                throw maybe.Exception;
                        }
                        else
                        {
                            onCompleted();
                            stop = true;
                        }
                        Monitor.PulseAll(_waitLock);
                    }
                }
                catch (Exception ex)
                {
                    if (subscribeSafe)
                        onException(ex);
                    else
                        throw;
                }
            });
        }
        */

        /* First try
        public static void Subscribe<T>(this Action<Action<Maybe<T>>> observer, Action<T> onNext, Action onCompleted)
        {
            (new Thread(() => observer(o =>
            {
                if (o is Maybe<T>.Some)
                {
                    onNext(o.Value);
                }
                else
                {
                    onCompleted();
                }
            }))).Start();
        }
        */
    }
}    
