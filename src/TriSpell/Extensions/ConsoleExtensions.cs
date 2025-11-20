using System;
using CommunityToolkit.Diagnostics;

namespace TriSpell.Extensions;

/// <summary>Contains useful extensions for the <see cref="Console"/>.</summary>
internal static class ConsoleExtensions {

    extension(Console) {

        /// <summary>
        /// Writes a specified text with a specified foreground color to the
        /// standard output stream.
        /// </summary>
        /// <remarks>
        /// The previous foreground color of the <see cref="Console"/> is saved
        /// and restored using the <see cref="Console.ForegroundColor"/>
        /// property before this method returns.
        /// </remarks>
        /// <param name="text">
        /// The text to write to the standard output stream.
        /// </param>
        /// <param name="foregroundColor">
        /// The <see cref="ConsoleColor"/> to use as the foreground color.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="text"/> is <see langword="null"/>.
        /// </exception>
        public static void WriteColored(
            string text,
            ConsoleColor foregroundColor
        ) {
            Guard.IsNotNull(text);
            ConsoleColor previousForegroundColor = Console.ForegroundColor;
            Console.ForegroundColor = foregroundColor;
            Console.Write(text);
            Console.ForegroundColor = previousForegroundColor;
        }

        /// <summary>
        /// Writes a specified text with a specified foreground color, followed
        /// by the current line terminator, to the standard output stream.
        /// </summary>
        /// <remarks>
        /// The previous foreground color of the <see cref="Console"/> is saved
        /// and restored using the <see cref="Console.ForegroundColor"/>
        /// property before this method returns.
        /// </remarks>
        /// <param name="text">
        /// The text to write to the standard output stream.
        /// </param>
        /// <param name="foregroundColor">
        /// The <see cref="ConsoleColor"/> to use as the foreground color.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="text"/> is <see langword="null"/>.
        /// </exception>
        public static void WriteLineColored(
            string text,
            ConsoleColor foregroundColor
        ) {
            Guard.IsNotNull(text);
            ConsoleColor previousForegroundColor = Console.ForegroundColor;
            Console.ForegroundColor = foregroundColor;
            Console.WriteLine(text);
            Console.ForegroundColor = previousForegroundColor;
        }

    }

}