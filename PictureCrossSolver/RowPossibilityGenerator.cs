using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Collections.Immutable;

namespace PictureCrossSolver
{
	public class RowPossibilityGenerator
	{
		public List<SolvingBooleanSet[]> GenerateAll(IEnumerable<int> groupSizes, int rowLength)
		{
			if (OutOfBounds(groupSizes, rowLength))
			{
				throw new InvalidOperationException("Not enough space for this combination");
			}

			List<SolvingBooleanSet[]> results = new List<SolvingBooleanSet[]>();
			GenerateRec(ImmutableQueue.CreateRange(groupSizes), ImmutableList<SolvingBooleanSet>.Empty, rowLength, results);
			return results;
		}

		public List<SolvingBooleanSet[]> Generate(IEnumerable<int> groupSizes, SolvingBooleanSet[] knownData)
		{
			if (OutOfBounds(groupSizes, knownData.Length))
			{
				throw new InvalidOperationException("Not enough space for this combination");
			}

			List<SolvingBooleanSet[]> results = new List<SolvingBooleanSet[]>();
			GenerateIncludingKnownDataRec(knownData, ImmutableQueue.CreateRange(groupSizes), ImmutableList<SolvingBooleanSet>.Empty, 0, results);
			return results;
		}

		public SolvingBooleanSet[] Intersection(IEnumerable<int> groupSizes, int rowLength)
		{
			IEnumerable<SolvingBooleanSet[]> results = GenerateAll(groupSizes, rowLength);
			SolvingBooleanSet[] arr = Enumerable.Range(1, rowLength).Select(x => SolvingBooleanSet.Impossible).ToArray();
			Intersect(results, arr);
			return arr;
		}

		public SolvingBooleanSet[] Constrain(IEnumerable<int> groupSizes, SolvingBooleanSet[] initial)
		{
			var allResults = Generate(groupSizes, initial);
			SolvingBooleanSet[] ret = initial.ToList().Select(v => v.IsDefined ? v : SolvingBooleanSet.Impossible).ToArray();
			Intersect(allResults, ret);
			return ret;
		}

		private void Intersect(IEnumerable<SolvingBooleanSet[]> combinations, SolvingBooleanSet[] arr)
		{
			foreach (var row in combinations)
			{
				for (int index = 0; index < arr.Length; index++)
				{
					arr[index].Add(row.ElementAt(index));
				}
			}
		}

		private void GenerateRec(ImmutableQueue<int> groupSizes, ImmutableList<SolvingBooleanSet> formerProgress, int remaining, List<SolvingBooleanSet[]> output)
		{
			if (OutOfBounds(groupSizes, remaining)) return;

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
					GenerateRec(nextGroupSizes, nextProgress, (remaining - group) - 1, output);
				}
				else
				{
					GenerateRec(nextGroupSizes, nextProgress, 0, output);
				}
			}

			// don't take a group and append an empty box
			var next = formerProgress.Add(SolvingBooleanSet.FalseOnly);
			GenerateRec(groupSizes, next, remaining - 1, output);
		}

		private void GenerateIncludingKnownDataRec(SolvingBooleanSet[] known, ImmutableQueue<int> groupSizes, ImmutableList<SolvingBooleanSet> formerProgress, int index, List<SolvingBooleanSet[]> output)
		{
			if (OutOfBounds(groupSizes, known.Length - index)) return;
			if (index > known.Length) throw new InvalidOperationException();

			// base case - all groups are done so finish off the rest of the row with empty cells
			if (groupSizes.IsEmpty)
			{
				while (index < known.Length)
				{
					if (known[index].Values != SolveBool.True)
					{
						formerProgress = formerProgress.Add(SolvingBooleanSet.FalseOnly);
						index++;
					}
					else
					{
						// if we found a cell that can only be true, but all groups are gone, then this failed
						return; // break early
					}
				}
				output.Add(formerProgress.ToArray());
				return;
			}

			// take two paths: add a group and don't add a group, but fail if a contradictory cell is found
			// path 1: add a group if there is not a cell known to be empty
			if (known[index].Values != SolveBool.False)
			{
				int group = groupSizes.Peek();
				var nextGroupSizes = groupSizes.Dequeue();
				int nextIndex = index, end = index + group;
				var nextProgress = formerProgress;
				bool canAdd = true;
				while (nextIndex < end)
				{
					if (known[nextIndex].Values == SolveBool.False)
					{
						canAdd = false;
						break;
					}
					else
					{
						nextProgress = nextProgress.Add(SolvingBooleanSet.TrueOnly);
						nextIndex++;
					}
				}
				if (canAdd)
				{
					if (nextIndex < known.Length && known[nextIndex].Values != SolveBool.True)
					{
						nextProgress = nextProgress.Add(SolvingBooleanSet.FalseOnly);
						nextIndex++;
					}
					GenerateIncludingKnownDataRec(known, nextGroupSizes, nextProgress, nextIndex, output);
				}
			}

			// path 2: try adding an empty cell if possible
			if (known[index].Values != SolveBool.True)
			{
				GenerateIncludingKnownDataRec(known, groupSizes, formerProgress.Add(SolvingBooleanSet.FalseOnly), index + 1, output);
			}
		}

		private bool OutOfBounds(IEnumerable<int> groupSizes, int remainingLength)
		{
			// all of the groups plus at least one spacer between them should be less than the row length
			return (groupSizes.Sum() + groupSizes.Count() - 1 > remainingLength);
		}
	}
}
