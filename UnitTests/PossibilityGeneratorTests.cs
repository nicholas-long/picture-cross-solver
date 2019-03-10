using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using PictureCrossSolver;
using System.Collections.Immutable;
using System.Linq;

namespace UnitTests
{
	[Microsoft.VisualStudio.TestTools.UnitTesting.TestClass]
	public class PossibilityGeneratorTests
	{
		[TestMethod]
		public void TestPossibilities_4_5()
		{
			RowPossibilityGenerator gen = new RowPossibilityGenerator();
			var results = gen.Generate(new List<int>() { 4 }, 5);
			Assert.AreEqual(results.Count(), 2);
			Assert.IsTrue(results.All(r => r.Length == 5));
			Assert.IsTrue(results.All(r => r.ElementAt(1).BoolValue && r.ElementAt(2).BoolValue && r.ElementAt(3).BoolValue));
			var possibilities = gen.Intersection(new List<int>() { 4 }, 5);
			Assert.IsFalse(possibilities[0].IsDefined);
			Assert.IsTrue(possibilities[1].IsDefined);
			Assert.IsTrue(possibilities[2].IsDefined);
			Assert.IsTrue(possibilities[3].IsDefined);
			Assert.IsFalse(possibilities[4].IsDefined);
		}

		[TestMethod]
		public void TestPossibilities_9_15()
		{
			RowPossibilityGenerator gen = new RowPossibilityGenerator();
			var results = gen.Intersection(new List<int>() { 9 }, 15);

			Assert.IsTrue(results.Count(b => b.IsDefined) == 3);
			Assert.IsTrue(new List<int>() { 6, 7, 8 }.All(n => results[n].IsDefined));
		}

		[TestMethod]
		public void TestExperiment()
		{
			RowPossibilityGenerator gen = new RowPossibilityGenerator();
			var results = gen.Intersection(new List<int>() { 2, 9 }, 15);
			Assert.IsTrue(results.Length > 0);
		}

		[TestMethod]
		public void TestConstrain()
		{
			RowPossibilityGenerator gen = new RowPossibilityGenerator();
			var c = new TestCase()
			{
				Row = "00....111......",
				Target = "00....11111....",
				Groups = new List<int>() { 9 },
			};
			var result = gen.Constrain(c.Groups, c.BooleanRow);
			Assert.IsTrue(result[9].IsDefined);
			Assert.IsTrue(result.SequenceEqual(c.TargetBooleanRow));
		}

		#region Test case classes

		class TestCase
		{
			public string Row { get; set; }
			public List<int> Groups { get; set; }
			public string Target { get; set; }
			public int Length
			{
				get
				{
					return Row.Length;
				}
			}
			public SolvingBooleanSet[] BooleanRow
			{
				get
				{
					return ParseTestCase(Row);
				}
			}

			public SolvingBooleanSet[] TargetBooleanRow
			{
				get
				{
					return ParseTestCase(Target);
				}
			}

			private static SolvingBooleanSet ParseChar(char c)
			{
				switch (c)
				{
					case '.':
						return SolvingBooleanSet.Undefined;
					case '0':
						return SolvingBooleanSet.FalseOnly;
					case '1':
						return SolvingBooleanSet.TrueOnly;
					default:
						throw new ArgumentException(c.ToString());
				}
			}

			private static SolvingBooleanSet[] ParseTestCase(string input)
			{
				return input.Select(ParseChar).ToArray();
			}
		}


		#endregion
	}
}
