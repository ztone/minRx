// A reference to the author of these three operators Ana, Cata and Bind.
// Some operators have been slightly modified to fix the minRx API.

//Reference:
// Project "MinLINQ" - Bart De Smet (C) 2010
//
// http://blogs.bartdesmet.net/blogs/bart/archive/2010/01/01/the-essence-of-linq-minlinq.aspx
//
// This project is meant as an illustration of how an academically satifying layering
// of a LINQ to Objects implementation can be realized using monadic concepts and only
// three primitives: anamorphism, bind and catamorphism.
//
// The code in this project is not meant to be used in production and no guarantees are
// made about its functionality. Use it for academic stimulation purposes only. To use
// LINQ for real, use System.Linq in .NET 3.5 or higher.
//
// All of the source code may be used in presentations of LINQ or for other educational
// purposes, but references to http://www.codeplex.com/LINQSQO and the blog post referred
// to above - "The Essence of LINQ - MinLINQ" - are required.
//
using System;

namespace minRx.Reactive.Linq
{
    public static partial class FObservable
	{
        /// <summary>
        /// Anamorphism:  T --> (T --> bool) --> (T --> T) --> (T --> R) --> M{R}
        /// </summary>
        /// <typeparam name="T">Source generator input type.</typeparam>
        /// <param name="seed">Seed value.</param>
        /// <param name="condition">Terminating condition.</param>
        /// <param name="next">Iteration function.</param>
        /// <param name="scheduler">The scheduler to run the sequence</param>
        /// <returns>A disposeable sequence.</returns>
        public static Func<Action<Maybe<T>>, Action> Ana<T>(T seed, Func<T, bool> condition,
            Func<T, T> next, Func<Action, Action> scheduler)
        {
            return Ana(seed, condition, next, _ => _, scheduler);
        }

	    /// <summary>
	    /// Anamorphism:  T --> (T --> bool) --> (T --> T) --> (T --> R) --> M{R}
	    /// </summary>
	    /// <typeparam name="T">Source generator input type.</typeparam>
	    /// <typeparam name="R">Result sequence element type.</typeparam>
	    /// <param name="seed">Seed value.</param>
	    /// <param name="condition">Terminating condition.</param>
	    /// <param name="next">Iteration function.</param>
	    /// <param name="result">Result selector.</param>
        /// <param name="scheduler">The scheduler to run the sequence</param>
	    /// <returns>A disposeable sequence.</returns>
        public static Func<Action<Maybe<R>>, Action> Ana<T, R>(T seed, Func<T, bool> condition, Func<T, T> next,
            Func<T, R> result, Func<Action, Action> scheduler)
        {
            var count = 0;
            return o => scheduler(() =>
                                      {
                                          for (T t = seed; condition(t); t = next(t))
                                              o(new Maybe<R>.Some(result(t), count++)); 
                                          o(new Maybe<R>.None(count++));
                                      });

        }

	    /// <summary>
	    /// Catamorphism.
	    /// </summary>
	    /// <typeparam name="T">Source sequence element type.</typeparam>
	    /// <typeparam name="R">Result object type.</typeparam>
	    /// <param name="source">Source sequence.</param>
	    /// <param name="seed">Seed value.</param>
        /// <param name="condition">Terminating condition.</param>
	    /// <param name="func">Aggregator function.</param>
	    /// <returns>Result of the catamorphic operation on the sequence.</returns>
        public static R Cata<T, R>(this Func<Action<Maybe<T>>, Action> source, R seed, Predicate<R> condition, Func<R, T, R> func)
        {
            R result = seed;

            bool end = false;
            source(x =>
            {
                if (x is Maybe<T>.Some && !end && condition(result))
                    result = func(result, x.Value);
                else
                    end = true; // or break using exception
            });

            return result;
        }

        /// <summary>
        /// Bind.
        /// </summary>
        /// <typeparam name="T">Source sequence element type.</typeparam>
        /// <typeparam name="R">Result sequence element type.</typeparam>
        /// <param name="source">Source sequence.</param>
        /// <param name="selector">Selector for source elements.</param>
        /// <returns>Result sequence.</returns>
        public static Func<Action<Maybe<R>>, Action> Bind<T, R>(this Func<Action<Maybe<T>>, Action> source, Func<T, Func<Action<Maybe<R>>, Action>> selector)
        {
            return observer => source(maybe =>
            {
                if (maybe is Maybe<T>.None)
                {
                    observer(new Maybe<R>.None());
                }
                else
                {
                    selector(maybe.Value)(option =>
                    {
                        if (option is Maybe<R>.Some)
                            observer(option);
                    });
                }
            });
        }
	}
}
