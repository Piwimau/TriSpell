using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Diagnostics;
using TriSpell.EditDistanceCalculators;

namespace TriSpell;

internal sealed class Program {

    /// <summary>Default foreground color of the console output.</summary>
    private const ConsoleColor ForegroundColor = ConsoleColor.White;

    /// <summary>Default background color of the console output.</summary>
    private const ConsoleColor BackgroundColor = ConsoleColor.Black;

    /// <summary>Default accent color of the console output.</summary>
    private const ConsoleColor AccentColor = ConsoleColor.DarkCyan;

    /// <summary>
    /// Path to the dictionary file containing a set of known words.
    /// </summary>
    private static readonly string DictionaryPath = Path.Combine(
        AppContext.BaseDirectory,
        "Resources",
        "Dictionary.txt"
    );

    /// <summary>
    /// Asynchronously reads the dictionary file and returns a set of known
    /// words.
    /// </summary>
    /// <returns>A set of known words.</returns>
    private static async Task<FrozenSet<string>> ReadDictionaryAsync() {
        HashSet<string> words = [];
        await foreach (string word in File.ReadLinesAsync(DictionaryPath)) {
            words.Add(word);
        }
        return words.ToFrozenSet();
    }

    /// <summary>
    /// Writes a specified text with a custom foreground color to the standard
    /// output stream.
    /// </summary>
    /// <remarks>
    /// The previous foreground color of the <see cref="Console"/> is saved and
    /// restored using the <see cref="Console.ForegroundColor"/> property before
    /// this call returns.
    /// </remarks>
    /// <param name="text">
    /// The text to write to the standard output stream.
    /// </param>
    /// <param name="foregroundColor">
    /// The custom <see cref="ConsoleColor"/> to use as the foreground color.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="text"/> is <see langword="null"/>.
    /// </exception>
    private static void WriteColored(
        string text,
        ConsoleColor foregroundColor
    ) {
        Guard.IsNotNull(text);
        ConsoleColor oldForegroundColor = Console.ForegroundColor;
        Console.ForegroundColor = foregroundColor;
        Console.Write(text);
        Console.ForegroundColor = oldForegroundColor;
    }

    /// <summary>
    /// Writes a specified text with a custom foreground color, followed by the
    /// current line terminator, to the standard output stream.
    /// </summary>
    /// <remarks>
    /// The previous foreground color of the <see cref="Console"/> is saved and
    /// restored using the <see cref="Console.ForegroundColor"/> property before
    /// this call returns.
    /// </remarks>
    /// <param name="text">
    /// The text to write to the standard output stream.
    /// </param>
    /// <param name="foregroundColor">
    /// The custom <see cref="ConsoleColor"/> to use as the foreground color.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="text"/> is <see langword="null"/>.
    /// </exception>
    private static void WriteLineColored(
        string text,
        ConsoleColor foregroundColor
    ) {
        Guard.IsNotNull(text);
        ConsoleColor oldForegroundColor = Console.ForegroundColor;
        Console.ForegroundColor = foregroundColor;
        Console.WriteLine(text);
        Console.ForegroundColor = oldForegroundColor;
    }

    /// <summary>Shows the main menu of the application.</summary>
    /// <param name="calculator">
    /// The current <see cref="IEditDistanceCalculator"/>.
    /// </param>
    /// <param name="accuracy">The current <see cref="Accuracy"/> level.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="calculator"/> is <see langword="null"/>.
    /// </exception>
    private static void ShowMainMenu(
        IEditDistanceCalculator calculator,
        Accuracy accuracy
    ) {
        Guard.IsNotNull(calculator);
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
           AccentColor
        );
        WriteColored("Welcome to ", ForegroundColor);
        WriteColored("TriSpell", AccentColor);
        WriteLineColored(
            ", a small and simple console application for basic spell "
                + "checking.\n",
            ForegroundColor
        );
        WriteLineColored("Settings", ForegroundColor);
        string accuracyDescription = accuracy.ToString();
        int tableWidth = "Algorithm | ".Length + Math.Max(
            calculator.Description.Length,
            accuracyDescription.Length
        );
        WriteLineColored(new string('-', tableWidth), ForegroundColor);
        WriteLineColored(
            $"Algorithm | {calculator.Description}",
            ForegroundColor
        );
        WriteLineColored(
            $"Accuracy  | {accuracyDescription}\n",
            ForegroundColor
        );
        WriteColored("[ESC] ", AccentColor);
        WriteLineColored("Exit Program", ForegroundColor);
        WriteColored("[1]   ", AccentColor);
        WriteLineColored("Select Algorithm", ForegroundColor);
        WriteColored("[2]   ", AccentColor);
        WriteLineColored("Select Accuracy", ForegroundColor);
        WriteColored("[3]   ", AccentColor);
        WriteLineColored("Spellcheck Word", ForegroundColor);
    }

    /// <summary>
    /// Displays a prompt and allows the user to select an option from a list of
    /// choices.
    /// </summary>
    /// <typeparam name="TOption">
    /// The type of the options in the list.
    /// </typeparam>
    /// <param name="prompt">
    /// The message displayed to the user to describe the context.
    /// </param>
    /// <param name="options">
    /// A read-only list of options from which the user can select.
    /// </param>
    /// <param name="defaultOption">The option selected by default.</param>
    /// <param name="optionDescription">
    /// A function that provides a string description for each option.
    /// </param>
    /// <param name="selectedOption">
    /// The selected option if the user confirms a selection, otherwise
    /// <see langword="null"/>.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the user confirms a selection, otherwise
    /// <see langword="false"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="prompt"/>, <paramref name="options"/> or
    /// <paramref name="optionDescription"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="defaultOption"/> is not one of the available
    /// options.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="options"/> does not contain at least two
    /// options.
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
        Guard.HasSizeGreaterThanOrEqualTo(options, 1);
        Guard.IsNotNull(optionDescription);
        Console.Clear();
        int selectedIndex = options
            .Index()
            .First(pair => pair.Item.Equals(defaultOption))
            .Index;
        while (true) {
            Console.SetCursorPosition(0, 0);
            WriteLineColored($"{prompt}\n", ForegroundColor);
            for (int i = 0; i < options.Count; i++) {
                WriteColored("(", ForegroundColor);
                if (i == selectedIndex) {
                    WriteColored("*", AccentColor);
                }
                else {
                    WriteColored(" ", ForegroundColor);
                }
                WriteColored(") ", ForegroundColor);
                WriteLineColored(
                    optionDescription(options[i]),
                    ForegroundColor
                );
            }
            WriteColored("\n[ESC]   ", AccentColor);
            WriteLineColored("Discard Changes", ForegroundColor);
            WriteColored("[^][v]  ", AccentColor);
            WriteLineColored("Change Selected Option", ForegroundColor);
            WriteColored("[ENTER] ", AccentColor);
            WriteLineColored("Confirm Selected Option", ForegroundColor);
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
    /// Spellchecks a word entered by the user against a set of known words
    /// using the specified <see cref="IEditDistanceCalculator"/> and
    /// <see cref="Accuracy"/>.
    /// </summary>
    /// <param name="calculator">
    /// The <see cref="IEditDistanceCalculator"/> to use for spell checking.
    /// </param>
    /// <param name="words">A set of known words to check against.</param>
    /// <param name="accuracy">
    /// The <see cref="Accuracy"/> to use for spell checking.
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
        WriteLineColored("\nEnter a word to spellcheck.\n", ForegroundColor);
        WriteColored("> ", AccentColor);
        Console.CursorVisible = true;
        string word = Console.ReadLine() ?? string.Empty;
        // The dictionary only contains lowercase words, so we convert the input
        // to lowercase.
        string source = word.ToLower(CultureInfo.CurrentCulture);
        Console.CursorVisible = false;
        WriteColored("\nThe word '", ForegroundColor);
        WriteColored(word, AccentColor);
        if (words.Contains(source)) {
            WriteLineColored("' is spelled correctly.", ForegroundColor);
        }
        else {
            int maxEditDistance = accuracy.MaxEditDistance(source.Length);
            IReadOnlyCollection<(string Target, int Distance)> possibleMatches = [
                .. words
                    .Select(
                        target => (
                            Target: target,
                            EditDistance: calculator.EditDistance(
                                source,
                                target
                            )
                        )
                    )
                    .Where(match => match.EditDistance <= maxEditDistance)
                    .OrderBy(match => match.EditDistance)
                    .ThenBy(match => match.Target)
            ];
            if (possibleMatches.Count == 0) {
                WriteLineColored(
                    "' might be misspelled (no possible matches were found).",
                    ForegroundColor
                );
                if (accuracy > Accuracy.Low) {
                    WriteLineColored(
                        "Try selecting a lower accuracy to find more possible "
                            + "matches.",
                        ForegroundColor
                    );
                }
            }
            else {
                WriteLineColored(
                    "' might be misspelled, the following possible matches "
                        + "were found.",
                    ForegroundColor
                );
                int maxTargetLength = possibleMatches.Max(
                    match => match.Target.Length
                );
                string header = "Possible Match".PadRight(maxTargetLength)
                    + " | Edit Distance";
                WriteLineColored($"\n{header}", ForegroundColor);
                WriteLineColored(
                    new string('-', header.Length),
                    ForegroundColor
                );
                foreach ((string target, int editDistance) in possibleMatches) {
                    int paddingLength = Math.Max(
                        maxTargetLength,
                        "Possible Match".Length
                    );
                    WriteColored(
                        $"{target.PadRight(paddingLength)} | ",
                        ForegroundColor
                    );
                    ConsoleColor foregroundColor;
                    if (editDistance
                            <= Accuracy.High.MaxEditDistance(source.Length)) {
                        foregroundColor = ConsoleColor.Green;
                    }
                    else if (editDistance
                            <= Accuracy.Medium.MaxEditDistance(source.Length)) {
                        foregroundColor = ConsoleColor.Yellow;
                    }
                    else {
                        foregroundColor = ConsoleColor.Red;
                    }
                    WriteLineColored($"{editDistance}", foregroundColor);
                }
            }
        }
        WriteLineColored("\nPress any key to continue.", ForegroundColor);
        Console.ReadKey(true);
    }

    private static async Task Main() {
        IReadOnlySet<string> words = await ReadDictionaryAsync();
        ConsoleColor oldForegroundColor = Console.ForegroundColor;
        ConsoleColor oldBackgroundColor = Console.BackgroundColor;
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
            ShowMainMenu(calculator, accuracy);
            switch (Console.ReadKey(true).Key) {
                case ConsoleKey.Escape:
                    Console.CursorVisible = true;
                    Console.ForegroundColor = oldForegroundColor;
                    Console.BackgroundColor = oldBackgroundColor;
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
                        accuracy => accuracy.ToString(),
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