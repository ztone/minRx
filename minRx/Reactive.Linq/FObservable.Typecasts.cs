using System;
using System.Collections.Generic;

namespace minRx.Reactive.Linq
{
	public static partial class FObservable
	{
        /// <summary>
        /// Converts an FObservable to an IEnumerable
        /// </summary>
        /// <typeparam name="T">Sequence element type</typeparam>
        /// <param name="source">Source to be converted</param>
        /// <returns>A IEnumerable sequence</returns>
        public static IEnumerable<T> AsEnumerable<T>(this Func<Action<Maybe<T>>, Action> source)
        {
            var result = new List<Maybe<T>>();
            var dispose = source(result.Add);
            try
            {
                foreach (var maybe in result)
                {
                    if (maybe is Maybe<T>.Some)
                    {
                        yield return maybe.Value;
                    }
                }
            }
            finally
            {
                dispose();
            }
        }
	}
}
