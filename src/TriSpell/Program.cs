using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using CommunityToolkit.Diagnostics;
using TriSpell.EditDistanceCalculators;
using TriSpell.Extensions;

namespace TriSpell;

internal sealed class Program {

    /// <summary>The default foreground color of the console output.</summary>
    private const ConsoleColor ForegroundColor = ConsoleColor.White;

    /// <summary>The default background color of the console output.</summary>
    private const ConsoleColor BackgroundColor = ConsoleColor.Black;

    /// <summary>The default accent color of the console output.</summary>
    private const ConsoleColor AccentColor = ConsoleColor.DarkCyan;

    /// <summary>
    /// The path to the dictionary file containing a set of known words.
    /// </summary>
    private static readonly string DictionaryPath = Path.Combine(
        AppContext.BaseDirectory,
        "Resources",
        "Dictionary.txt"
    );

    /// <summary>Draws the main menu of the application.</summary>
    /// <param name="calculator">
    /// The current <see cref="IEditDistanceCalculator"/>.
    /// </param>
    /// <param name="accuracy">The current <see cref="Accuracy"/> level.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="calculator"/> is <see langword="null"/>.
    /// </exception>
    private static void DrawMainMenu(
        IEditDistanceCalculator calculator,
        Accuracy accuracy
    ) {
        Guard.IsNotNull(calculator);
        Console.Clear();
        Console.WriteLineColored(
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
            AccentColor
        );
        Console.WriteColored("Welcome to ", ForegroundColor);
        Console.WriteColored("TriSpell", AccentColor);
        Console.WriteLineColored(
            ", a small and simple console application for spell checking.",
            ForegroundColor
        );
        Console.WriteLine();
        Console.WriteLineColored("Settings", ForegroundColor);
        int tableWidth = "Algorithm | ".Length + Math.Max(
            calculator.Description.Length,
            accuracy.Description.Length
        );
        Console.WriteLineColored(new string('-', tableWidth), ForegroundColor);
        Console.WriteLineColored(
            $"Algorithm | {calculator.Description}",
            ForegroundColor
        );
        Console.WriteLineColored(
            $"Accuracy  | {accuracy.Description}",
            ForegroundColor
        );
        Console.WriteLine();
        Console.WriteColored("[ESC] ", AccentColor);
        Console.WriteLineColored("Exit Program", ForegroundColor);
        Console.WriteColored("[1]   ", AccentColor);
        Console.WriteLineColored("Select Algorithm", ForegroundColor);
        Console.WriteColored("[2]   ", AccentColor);
        Console.WriteLineColored("Select Accuracy", ForegroundColor);
        Console.WriteColored("[3]   ", AccentColor);
        Console.WriteLineColored("Spellcheck Word", ForegroundColor);
    }

    /// <summary>
    /// Displays a prompt and allows the user to select an option from a list of
    /// choices.
    /// </summary>
    /// <typeparam name="TOption">
    /// The type of the options in the list.
    /// </typeparam>
    /// <param name="prompt">
    /// The prompt displayed to the user to describe the context.
    /// </param>
    /// <param name="options">
    /// A read-only list of options from which the user can select.
    /// </param>
    /// <param name="defaultOption">The option selected by default.</param>
    /// <param name="optionDescription">
    /// A function that returns a short description for each option.
    /// </param>
    /// <param name="selectedOption">
    /// The selected option if the user confirms the selection, otherwise
    /// <see langword="null"/>.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the user confirms the selection, otherwise
    /// <see langword="false"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="prompt"/>, <paramref name="options"/> or
    /// <paramref name="optionDescription"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="defaultOption"/> is not part of
    /// <paramref name="options"/>.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="options"/> contains less than two elements.
    /// </exception>
    private static bool TrySelectOption<TOption>(
        string prompt,
        IReadOnlyList<TOption> options,
        TOption defaultOption,
        Func<TOption, string> optionDescription,
        [NotNullWhen(true)] out TOption? selectedOption
    ) where TOption : notnull {
        Guard.IsNotNull(prompt);
        Guard.IsNotNull(options);
        Guard.IsTrue(
            options.Contains(defaultOption),
            nameof(defaultOption),
            "The default option must be one of the available options."
        );
        Guard.HasSizeGreaterThanOrEqualTo(options, 2);
        Guard.IsNotNull(optionDescription);
        Console.Clear();
        int selectedIndex = options
            .Index()
            .First(pair => pair.Item.Equals(defaultOption))
            .Index;
        while (true) {
            Console.SetCursorPosition(0, 0);
            Console.WriteLineColored($"{prompt}", ForegroundColor);
            Console.WriteLine();
            for (int i = 0; i < options.Count; i++) {
                Console.WriteColored("(", ForegroundColor);
                if (i == selectedIndex) {
                    Console.WriteColored("*", AccentColor);
                }
                else {
                    Console.WriteColored(" ", ForegroundColor);
                }
                Console.WriteColored(") ", ForegroundColor);
                Console.WriteLineColored(
                    optionDescription(options[i]),
                    ForegroundColor
                );
            }
            Console.WriteLine();
            Console.WriteColored("[ESC]   ", AccentColor);
            Console.WriteLineColored("Discard Changes", ForegroundColor);
            Console.WriteColored("[^][v]  ", AccentColor);
            Console.WriteLineColored("Change Selection", ForegroundColor);
            Console.WriteColored("[ENTER] ", AccentColor);
            Console.WriteLineColored("Confirm Selection", ForegroundColor);
            switch (Console.ReadKey(true).Key) {
                case ConsoleKey.Escape:
                    selectedOption = default;
                    return false;
                case ConsoleKey.UpArrow:
                    selectedIndex = Math.Max(selectedIndex - 1, 0);
                    break;
                case ConsoleKey.DownArrow:
                    selectedIndex = Math.Min(
                        selectedIndex + 1,
                        options.Count - 1
                    );
                    break;
                case ConsoleKey.Enter:
                    selectedOption = options[selectedIndex];
                    return true;
            }
        }
    }

    /// <summary>
    /// Spellchecks a word against a set of known words using a specified
    /// <see cref="IEditDistanceCalculator"/> and <see cref="Accuracy"/> level.
    /// </summary>
    /// <param name="calculator">
    /// The <see cref="IEditDistanceCalculator"/> to use for spell checking.
    /// </param>
    /// <param name="words">A set of known words to check against.</param>
    /// <param name="accuracy">
    /// The <see cref="Accuracy"/> level to use for spell checking.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="calculator"/> or <paramref name="words"/> is
    /// <see langword="null"/>.
    /// </exception>
    private static void SpellcheckWord(
        IEditDistanceCalculator calculator,
        IReadOnlySet<string> words,
        Accuracy accuracy
    ) {
        Guard.IsNotNull(calculator);
        Guard.IsNotNull(words);
        Console.WriteLine();
        Console.WriteLineColored(
            "Enter a word to spellcheck.",
            ForegroundColor
        );
        Console.WriteLine();
        Console.WriteColored("> ", AccentColor);
        Console.CursorVisible = true;
        string word = Console.ReadLine() ?? string.Empty;
        // The dictionary only contains lowercase words, so we convert the input
        // to lowercase as well.
        string source = word.ToLower(CultureInfo.CurrentCulture);
        Console.CursorVisible = false;
        Console.WriteLine();
        Console.WriteColored("The word '", ForegroundColor);
        Console.WriteColored(word, AccentColor);
        if (words.Contains(source)) {
            Console.WriteLineColored(
                "' is spelled correctly.",
                ForegroundColor
            );
        }
        else {
            int maxDistance = accuracy.MaxEditDistance(source.Length);
            IReadOnlyCollection<(string Target, int Distance)> matches = [
                .. words
                    .Select(
                        target => (
                            Target: target,
                            Distance: calculator.EditDistance(source, target)
                        )
                    )
                    .Where(match => match.Distance <= maxDistance)
                    .OrderBy(match => match.Distance)
                    .ThenBy(match => match.Target)
            ];
            if (matches.Count == 0) {
                Console.WriteLineColored(
                    "' might be misspelled (no possible matches were found).",
                    ForegroundColor
                );
                if (accuracy > Accuracy.Low) {
                    Console.WriteLineColored(
                        "Try selecting a lower accuracy to find more possible "
                            + "matches.",
                        ForegroundColor
                    );
                }
            }
            else {
                Console.WriteLineColored(
                    "' might be misspelled, the following possible matches "
                        + "were found.",
                    ForegroundColor
                );
                int maxTargetLength = matches.Max(match => match.Target.Length);
                string header = "Possible Match".PadRight(maxTargetLength)
                    + " | Edit Distance";
                Console.WriteLine();
                Console.WriteLineColored(header, ForegroundColor);
                Console.WriteLineColored(
                    new string('-', header.Length),
                    ForegroundColor
                );
                int paddingLength = Math.Max(
                    maxTargetLength,
                    "Possible Match".Length
                );
                foreach ((string target, int distance) in matches) {
                    Console.WriteColored(
                        $"{target.PadRight(paddingLength)} | ",
                        ForegroundColor
                    );
                    ConsoleColor foregroundColor;
                    if (distance <= Accuracy.High.MaxEditDistance(source.Length)) {
                        foregroundColor = ConsoleColor.Green;
                    }
                    else if (distance <= Accuracy.Medium.MaxEditDistance(source.Length)) {
                        foregroundColor = ConsoleColor.Yellow;
                    }
                    else {
                        foregroundColor = ConsoleColor.Red;
                    }
                    Console.WriteLineColored($"{distance}", foregroundColor);
                }
            }
        }
        Console.WriteLine();
        Console.WriteLineColored("Press any key to continue.", ForegroundColor);
        Console.ReadKey(true);
    }

    private static void Main() {
        IReadOnlySet<string> words = File.ReadLines(DictionaryPath)
            .ToFrozenSet();
        ConsoleColor previousForegroundColor = Console.ForegroundColor;
        ConsoleColor previousBackgroundColor = Console.BackgroundColor;
        Console.CursorVisible = false;
        Console.ForegroundColor = ForegroundColor;
        Console.BackgroundColor = BackgroundColor;
        IReadOnlyList<IEditDistanceCalculator> calculators = [
            new RecursiveEditDistanceCalculator(),
            new IterativeFullMatrixEditDistanceCalculator(),
            new IterativeOptimizedMatrixEditDistanceCalculator()
        ];
        IEditDistanceCalculator calculator = calculators[1];
        IReadOnlyList<Accuracy> accuracies = [
            Accuracy.Low,
            Accuracy.Medium,
            Accuracy.High
        ];
        Accuracy accuracy = accuracies[1];
        while (true) {
            DrawMainMenu(calculator, accuracy);
            switch (Console.ReadKey(true).Key) {
                case ConsoleKey.Escape:
                    Console.CursorVisible = true;
                    Console.ForegroundColor = previousForegroundColor;
                    Console.BackgroundColor = previousBackgroundColor;
                    return;
                case ConsoleKey.D1: {
                    bool selectionConfirmed = TrySelectOption(
                        "Select one of the following algorithms for spell "
                            + "checking.",
                        calculators,
                        calculator,
                        calculator => calculator.Description,
                        out IEditDistanceCalculator? selectedCalculator
                    );
                    if (selectionConfirmed) {
                        calculator = selectedCalculator!;
                    }
                    break;
                }
                case ConsoleKey.D2: {
                    bool selectionConfirmed = TrySelectOption(
                        "Select one of the following accuracies for spell "
                            + "checking.",
                        accuracies,
                        accuracy,
                        accuracy => accuracy.Description,
                        out Accuracy selectedAccuracy
                    );
                    if (selectionConfirmed) {
                        accuracy = selectedAccuracy;
                    }
                    break;
                }
                case ConsoleKey.D3:
                    SpellcheckWord(calculator, words, accuracy);
                    break;
            }
        }
    }

}