using System;
using TriSpell.EditDistanceCalculators;

namespace TriSpell.Extensions;

/// <summary>
/// Contains useful extensions for an <see cref="IEditDistanceCalculator"/>.
/// </summary>
internal static class EditDistanceCalculatorExtensions {

    extension(IEditDistanceCalculator calculator) {

        /// <summary>
        /// Gets a short description for this
        /// <see cref="IEditDistanceCalculator"/>.
        /// </summary>
        public string Description => calculator switch {
            RecursiveEditDistanceCalculator => "Recursive (Slow)",
            IterativeFullMatrixEditDistanceCalculator
                => "Iterative Full Matrix (Medium)",
            IterativeOptimizedMatrixEditDistanceCalculator
                => "Iterative Optimized Matrix (Fast)",
            _ => throw new InvalidOperationException("Unreachable.")
        };

    }

}