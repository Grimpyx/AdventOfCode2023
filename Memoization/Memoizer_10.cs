﻿using System;

namespace Memoization
{
	/// <summary>
	/// A memoized Func with 10 arguments
	/// </summary>
	public sealed class Memoizer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TOut> : Memoizer
	{
		private readonly Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TOut> func = null;

		internal Memoizer(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TOut> func)
		{
			this.func = func ?? throw new ArgumentNullException(nameof(func), "A Func delegate to memoize is required.");
		}

		public TOut Call(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
		{
			if (ArgsMatchPreviousCall(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10)) return PreviousResult<TOut>();

			TOut result = func(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);

			StoreArguments(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
			StoreResult(result);

			return result;
		}

		public static implicit operator Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TOut>(Memoizer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TOut> fn) => fn.Call;
	}
}
