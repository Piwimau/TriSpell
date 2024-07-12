using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;

using TriSpell.Source.DistanceCalculators;

namespace TriSpell.Source;

internal sealed class Program {

    /// <summary>Represents an enumeration of all available accuracies.</summary>
    private enum Accuracy { Low, Medium, High }

    /// <summary>Default foreground color of the console application.</summary>
    private const ConsoleColor ForegroundColor = ConsoleColor.White;

    /// <summary>Default background color of the console application.</summary>
    private const ConsoleColor BackgroundColor = ConsoleColor.Black;

    /// <summary>Default highlight color of the console application.</summary>
    private const ConsoleColor HighlightColor = ConsoleColor.DarkCyan;

    /// <summary>Path of the dictionary file used for spellchecking.</summary>
    private static readonly string DictionaryPath = Path.Combine(
        AppContext.BaseDirectory,
        "Resources",
        "Dictionary",
        "Dictionary.txt"
    );

    /// <summary>Content of the dictionary file used for spellchecking.</summary>
    private static readonly IReadOnlyList<string> Words = [.. File.ReadLines(DictionaryPath)];

    /// <summary>List of all accuracies, cached for efficiency.</summary>
    private static readonly IReadOnlyList<Accuracy> Accuracies = [.. Enum.GetValues<Accuracy>()];

    /// <summary>Descriptions displayed for each accuracy.</summary>
    private static readonly IReadOnlyDictionary<Accuracy, string> DescriptionByAccuracy =
        new Dictionary<Accuracy, string>() {
            [Accuracy.Low] = "Low (Maximum Edit Distance = 3 Characters)",
            [Accuracy.Medium] = "Medium (Maximum Edit Distance = 2 Characters)",
            [Accuracy.High] = "High (Maximum Edit Distance = 1 Character)",
        };

    /// <summary>Maximum allowed edit distance for each accuracy.</summary>
    private static readonly IReadOnlyDictionary<Accuracy, int> MaximumEditDistanceByAccuracy =
        new Dictionary<Accuracy, int>() {
            [Accuracy.Low] = 3,
            [Accuracy.Medium] = 2,
            [Accuracy.High] = 1
        };

    /// <summary>List of all edit distance calculation algorithms, cached for efficiency.</summary>
    private static readonly IReadOnlyList<IDistanceCalculator> DistanceCalculators = [
        RecursiveCalculator.Instance,
        IterativeFullMatrixCalculator.Instance,
        IterativeOptimizedMatrixCalculator.Instance
    ];

    /// <summary>Descriptions displayed for each edit distance calculation algorithm.</summary>
    private static readonly IReadOnlyDictionary<IDistanceCalculator, string>
        DescriptionByDistanceCalculator = new Dictionary<IDistanceCalculator, string>() {
            [RecursiveCalculator.Instance] = "Recursive (Slow)",
            [IterativeFullMatrixCalculator.Instance] = "Iterative Full Matrix (Medium)",
            [IterativeOptimizedMatrixCalculator.Instance] = "Iterative Optimized Matrix (Fast)"
        };

    /// <summary>
    /// Writes a given text with a custom foreground color to the standard output stream.
    /// </summary>
    /// <remarks>
    /// The previous foreground color of the <see cref="Console"/> is saved and restored using the
    /// <see cref="Console.ForegroundColor"/> property before this call returns.
    /// </remarks>
    /// <param name="text">Text to write to the standard output stream.</param>
    /// <param name="foregroundColor">
    /// Custom <see cref="ConsoleColor"/> to use as the foreground color.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="text"/> is <see langword="null"/>.
    /// </exception>
    private static void WriteColored(string text, ConsoleColor foregroundColor) {
        ArgumentNullException.ThrowIfNull(text, nameof(text));
        (foregroundColor, Console.ForegroundColor) = (Console.ForegroundColor, foregroundColor);
        Console.Write(text);
        Console.ForegroundColor = foregroundColor;
    }

    /// <summary>
    /// Writes a given text with a custom foreground color, followed by the current line terminator,
    /// to the standard output stream.
    /// </summary>
    /// <remarks>
    /// The previous foreground color of the <see cref="Console"/> is saved and restored using the
    /// <see cref="Console.ForegroundColor"/> property before this call returns.
    /// </remarks>
    /// <param name="text">Text to write to the standard output stream.</param>
    /// <param name="foregroundColor">
    /// Custom <see cref="ConsoleColor"/> to use as the foreground color.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="text"/> is <see langword="null"/>.
    /// </exception>
    private static void WriteLineColored(string text, ConsoleColor foregroundColor) {
        ArgumentNullException.ThrowIfNull(text, nameof(text));
        (foregroundColor, Console.ForegroundColor) = (Console.ForegroundColor, foregroundColor);
        Console.WriteLine(text);
        Console.ForegroundColor = foregroundColor;
    }

    /// <summary>
    /// Draws the main menu including the currently selected <see cref="IDistanceCalculator"/> and
    /// <see cref="Accuracy"/>.
    /// </summary>
    /// <param name="distanceCalculator">
    /// Currently selected <see cref="IDistanceCalculator"/>.
    /// </param>
    /// <param name="accuracy">Currently selected <see cref="Accuracy"/>.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="distanceCalculator"/> is <see langword="null"/>.
    /// </exception>
    private static void DrawMainMenu(IDistanceCalculator distanceCalculator, Accuracy accuracy) {
        ArgumentNullException.ThrowIfNull(distanceCalculator, nameof(distanceCalculator));
        Console.Clear();
        WriteLineColored(
           """
            ▄▄▄█████▓ ██▀███   ██▓  ██████  ██▓███  ▓█████  ██▓     ██▓    
            ▓  ██▒ ▓▒▓██ ▒ ██▒▓██▒▒██    ▒ ▓██░  ██▒▓█   ▀ ▓██▒    ▓██▒    
            ▒ ▓██░ ▒░▓██ ░▄█ ▒▒██▒░ ▓██▄   ▓██░ ██▓▒▒███   ▒██░    ▒██░    
            ░ ▓██▓ ░ ▒██▀▀█▄  ░██░  ▒   ██▒▒██▄█▓▒ ▒▒▓█  ▄ ▒██░    ▒██░    
              ▒██▒ ░ ░██▓ ▒██▒░██░▒██████▒▒▒██▒ ░  ░░▒████▒░██████▒░██████▒
              ▒ ░░   ░ ▒▓ ░▒▓░░▓  ▒ ▒▓▒ ▒ ░▒▓▒░ ░  ░░░ ▒░ ░░ ▒░▓  ░░ ▒░▓  ░
                ░      ░▒ ░ ▒░ ▒ ░░ ░▒  ░ ░░▒ ░      ░ ░  ░░ ░ ▒  ░░ ░ ▒  ░
              ░        ░░   ░  ▒ ░░  ░  ░  ░░          ░     ░ ░     ░ ░   
                        ░      ░        ░              ░  ░    ░  ░    ░  ░

            """,
           HighlightColor
        );
        WriteColored("Welcome to ", ForegroundColor);
        WriteColored("TriSpell", HighlightColor);
        WriteLineColored(", a small and simple console spellchecker.\n", ForegroundColor);
        WriteLineColored("Current Settings", ForegroundColor);
        string distanceCalculatorDescription = DescriptionByDistanceCalculator[distanceCalculator];
        string accuracyDescription = DescriptionByAccuracy[accuracy];
        int tableWidth = "Algorithm | ".Length
            + Math.Max(distanceCalculatorDescription.Length, accuracyDescription.Length);
        WriteLineColored(new string('─', tableWidth), ForegroundColor);
        WriteLineColored($"Algorithm │ {distanceCalculatorDescription}", ForegroundColor);
        WriteLineColored($"Accuracy  │ {accuracyDescription}\n", ForegroundColor);
        WriteColored("[ESC] ", HighlightColor);
        WriteLineColored("Exit Program", ForegroundColor);
        WriteColored("[1]   ", HighlightColor);
        WriteLineColored("Select Algorithm", ForegroundColor);
        WriteColored("[2]   ", HighlightColor);
        WriteLineColored("Select Accuracy", ForegroundColor);
        WriteColored("[3]   ", HighlightColor);
        WriteLineColored("Spellcheck A Word", ForegroundColor);
    }

    /// <summary>Tries to let the user select one among a list of options.</summary>
    /// <typeparam name="T">Type of the options to select one of.</typeparam>
    /// <param name="prompt">Prompt text shown to the user.</param>
    /// <param name="options">List of all available options.</param>
    /// <param name="optionDescriptions">Descriptions for all options shown to the user.</param>
    /// <param name="selectedOption">
    /// Option selected by the user or <see langword="null"/> if the selection was cancelled.
    /// </param>
    /// <returns>
    /// <see langword="True"/> if an option was selected by the user,
    /// otherwise <see langword="false"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="prompt"/>, <paramref name="options"/> or
    /// <paramref name="optionDescriptions"/> is <see langword="null"/>.
    /// </exception>
    private static bool TrySelectOption<T>(
        string prompt,
        IReadOnlyList<T> options,
        IReadOnlyDictionary<T, string> optionDescriptions,
        [NotNullWhen(true)] out T? selectedOption
    ) where T : notnull {
        ArgumentNullException.ThrowIfNull(prompt, nameof(prompt));
        ArgumentNullException.ThrowIfNull(options, nameof(options));
        ArgumentNullException.ThrowIfNull(optionDescriptions, nameof(optionDescriptions));
        Console.Clear();
        int index = 0;
        while (true) {
            Console.SetCursorPosition(0, 0);
            WriteLineColored($"{prompt}\n", ForegroundColor);
            for (int i = 0; i < options.Count; i++) {
                WriteColored("[", ForegroundColor);
                if (i == index) {
                    WriteColored("*", HighlightColor);
                }
                else {
                    WriteColored(" ", ForegroundColor);
                }
                WriteColored("] ", ForegroundColor);
                WriteLineColored(optionDescriptions[options[i]], ForegroundColor);
            }
            WriteColored("\n[ESC]   ", HighlightColor);
            WriteLineColored("Discard Changes", ForegroundColor);
            WriteColored("[^][v]  ", HighlightColor);
            WriteLineColored("Change Selected Option", ForegroundColor);
            WriteColored("[ENTER] ", HighlightColor);
            WriteLineColored("Confirm Selected Option", ForegroundColor);
            switch (Console.ReadKey(true).Key) {
                case ConsoleKey.UpArrow:
                    if (index > 0) {
                        index--;
                    }
                    break;
                case ConsoleKey.DownArrow:
                    if (index < (options.Count - 1)) {
                        index++;
                    }
                    break;
                case ConsoleKey.Enter:
                    selectedOption = options[index];
                    return true;
                case ConsoleKey.Escape:
                    selectedOption = default;
                    return false;
            }
        }
    }

    /// <summary>Performs spellchecking on a word entered by the user.</summary>
    /// <param name="distanceCalculator">
    /// Currently selected <see cref="IDistanceCalculator"/>.
    /// </param>
    /// <param name="accuracy">Currently selected <see cref="Accuracy"/>.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="distanceCalculator"/> is <see langword="null"/>.
    /// </exception>
    private static void SpellcheckWord(IDistanceCalculator distanceCalculator, Accuracy accuracy) {
        ArgumentNullException.ThrowIfNull(distanceCalculator, nameof(distanceCalculator));
        WriteLineColored("\nEnter a word to spellcheck.\n", ForegroundColor);
        WriteColored("> ", HighlightColor);
        Console.CursorVisible = true;
        string source = Console.ReadLine() ?? "";
        Console.CursorVisible = false;
        WriteColored("\nThe word ", ForegroundColor);
        WriteColored(source, HighlightColor);
        IReadOnlyList<(string Target, int Distance)> possibleMatches = [.. Words
            .Select(target => (
                Target: target,
                EditDistance: distanceCalculator.Distance(source, target)
            ))
            .Where(pair => pair.EditDistance <= MaximumEditDistanceByAccuracy[accuracy])
            .OrderBy(pair => pair.EditDistance)
        ];
        if (possibleMatches.Count == 0) {
            WriteLineColored(
                " might be misspelled (no possible matches were found).",
                ForegroundColor
            );
        }
        else if (possibleMatches[0].Distance == 0) {
            WriteLineColored(" is spelled correctly.", ForegroundColor);
        }
        else {
            WriteLineColored(
                " might be misspelled, the following possible matches were found.",
                ForegroundColor
            );
            int maxTargetLength = possibleMatches.Max(match => match.Target.Length);
            string header = "\nPossible Match".PadRight(maxTargetLength) + " │ Distance";
            WriteLineColored(header, ForegroundColor);
            WriteLineColored(new string('─', header.Length), ForegroundColor);
            foreach ((string target, int editDistance) in possibleMatches) {
                int paddingLength = Math.Max(maxTargetLength, "Possible Match".Length);
                WriteColored($"{target.PadRight(paddingLength)} │ ", ForegroundColor);
                Accuracy targetAccuracy = MaximumEditDistanceByAccuracy
                    .Last(pair => editDistance <= pair.Value).Key;
                ConsoleColor foregroundColor = targetAccuracy switch {
                    Accuracy.Low => ConsoleColor.Red,
                    Accuracy.Medium => ConsoleColor.Yellow,
                    Accuracy.High => ConsoleColor.Green,
                    _ => throw new InvalidOperationException("Unreachable.")
                };
                WriteLineColored(editDistance.ToString(), foregroundColor);
            }
        }
        WriteLineColored("\nPress any key to continue.", ForegroundColor);
        Console.ReadKey(true);
    }

    private static void Main() {
        // Save current fore- and background color to restore them at the end.
        ConsoleColor currentForegroundColor = Console.ForegroundColor;
        ConsoleColor currentBackgroundColor = Console.BackgroundColor;
        // Cursor is only shown when the user is actually supposed to type something.
        Console.CursorVisible = false;
        Console.ForegroundColor = ForegroundColor;
        Console.BackgroundColor = BackgroundColor;
        IDistanceCalculator distanceCalculator = IterativeOptimizedMatrixCalculator.Instance;
        Accuracy accuracy = Accuracy.Medium;
        ConsoleKey consoleKey;
        do {
            DrawMainMenu(distanceCalculator, accuracy);
            consoleKey = Console.ReadKey(true).Key;
            switch (consoleKey) {
                case ConsoleKey.D1:
                    if (TrySelectOption(
                            "Select one of the following algorithms.",
                            DistanceCalculators,
                            DescriptionByDistanceCalculator,
                            out IDistanceCalculator? selectedDistanceCalculator)
                        ) {
                        distanceCalculator = selectedDistanceCalculator;
                    }
                    break;
                case ConsoleKey.D2:
                    if (TrySelectOption(
                            "Select one of the following accuracies.",
                            Accuracies,
                            DescriptionByAccuracy,
                            out Accuracy selectedAccuracy)
                        ) {
                        accuracy = selectedAccuracy;
                    }
                    break;
                case ConsoleKey.D3:
                    SpellcheckWord(distanceCalculator, accuracy);
                    break;
            }
        }
        while (consoleKey != ConsoleKey.Escape);
        Console.CursorVisible = true;
        Console.ForegroundColor = currentForegroundColor;
        Console.BackgroundColor = currentBackgroundColor;
    }

}