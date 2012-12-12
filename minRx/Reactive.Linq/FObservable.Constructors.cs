using System;
using minRx.Reactive.Concurrency;

namespace minRx.Reactive.Linq
{
	public static partial class FObservable
	{
        /// <summary>
        /// Empty sequence constructor.
        /// </summary>
        /// <typeparam name="T">Result sequence element type.</typeparam>
        /// <returns>Empty sequence.</returns>
        public static Func<Action<Maybe<T>>, Action> Empty<T>()
        {
            return Empty<T>(FImmediateScheduler.Instance());
        }

        /// <summary>
        /// Empty sequence constructor.
        /// </summary>
        /// <typeparam name="T">Result sequence element type.</typeparam>
        /// <param name="scheduler">The scheduler to run the sequence</param>
        /// <returns>Empty sequence.</returns>
        public static Func<Action<Maybe<T>>, Action> Empty<T>(Func<Action, Action> scheduler)
        {
            /*
             * Direct implementation
             * 
             * return o => o(new Maybe<T>.None());
             */

            return Ana(default(T), _ => false, _ => _, scheduler);
        }

        /// <summary>
        /// Element repeating sequence constructor.
        /// </summary>
        /// <typeparam name="T">Result sequence element type.</typeparam>
        /// <param name="value">Element to repeat.</param>
        /// <param name="count">Number of times to repeat the element.</param>
        /// <returns>Element repeating sequence.</returns>
        public static Func<Action<Maybe<T>>, Action> Repeat<T>(T value, int count)
        {
            return Repeat(value, count, FImmediateScheduler.Instance());
        }

	    /// <summary>
	    /// Element repeating sequence constructor.
	    /// </summary>
	    /// <typeparam name="T">Result sequence element type.</typeparam>
	    /// <param name="value">Element to repeat.</param>
	    /// <param name="count">Number of times to repeat the element.</param>
        /// <param name="scheduler">The scheduler to run the sequence</param>
	    /// <returns>Element repeating sequence.</returns>
	    public static Func<Action<Maybe<T>>, Action> Repeat<T>(T value, int count, Func<Action,Action> scheduler)
        {
            var i = 0;
            return Ana(value, x => i++ < count, _ => _, scheduler);
        }


        /// <summary>
        /// Single element sequence constructor.
        /// </summary>
        /// <typeparam name="T">Result sequence element type.</typeparam>
        /// <param name="value">Single element.</param>
        /// <returns>Single element sequence.</returns>
        public static Func<Action<Maybe<T>>, Action> Return<T>(T value)
        {
            return Return(value, FImmediateScheduler.Instance());
        }

        /// <summary>
        /// Single element sequence constructor.
        /// </summary>
        /// <typeparam name="T">Result sequence element type.</typeparam>
        /// <param name="value">Single element.</param>
        /// <param name="scheduler">The scheduler to run the sequence</param>
        /// <returns>Single element sequence.</returns>
        public static Func<Action<Maybe<T>>, Action> Return<T>(T value, Func<Action, Action> scheduler)
        {
            /*
             * Direct implementation
             * 
             * return o =>
             * {
             *    o(new Maybe<T>.Some(value));
             *    o(new Maybe<T>.None());
             *};
             */
            return Ana(0,  n => n < 1, n => n + 1, _ => value, scheduler);
        }

        /// <summary>
        /// Integral value range sequence constructor.
        /// </summary>
        /// <param name="start">First element in sequence.</param>
        /// <param name="count">Number of elements in sequence.</param>
        /// <returns>Integral value range sequence.</returns>
        public static Func<Action<Maybe<int>>, Action> Range(int start, int count)
        {
            return Range(start, count, FImmediateScheduler.Instance());
        }

        /// <summary>
        /// Integral value range sequence constructor.
        /// </summary>
        /// <param name="start">First element in sequence.</param>
        /// <param name="count">Number of elements in sequence.</param>
        /// <param name="scheduler">The scheduler to run the sequence</param>
        /// <returns>Integral value range sequence.</returns>
        public static Func<Action<Maybe<int>>, Action> Range(int start, int count, Func<Action, Action> scheduler)
        {
            if (count < 0)
                throw new ArgumentOutOfRangeException("count");

            return Ana(start, n => n < start + count, n => n + 1, scheduler);
        }

	    /// <summary>
        /// Generates the infinite sequence of natural numbers.
        /// </summary>
        /// <returns>Infinite sequence of natural numbers.</returns>
        public static Func<Action<Maybe<int>>, Action> Naturals()
	    {
	        return Naturals(FImmediateScheduler.Instance());
	    }

        /// <summary>
        /// Generates the infinite sequence of natural numbers.
        /// </summary>
        /// <param name="scheduler">Scheduler to run the generator loop on</param>
        /// <returns>Infinite sequence of natural numbers.</returns>
        public static Func<Action<Maybe<int>>, Action> Naturals(Func<Action,Action> scheduler)
	    {
            return Ana(0, _ => true, n => checked(n + 1), scheduler);
	    }

        /// <summary>
        /// Generates an observable sequence by running a loop producing the sequence
        /// </summary>
        /// <typeparam name="T">Source sequence element type.</typeparam>
        /// <typeparam name="R">Result sequence element type.</typeparam>
        /// <param name="seed">Initial state</param>
        /// <param name="condition">Condition to terminate generation</param>
        /// <param name="next">Iteration step function</param>
        /// <param name="result">Selector function for results produced in the sequence</param>
        /// <returns>A generated element sequence</returns>
        public static Func<Action<Maybe<R>>, Action> Generate<T, R>(T seed, Func<T, bool> condition, Func<T, T> next, 
            Func<T,R> result)
        {
            return Generate(seed, condition, next, result, FImmediateScheduler.Instance());
        }

        /// <summary>
        /// Generates an observable sequence by running a loop producing the sequence
        /// </summary>
        /// <typeparam name="T">Source sequence element type.</typeparam>
        /// <typeparam name="R">Result sequence element type.</typeparam>
        /// <param name="seed">Initial state</param>
        /// <param name="condition">Condition to terminate generation</param>
        /// <param name="next">Iteration step function</param>
        /// <param name="result">Selector function for results produced in the sequence</param>
        /// <param name="scheduler">The scheduler to run the sequence</param>
        /// <returns>A generated element sequence</returns>
        public static Func<Action<Maybe<R>>, Action> Generate<T, R>(T seed, Func<T, bool> condition, Func<T, T> next, 
            Func<T,R> result, Func<Action, Action> scheduler)
        {
            return Ana(seed, condition, next, result, scheduler);
        }

        /// <summary>
        /// Creates an observable sequence from a specified Subscribe method implementation
        /// Using this method is actully just syntatic sugar
        /// </summary>
        /// <typeparam name="T">Sequence element type</typeparam>
        /// <param name="observable">Implementation of the resulting observable sequence Subscribe method</param>
        /// <returns>A created element sequence</returns>
        public static Func<Action<Maybe<T>>, Action> Create<T>(Func<Action<Maybe<T>>, Action> observable)
        {
            return observable;
        }
	}
}
