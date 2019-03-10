using System;
using System.Collections.Generic;
using System.Text;

namespace PictureCrossSolver
{

	// this is bitwise, but the value for an undefined or "full" set of true and false is 3 because it contains true and false
	[Flags]
	public enum SolveBool
	{
		Impossible = 0,
		False = 1,
		True = 2,
		Undefined = 3, // special guy
	}

	// represents a set of booleans, which can take 4 states: true, false, unknown, and impossible
	// impossible means there was a failure to solve constraints
	public struct SolvingBooleanSet
	{
		public static SolvingBooleanSet Undefined = new SolvingBooleanSet(SolveBool.Undefined);
		public static SolvingBooleanSet TrueOnly = new SolvingBooleanSet(SolveBool.True);
		public static SolvingBooleanSet FalseOnly = new SolvingBooleanSet(SolveBool.False);
		public static SolvingBooleanSet Impossible = new SolvingBooleanSet(SolveBool.Impossible);

		public SolvingBooleanSet(SolveBool values)
		{
			this.values = values;
		}

		public SolvingBooleanSet(bool value)
		{
			this.values = value ? SolveBool.True : SolveBool.False;
		}

		SolveBool values;

		public SolveBool Values
		{
			get
			{
				return values;
			}
			set
			{
				values = value;
			}
		}

		public bool BoolValue
		{
			get
			{
				switch (values)
				{
					case SolveBool.False:
						return false;
					case SolveBool.True:
						return true;
					case SolveBool.Undefined:
					case SolveBool.Impossible:
						throw new InvalidCastException();
					default:
						throw new NotImplementedException();
				}
			}
		}

		public bool IsDefined
		{
			get
			{
				return values == SolveBool.True || values == SolveBool.False;
			}
		}

		public bool Invalid
		{
			get
			{
				return values == SolveBool.Impossible;
			}
		}

		public void Remove(SolveBool change)
		{
			if (values == SolveBool.Impossible) return;
			switch (values)
			{
				case SolveBool.True:
				case SolveBool.False:
				case SolveBool.Undefined:
					values = values & ~change;
					break;
				default:
					break;
			}
		}

		public void Add(SolveBool change)
		{
			values = values | change;
		}

		public void Add(SolvingBooleanSet s)
		{
			Add(s.values);
		}

		public void Remove(SolvingBooleanSet s)
		{
			Remove(s.values);
		}

		public void Remove(bool value)
		{
			Remove(value ? SolveBool.True : SolveBool.False);
		}

		public void Add(bool value)
		{
			Add(value ? SolveBool.True : SolveBool.False);
		}

		public override string ToString()
		{
			return values.ToString();
		}
	}
}
