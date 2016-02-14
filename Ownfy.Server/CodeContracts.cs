// <copyright company="Skivent Ltda.">
// Copyright (c) 2013, All Right Reserved, http://www.skivent.com.co/
// </copyright>

namespace Ownfy.Server
{
	using System;
	using System.Diagnostics;
	using System.Linq;

	/// <summary>
	/// Allows to make explicit assertions about program preconditions
	/// </summary>
	public static class CodeContracts
	{
		/// <summary>
		/// Assert the specified condition.
		/// </summary>
		/// <param name="cond">Write in this parameter the condition to assert.</param>
		/// <param name="msg">The the semantic of not asserts.</param>
		/// <exception cref="System.ArgumentException">Thrown when assert failed</exception>
		[Conditional("DEBUG")]
		[Conditional("CODE_CONTRACTS")]
		public static void Requires(bool cond, string msg = "")
		{
			if (!cond)
			{
				throw new ArgumentException(msg);
			}
		}

		/// <summary>
		/// Assert that specified object are not null.
		/// </summary>
		/// <param name="o">The objects that should not be null.</param>
		/// <exception cref="System.ArgumentNullException">Thrown when assert failed</exception>
		[Conditional("DEBUG")]
		[Conditional("CODE_CONTRACTS")]
		public static void RequiresNotNull(params object[] o)
		{
			if (o.Any(x => x == null))
			{
				throw new ArgumentNullException();
			}
		}
	}
}