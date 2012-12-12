using System;

namespace minRx.Reactive.Linq
{
	public static partial class FObservable
	{
        /// <summary>
        /// Computes the sum of the elements in the sequence.
        /// </summary>
        /// <param name="source">Source sequence.</param>
        /// <returns>Sum of the elements in the sequence.</returns>
        public static int Sum(this Func<Action<Maybe<int>>, Action> source)
        {
            return source.Cata(0, _ => true, (sum, x) => checked(sum + x));
        }

        /// <summary>
        /// Gets the first element in the sequence, or default(T) if none is found.
        /// </summary>
        /// <typeparam name="T">Source sequence element type.</typeparam>
        /// <param name="source">Source sequence.</param>
        /// <returns>First element in the sequence, or default(T) if none is found.</returns>
        public static T FirstOrDefault<T>(this Func<Action<Maybe<T>>, Action> source)
        {
            return source.Cata(new {Count = 0, Value = default(T)},
                n => n.Count < 1, 
                (sum, x) => new{ Count = sum.Count+1, Value = x}).Value;
        }

        /// <summary>
        /// Gets the last element in the sequence for which the predicate holds, or default(T) if none is found.
        /// </summary>
        /// <typeparam name="T">Source sequence element type.</typeparam>
        /// <param name="source">Source sequence.</param>
        /// <returns>Last element in the sequence for which the predicate holds, or default(T) if none is found.</returns>
        public static T LastOrDefault<T>(this Func<Action<Maybe<T>>, Action> source)
        {
            return source.Cata(default(T), _ => true, (_, newValue) => newValue);
        }

        /// <summary>
        /// Counts the number of elements in the sequence.
        /// </summary>
        /// <typeparam name="T">Source sequence element type.</typeparam>
        /// <param name="source">Source sequence.</param>
        /// <returns>Number of elements in the sequence.</returns>
        public static int Count<T>(this Func<Action<Maybe<T>>, Action> source)
        {
            return source.Cata(0, _ => true, (count, _) => checked(count + 1));
        }
	}
}
