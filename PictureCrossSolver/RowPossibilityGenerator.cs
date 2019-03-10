using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Collections.Immutable;

namespace PictureCrossSolver
{
	public class RowPossibilityGenerator
	{
		public List<SolvingBooleanSet[]> Generate(IEnumerable<int> groupSizes, int rowLength)
		{
			// all of the groups plus at least one spacer between them should be less than the row length
			if (outOfBounds(groupSizes, rowLength))
			{
				throw new InvalidOperationException("Not enough space for this combination");
			}

			//
			List<SolvingBooleanSet[]> results = new List<SolvingBooleanSet[]>();
			generateRec(ImmutableQueue.CreateRange(groupSizes), ImmutableList<SolvingBooleanSet>.Empty, rowLength, results);
			return results;
		}

		public SolvingBooleanSet[] Intersection(IEnumerable<int> groupSizes, int rowLength)
		{
			IEnumerable<SolvingBooleanSet[]> results = Generate(groupSizes, rowLength);
			SolvingBooleanSet[] arr = Enumerable.Range(1, rowLength).Select(x => SolvingBooleanSet.Impossible).ToArray();
			intersect(results, arr);
			//foreach (var row in results)
			//{
			//	for (int index = 0; index < rowLength; index++)
			//	{
			//		arr[index].Add(row.ElementAt(index));
			//	}
			//}
			return arr;
		}

		//TODO: rewrite this.  this is way too inefficient.
		public SolvingBooleanSet[] Constrain(IEnumerable<int> groupSizes, SolvingBooleanSet[] initial)
		{
			var allResults = Generate(groupSizes, initial.Length); // take all the possibilities

			var filtered = allResults.Where(r => fits(r, initial)); // remove ones that don't fit with what we know

			// do intersection of filtered values
			SolvingBooleanSet[] ret = initial.ToList().Select(v => v.IsDefined ? v : SolvingBooleanSet.Impossible).ToArray();			intersect(filtered, ret);
			intersect(filtered, ret);

			return ret;
		}

		private bool fits(SolvingBooleanSet[] query, SolvingBooleanSet[] known)
		{
			for (int index = 0; index < known.Length; index++)
			{
				if (known[index].IsDefined && !query[index].IsDefined) return false;
				if (known[index].IsDefined && query[index].Values != known[index].Values) return false;
			}
			return true;
		}

		private void intersect(IEnumerable<SolvingBooleanSet[]> combinations, SolvingBooleanSet[] arr)
		{
			foreach (var row in combinations)
			{
				for (int index = 0; index < arr.Length; index++)
				{
					arr[index].Add(row.ElementAt(index));
				}
			}
		}

		private void generateRec(ImmutableQueue<int> groupSizes, ImmutableList<SolvingBooleanSet> formerProgress, int remaining, List<SolvingBooleanSet[]> output)
		{
			if (outOfBounds(groupSizes, remaining)) return;

			// base case: all groups are generated
			if (groupSizes.IsEmpty)
			{
				while (remaining > 0)
				{
					formerProgress = formerProgress.Add(SolvingBooleanSet.FalseOnly);
					remaining--;
				}
				output.Add(formerProgress.ToArray());
				return;
			}

			// 2 paths: take a group and append it, or don't take a group and append an empty box
			// recurse in both

			// take a group
			if (!groupSizes.IsEmpty)
			{
				int group = groupSizes.Peek();
				var nextGroupSizes = groupSizes.Dequeue();
				var nextProgress = formerProgress;
				for (int n = 0; n < group; n++) // add group
				{
					nextProgress = nextProgress.Add(SolvingBooleanSet.TrueOnly);
				}
				if (remaining - group > 0)
				{
					nextProgress = nextProgress.Add(SolvingBooleanSet.FalseOnly); // add spacer
					generateRec(nextGroupSizes, nextProgress, (remaining - group) - 1, output);
				}
				else
				{
					generateRec(nextGroupSizes, nextProgress, 0, output);
				}
			}

			// don't take a group and append an empty box
			var next = formerProgress.Add(SolvingBooleanSet.FalseOnly);
			generateRec(groupSizes, next, remaining - 1, output);
		}

		private bool outOfBounds(IEnumerable<int> groupSizes, int remainingLength)
		{
			return (groupSizes.Sum() + groupSizes.Count() - 1 > remainingLength);
		}
	}
}
