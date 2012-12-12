using System;

namespace minRx.Reactive.Linq
{
	public  static partial class FObservable
	{
        /// <summary>
        /// Call an action on each item in the list
        /// </summary>
        /// <typeparam name="T">Source sequence element type.</typeparam>
        /// <param name="source">Source sequence.</param>
        /// <param name="action">A scheduler.</param>
        /// <returns>Result sequence.</returns>
        public static Func<Action<Maybe<T>>, Action> ForEach<T>(this Func<Action<Maybe<T>>, Action> source, Action<Action> action)
        {
            return observer => source(maybe => action(() => observer(maybe)));
        }

        /// <summary>
        /// Invokes an action for each element in the observable sequence.  
        /// The action will not affect the orignal sequence
        /// This method can be used for debugging, logging etc.
        /// </summary>
        /// <typeparam name="T">Sequence element type</typeparam>
        /// <param name="source">Source sequence</param>
        /// <param name="onNext">Action to invoke on each element in the sequence</param>
        /// <returns>A unmodified sequence</returns>
        public static Func<Action<Maybe<T>>, Action> Do<T>(this Func<Action<Maybe<T>>, Action> source, Action<T> onNext)
        {
            return observer => source(maybe =>
                                        {
                                            if(maybe is Maybe<T>.Some)
                                                onNext(maybe.Value);
                                            observer(maybe);
                                        });
        }
	}
}

