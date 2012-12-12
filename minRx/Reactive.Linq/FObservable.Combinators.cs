using System;

namespace minRx.Reactive.Linq
{
	public static partial class FObservable
	{
        /// <summary>
        /// Projects elements in a sequence given a selector function.
        /// </summary>
        /// <typeparam name="T">Source sequence element type.</typeparam>
        /// <typeparam name="R">Result sequence element type.</typeparam>
        /// <param name="source">Source sequence.</param>
        /// <param name="selector">Selector to apply to source elements.</param>
        /// <returns>Result sequence.</returns>
        public static Func<Action<Maybe<R>>, Action> Select<T, R>(this Func<Action<Maybe<T>>, Action> source, Func<T, R> selector)
        {
            return source.Bind(t => FObservable.Return(selector(t)));
        }

        /// <summary>
        /// Filters elements in a sequence given a predicate.
        /// </summary>
        /// <typeparam name="T">Source sequence element type.</typeparam>
        /// <param name="source">Source sequence.</param>
        /// <param name="filter">Predicate to apply to source elements.</param>
        /// <returns>Filtered sequence.</returns>
        public static Func<Action<Maybe<T>>, Action> Where<T>(this Func<Action<Maybe<T>>, Action> source, Predicate<T> filter)
        {
            return source.Bind(t => filter(t) ? FObservable.Return(t) : FObservable.Empty<T>());
        }
    }
}
