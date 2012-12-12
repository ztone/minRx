using System;
using System.Threading;

namespace minRx.Reactive.Linq
{
    /// <summary>
    /// Optional object type.
    /// </summary>
    /// <typeparam name="T">Type of the object.</typeparam>
    public abstract class Maybe<T>
    {
        /// <summary>
        /// Gets whether an object is present.
        /// </summary>
        public abstract bool HasValue { get; }

        /// <summary>
        /// Gets the object.
        /// </summary>
        public abstract T Value { get; }

        public Exception Exception { get; set; }

        public Action Dispose { get; set; }

        public int Priority { get; private set; }

        /// <summary>
        /// None optional type.
        /// </summary>
        public sealed class None : Maybe<T>
        {

            public None()
            {
                
            }

            public None(int priority)
            {
                Priority = priority;
            }

            /// <summary>
            /// Gets whether an object is present. Always returns false for None.
            /// </summary>
            public override bool HasValue
            {
                get
                {
                    return false;
                }
            }

            /// <summary>
            /// Gets the object. Always throws InvalidOperationException for None.
            /// </summary>
            public override T Value
            {
                get
                {
                    throw new InvalidOperationException();
                }
            }

            /// <summary>
            /// Provides a friendly string representation.
            /// </summary>
            /// <returns>None&lt;T&gt;</returns>
            public override string ToString()
            {
                return "None<" + typeof(T).Name + ">()";
            }
        }

        /// <summary>
        /// Some optional type.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        public sealed class Some : Maybe<T>
        {
            /// <summary>
            /// Object stored in Some.
            /// </summary>
            private readonly T _value;

            

            /// <summary>
            /// Creates a new Some object.
            /// </summary>
            /// <param name="value">Object stored in Some.</param>
            public Some(T value)
            {
                System.Diagnostics.Debug.WriteLine("Observed on thread: '{0}'", Thread.CurrentThread.ManagedThreadId);
                _value = value;
            }

            /// <summary>
            /// Creates a new Some object.
            /// </summary>
            /// <param name="value">Object stored in Some.</param>
            /// <param name="priority"></param>
            public Some(T value, int priority)
            {
                _value = value;
                Priority = priority;
            }

            /// <summary>
            /// Gets whether an object is present. Always returns true for Some.
            /// </summary>
            public override bool HasValue
            {
                get
                {
                    return true;
                }
            }

            /// <summary>
            /// Gets the object.
            /// </summary>
            public override T Value
            {
                get { return _value; }
            }

            /// <summary>
            /// Provides a friendly string representation.
            /// </summary>
            /// <returns>None&lt;T&gt;(Value)</returns>
            public override string ToString()
            {
                return "Some<" + typeof(T).Name + ">(" + (_value == null ? "null" : _value.ToString()) + ")";
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        public sealed class Ex : Maybe<T> 
        {
            /// <summary>
            /// Object stored in Some.
            /// </summary>
            private readonly Exception _value;



            /// <summary>
            /// Creates a new Exception object.
            /// </summary>
            /// <param name="value">Object stored in Some.</param>
            public Ex(Exception value)
            {
                _value = value;
            }

           /// <summary>
            /// Gets whether an object is present. Always returns false for Exception.
            /// </summary>
            public override bool HasValue
            {
                get
                {
                    return false;
                }
            }

            /// <summary>
            /// Exception does not return a value
            /// </summary>
            public override T Value
            {
                get { throw new Exception("", _value);  }
            }

            /// <summary>
            /// Provides a friendly string representation.
            /// </summary>
            /// <returns>None&lt;T&gt;(Value)</returns>
            public override string ToString()
            {
                return "Ex<" + typeof(T).Name + ">(" + (_value == null ? "null" : _value.ToString()) + ")";
            }
        }
    }
}
