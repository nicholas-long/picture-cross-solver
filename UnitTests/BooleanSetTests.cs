using Microsoft.VisualStudio.TestTools.UnitTesting;
using PictureCrossSolver;

namespace UnitTests
{
	[TestClass]
	public class BooleanSetTests
	{
		[TestMethod]
		public void TestConstructAndEquality()
		{
			SolvingBooleanSet s = SolvingBooleanSet.Undefined;
			Assert.IsTrue(s.Values == SolveBool.Undefined);
			SolvingBooleanSet b = new SolvingBooleanSet(SolveBool.Undefined);
			Assert.AreEqual(s, b);
		}

		[TestMethod]
		public void TestUniqueness()
		{
			SolvingBooleanSet s = SolvingBooleanSet.Undefined;
			var a = s;
			a.Values = SolveBool.True;
			Assert.AreNotEqual(a, s);
		}

		[TestMethod]
		public void TestLogic()
		{
			SolvingBooleanSet s = SolvingBooleanSet.Undefined;
			s.Remove(false);
			Assert.AreEqual(s, new SolvingBooleanSet(SolveBool.True));
			s.Remove(true);
			Assert.AreEqual(s, new SolvingBooleanSet(SolveBool.Impossible));
			s.Add(false);
			s.Add(true);
			Assert.AreEqual(s, SolvingBooleanSet.Undefined);
			s.Remove(SolvingBooleanSet.Undefined);
			Assert.AreEqual(s, SolvingBooleanSet.Impossible);

			s = SolvingBooleanSet.TrueOnly;
			s.Remove(false);
			Assert.AreEqual(s, new SolvingBooleanSet(SolveBool.True));
			s.Remove(SolveBool.Undefined);
			Assert.IsTrue(s.Invalid);
		}
	}
}
