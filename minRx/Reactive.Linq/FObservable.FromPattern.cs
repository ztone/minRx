using System;
using System.Threading;

namespace minRx.Reactive.Linq
{
    public class EventPattern<T> where T : EventArgs
    {
        public T EventArgs { get; set; }
        public object Sender { get; set; }
    }

    public static partial class FObservable
    {
        /// http://social.msdn.microsoft.com/Forums/en-US/rx/thread/5f4df41c-6df1-42be-b7bb-bbf072143c24/#d2149f26-de64-49fa-a662-929da9aea0ec
        /// <summary>
        /// Converts a .NET event to an observable sequence
        /// </summary>
        /// <typeparam name="T">The source sequence which are the EventArgs</typeparam>
        /// <param name="target">Object instance that exposes the event to convert</param>
        /// <param name="eventName">Name of the event to convert</param>
        /// <returns>An observable sequence of EventArgs</returns>
        public static Func<Action<Maybe<EventPattern<T>>>, Action> FromEventPattern<T>(object target, string eventName) where T : EventArgs
        {
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }

            var eventInfo = target.GetType().GetEvent(eventName);
            if (eventInfo == null)
            {
                throw new ArgumentException(string.Format("Event {0} is invalid", eventName));
            }

            return FObservable.Create<EventPattern<T>>(observer => 
            {
                var firstArgument = new EventHandler<T>((_, args) => observer.OnNext( new EventPattern<T> {EventArgs = args, Sender = target }));
                var delegateMethod = Delegate.CreateDelegate(eventInfo.EventHandlerType,firstArgument,typeof(EventHandler<T>).GetMethod("Invoke"));
                eventInfo.AddEventHandler(target, delegateMethod);

                return () => eventInfo.RemoveEventHandler(target, delegateMethod);
            });
        }

        /// <summary>
        /// Converts an Begin/End invoke function pair into an asyncronous function
        /// </summary>
        /// <typeparam name="T1">Type of the first paramenter to the asyncronous function</typeparam>
        /// <typeparam name="T">The source element type</typeparam>
        /// <param name="begin">Delagate the begins the asyncronous operation</param>
        /// <param name="end">Delagate the begins the asyncronous operation</param>
        /// <returns>A function that returns a observable sequence</returns>
        public static Func<T1, Func<Action<Maybe<T>>, Action>> FromAsyncPattern<T1, T>(Func<T1, AsyncCallback, object, IAsyncResult> begin, Func<IAsyncResult, T> end)
        {
            return argument => FObservable.Create<T>(observer =>
            {
                var syncLock = new object();
                var cancelationToken = false;
                begin(argument,
                    new AsyncCallback(asyncresult =>
                    {
                        try
                        {
                            System.Diagnostics.Debug.WriteLine("SubscribeOn: {0} - AsyncCallback", Thread.CurrentThread.ManagedThreadId);

                            lock (syncLock)
                            {
                                if (cancelationToken)
                                {
                                    asyncresult.AsyncWaitHandle.Close();
                                    asyncresult.AsyncWaitHandle.Dispose();
                                    return;
                                }

                                if (asyncresult.IsCompleted)
                                    observer(new Maybe<T>.Some(end(asyncresult)));
                                observer(new Maybe<T>.None());
                            }
                        }
                        catch(ObjectDisposedException)
                        {
                            Console.WriteLine("DISPOSED ASYNC");
                        }
                        catch (Exception ex)
                        {
                            observer(new Maybe<T>.Ex(ex));
                        }
                    }), null);
                return () =>
                           {
                               lock(syncLock)
                               {
                                   cancelationToken = true;
                               }
                           };
            });
        }

        /// <summary>
        /// Repeats the given source as long the specified condition holds, where the condition is 
        /// evaluated before each repeated source is subscribed to.
        /// The sequence is allowed to finish before the condition is evaluated again.
        /// If the source throws an exception the sequence will end.
        /// </summary>
        /// <typeparam name="T">The source element type</typeparam>
        /// <param name="predicate">Condition to be evaluated before subscribtion to the source, to determine 
        /// whether repetition of the source is required</param>
        /// <param name="source">The source to be repeated</param>
        /// <returns>An observable sequence</returns>
        public static Func<Action<Maybe<T>>, Action> While<T>(Func<bool> predicate, Func<Action<Maybe<T>>, Action> source)
        {
            return observer =>  //FObservable.Create
            {
                Action a = null;
                var done = false;
                var ok = true;
                while (predicate() && ok)
                {
                    done = false;
                    a = source(maybe =>
                    {
                        if (maybe is Maybe<T>.Some)
                        {
                            observer.OnNext(maybe.Value);
                        }
                        else if (maybe is Maybe<T>.None)
                        {
                            done = true;
                        }
                        else if (maybe is Maybe<T>.Ex)
                        {
                            observer.OnException(maybe.Exception);
                            ok = false;
                            done = true;
                        }
                        else
                        {
                            observer.OnException(new Exception("Unknown value"));
                            done = true;
                        }
                    });

                    while (!done)
                    {
                        Thread.Yield();
                        Thread.Sleep(200);  //else the CPU goes to 100%
                    }
                }
                observer.OnCompleted();
                return () => { done = true; ok = false; if (a != null) a(); };
            };
        }
    }
}
