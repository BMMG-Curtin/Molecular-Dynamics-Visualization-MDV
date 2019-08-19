﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CurtinUniversity.MolecularDynamics.Model {

    // Algebraic!
    public abstract class TypeMath<T> : ITypeMath<T>
	{
		private static Dictionary<Type, object> registeredMath = new Dictionary<Type, object>();

		public static void Register<DataType>(ITypeMath<DataType> math)
		{
			registeredMath.Add(typeof(DataType), math);
		}

		public static ITypeMath<T> GetMath()
		{
			Type type = typeof(T);
			object math;

			if (registeredMath.TryGetValue(type, out math))
			{
				return (ITypeMath<T>)math;
			}
			else
			{
				throw new Exception("No ITypeMath registered for type \"" + type.Name + "\"");
			}
		}

		#region ITypeMath<T> members

		public abstract int Compare(T a, T b);

		public abstract bool AreEqual(T a, T b);

		public virtual bool AreEqual(T[] a, T[] b)
		{
			if (a.Length != b.Length)
				return false;

			for (var index = 0; index < a.Length; index++)
			{
				if (!AreEqual(a[index], b[index]))
					return false;
			}

			return true;
		}

		public abstract T MinValue { get; }

		public abstract T MaxValue { get; }

		public T Min(T a, T b)
		{
			if (Compare(a, b) < 0)
				return a;
			else
				return b;
		}

		public T Max(T a, T b)
		{
			if (Compare(a, b) > 0)
				return a;
			else
				return b;
		}

		public abstract T Zero { get; }

		public abstract T NegativeInfinity { get; }

		public abstract T PositiveInfinity { get; }

		public abstract T Add(T a, T b);

		public abstract T Subtract(T a, T b);

		public abstract T Multiply(T a, T b);

		#endregion
	}
}
