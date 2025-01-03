﻿using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using TriSpell.Source.DistanceCalculators;

namespace TriSpell.Source;

internal sealed class TriSpell {

    /// <summary>Represents an enumeration of all supported accuracies for spellchecking.</summary>
    private enum Accuracy { Low, Medium, High }

    /// <summary>Default foreground color of the application.</summary>
    private const ConsoleColor ForegroundColor = ConsoleColor.White;

    /// <summary>Default background color of the application.</summary>
    private const ConsoleColor BackgroundColor = ConsoleColor.Black;

    /// <summary>Default highlight color of the application.</summary>
    private const ConsoleColor HighlightColor = ConsoleColor.DarkCyan;

    /// <summary>Path of the dictionary file used for spellchecking.</summary>
    private static readonly string DictionaryPath = Path.Combine(
        AppContext.BaseDirectory,
        "Resources",
        "Dictionary",
        "Dictionary.txt"
    );

    /// <summary>Content of the dictionary file used for spellchecking.</summary>
    private static readonly FrozenSet<string> Words = File.ReadLines(DictionaryPath).ToFrozenSet();

    /// <summary>Array of all accuracies, cached for efficiency.</summary>
    private static readonly ImmutableArray<Accuracy> Accuracies = [.. Enum.GetValues<Accuracy>()];

    /// <summary>Description displayed for each <see cref="Accuracy"/>.</summary>
    private static readonly FrozenDictionary<Accuracy, string> AccuracyToDescription =
        FrozenDictionary.ToFrozenDictionary([
            KeyValuePair.Create(Accuracy.Low, "Low (Maximum Edit Distance = 3)"),
            KeyValuePair.Create(Accuracy.Medium, "Medium (Maximum Edit Distance = 2)"),
            KeyValuePair.Create(Accuracy.High, "High (Maximum Edit Distance = 1)")
        ]);

    /// <summary>Maximum allowed edit distance for each <see cref="Accuracy"/>.</summary>
    private static readonly FrozenDictionary<Accuracy, int> AccuracyToEditDistance =
        FrozenDictionary.ToFrozenDictionary([
            KeyValuePair.Create(Accuracy.Low, 3),
            KeyValuePair.Create(Accuracy.Medium, 2),
            KeyValuePair.Create(Accuracy.High, 1)
        ]);

    /// <summary>Foreground color for each <see cref="Accuracy"/>.</summary>
    private static readonly FrozenDictionary<Accuracy, ConsoleColor> AccuracyToForegroundColor =
        FrozenDictionary.ToFrozenDictionary([
            KeyValuePair.Create(Accuracy.Low, ConsoleColor.Red),
            KeyValuePair.Create(Accuracy.Medium, ConsoleColor.Yellow),
            KeyValuePair.Create(Accuracy.High, ConsoleColor.Green)
        ]);

    /// <summary>Array of all edit distance calculators, cached for efficiency.</summary>
    private static readonly ImmutableArray<IDistanceCalculator> DistanceCalculators = [
        RecursiveCalculator.Instance,
        IterativeFullMatrixCalculator.Instance,
        IterativeOptimizedMatrixCalculator.Instance
    ];

    /// <summary>Description displayed for each edit distance calculator.</summary>
    private static readonly FrozenDictionary<IDistanceCalculator, string> DistanceCalculatorToDescription =
        FrozenDictionary.ToFrozenDictionary([
            KeyValuePair.Create<IDistanceCalculator, string>(
                RecursiveCalculator.Instance,
                "Slow (Recursive)"
            ),
            KeyValuePair.Create<IDistanceCalculator, string>(
                IterativeFullMatrixCalculator.Instance,
                "Fast (Iterative Full Matrix)"
            ),
            KeyValuePair.Create<IDistanceCalculator, string>(
                IterativeOptimizedMatrixCalculator.Instance,
                "Very Fast (Iterative Optimized Matrix)"
            )
        ]);

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
        ConsoleColor oldForegroundColor = Console.ForegroundColor;
        Console.ForegroundColor = foregroundColor;
        Console.Write(text);
        Console.ForegroundColor = oldForegroundColor;
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
        ConsoleColor oldForegroundColor = Console.ForegroundColor;
        Console.ForegroundColor = foregroundColor;
        Console.WriteLine(text);
        Console.ForegroundColor = oldForegroundColor;
    }

    /// <summary>
    /// Draws the main menu including the currently selected <see cref="IDistanceCalculator"/> and
    /// <see cref="Accuracy"/>.
    /// </summary>
    /// <param name="distanceCalculator">Current <see cref="IDistanceCalculator"/>.</param>
    /// <param name="accuracy">Current <see cref="Accuracy"/>.</param>
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
        WriteLineColored(
            ", a small and simple console application for basic spellchecking.\n",
            ForegroundColor
        );
        WriteLineColored("Settings", ForegroundColor);
        string distanceCalculatorDescription = DistanceCalculatorToDescription[distanceCalculator];
        string accuracyDescription = AccuracyToDescription[accuracy];
        int tableWidth = "Algorithm | ".Length
            + Math.Max(distanceCalculatorDescription.Length, accuracyDescription.Length);
        WriteLineColored(new string('-', tableWidth), ForegroundColor);
        WriteLineColored($"Algorithm | {distanceCalculatorDescription}", ForegroundColor);
        WriteLineColored($"Accuracy  | {accuracyDescription}\n", ForegroundColor);
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
                case ConsoleKey.Escape:
                    selectedOption = default;
                    return false;
                case ConsoleKey.UpArrow:
                    index = Math.Max(index - 1, 0);
                    break;
                case ConsoleKey.DownArrow:
                    index = Math.Min(index + 1, options.Count - 1);
                    break;
                case ConsoleKey.Enter:
                    selectedOption = options[index];
                    return true;
            }
        }
    }

    /// <summary>Performs spellchecking on a word entered by the user.</summary>
    /// <param name="distanceCalculator">Current <see cref="IDistanceCalculator"/>.</param>
    /// <param name="accuracy">Current <see cref="Accuracy"/>.</param>
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
        if (Words.Contains(source)) {
            WriteLineColored(" is spelled correctly.", ForegroundColor);
        }
        else {
            IReadOnlyList<(string Target, int Distance)> possibleMatches = [.. Words
                .Select(target => (
                    Target: target,
                    EditDistance: distanceCalculator.Distance(source, target)
                ))
                .Where(pair => pair.EditDistance <= AccuracyToEditDistance[accuracy])
                .OrderBy(pair => pair.EditDistance)
                .ThenBy(pair => pair.Target)
            ];
            if (possibleMatches.Count == 0) {
                WriteLineColored(
                    " might be misspelled (no possible matches were found).",
                    ForegroundColor
                );
            }
            else {
                WriteLineColored(
                    " might be misspelled, the following possible matches were found.\n",
                    ForegroundColor
                );
                int maxTargetLength = possibleMatches.Max(match => match.Target.Length);
                string header = "Possible Match".PadRight(maxTargetLength) + " | Edit Distance";
                WriteLineColored(header, ForegroundColor);
                WriteLineColored(new string('-', header.Length), ForegroundColor);
                foreach ((string target, int editDistance) in possibleMatches) {
                    int paddingLength = Math.Max(maxTargetLength, "Possible Match".Length);
                    WriteColored($"{target.PadRight(paddingLength)} | ", ForegroundColor);
                    Accuracy targetAccuracy = AccuracyToEditDistance
                        .Last(pair => editDistance <= pair.Value).Key;
                    WriteLineColored(
                        $"{editDistance}",
                        AccuracyToForegroundColor[targetAccuracy]
                    );
                }
            }
        }
        WriteLineColored("\nPress any key to continue.", ForegroundColor);
        Console.ReadKey(true);
    }

    private static void Main() {
        ConsoleColor oldForegroundColor = Console.ForegroundColor;
        ConsoleColor oldBackgroundColor = Console.BackgroundColor;
        Console.CursorVisible = false;
        Console.ForegroundColor = ForegroundColor;
        Console.BackgroundColor = BackgroundColor;
        IDistanceCalculator distanceCalculator = IterativeOptimizedMatrixCalculator.Instance;
        Accuracy accuracy = Accuracy.Medium;
        while (true) {
            DrawMainMenu(distanceCalculator, accuracy);
            switch (Console.ReadKey(true).Key) {
                case ConsoleKey.Escape:
                    Console.CursorVisible = true;
                    Console.ForegroundColor = oldForegroundColor;
                    Console.BackgroundColor = oldBackgroundColor;
                    return;
                case ConsoleKey.D1:
                    if (TrySelectOption(
                            "Select one of the following algorithms.",
                            DistanceCalculators,
                            DistanceCalculatorToDescription,
                            out IDistanceCalculator? selectedDistanceCalculator
                        )) {
                        distanceCalculator = selectedDistanceCalculator;
                    }
                    break;
                case ConsoleKey.D2:
                    if (TrySelectOption(
                            "Select one of the following accuracies.",
                            Accuracies,
                            AccuracyToDescription,
                            out Accuracy selectedAccuracy
                        )) {
                        accuracy = selectedAccuracy;
                    }
                    break;
                case ConsoleKey.D3:
                    SpellcheckWord(distanceCalculator, accuracy);
                    break;
            }
        }
    }

}