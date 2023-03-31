using AngleSharp.Common;
using System.Text;

namespace eCommerceWebAPI.Application.Extensions
{
    public static class StringUtilities
    {
        static StringUtilities()
        {
        }

        public static string GenerateRandomString(int wordsMinLength = 0,
            int wordsMaxLength = 0,
            int minNumberOfWords = 1,
            int maxNumberOfWords = 1,
            string customCharacters = "",
            bool includeAlphabets = true,
            bool includeNumerals = false,
            StringCaseEnum stringCase = StringCaseEnum.LowerCaseSpaceBetween,
            string endingPhrase = "")
        {
            string chars;
            if (!string.IsNullOrEmpty(customCharacters))
            {
                chars = customCharacters;
            }
            else if (stringCase == StringCaseEnum.ShuffleCaseWithoutSpace ||
                stringCase == StringCaseEnum.ShuffleCaseSpaceBetween ||
                stringCase == StringCaseEnum.ShuffleSnakeCase ||
                stringCase == StringCaseEnum.ShuffleKebabCase)
            {
                chars = "aAbBcCdDeEfFgGhHiIjJkKlLmMnNoOpPqQrRsStTuUvVwWxXyYzZ";
                if (includeNumerals)
                {
                    chars = $"{chars}0123456789";
                }
            }
            else if (!includeNumerals)
            {
                chars = "abcdefghijklmnopqrstuvwxyz";
            }
            else if (includeNumerals && !includeAlphabets)
            {
                chars = "0123456789";
            }
            else
            {
                chars = "abcdefghijklmnopqrstuvwxyz0123456789";
            }

            Random random = new();

            if (wordsMinLength == 0 && wordsMaxLength == 0)
            {
                wordsMinLength = random.Next(1, 7);
                wordsMaxLength = random.Next(wordsMinLength, wordsMinLength + 7);
            }
            wordsMinLength = Math.Max(1, wordsMinLength);
            wordsMaxLength = Math.Max(wordsMinLength, wordsMaxLength);

            if (minNumberOfWords == 0 && maxNumberOfWords == 0)
            {
                minNumberOfWords = random.Next(1, 7);
                maxNumberOfWords = random.Next(minNumberOfWords, minNumberOfWords + 7);
            }
            minNumberOfWords = Math.Max(1, minNumberOfWords);
            maxNumberOfWords = Math.Max(minNumberOfWords, maxNumberOfWords);
            int numberOfWords = random.Next(minNumberOfWords, maxNumberOfWords + 1);

            List<string> words = new(numberOfWords);
            for (int i = 0; i < numberOfWords; i++)
            {
                int wordLength = wordsMinLength == wordsMaxLength ? wordsMinLength : random.Next(wordsMinLength, wordsMaxLength + 1);

                string word = new string(Enumerable
                    .Repeat(chars, wordLength)
                    .Select(s => s[random.Next(s.Length)])
                    .ToArray());

                words.Add(word);
            }

            switch (stringCase)
            {
                case StringCaseEnum.LowerCaseWithoutSpace:
                case StringCaseEnum.LowerCaseSpaceBetween:
                case StringCaseEnum.SnakeLowerCase:
                case StringCaseEnum.KebabLowerCase:
                    words = words.Select(w => w.ToLowerInvariant()).ToList();
                    break;
                case StringCaseEnum.LowerCaseSpaceBetweenFirstCharUpperCase:
                    words = words.Select(w => w.ToLowerInvariant()).ToList();
                    string word = words[0];
                    words[0] = $"{word.Substring(0, 1).ToUpperInvariant()}{word.Substring(1)}";
                    break;
                case StringCaseEnum.UpperCaseWithoutSpace:
                case StringCaseEnum.UpperCaseSpaceBetween:
                case StringCaseEnum.SnakeUpperCase:
                case StringCaseEnum.KebabUpperCase:
                    words = words.Select(w => w.ToUpperInvariant()).ToList();
                    break;
                case StringCaseEnum.PascalCaseWithoutSpace:
                case StringCaseEnum.PascalCaseSpaceBetween:
                case StringCaseEnum.SnakePascalCase:
                case StringCaseEnum.KebabPascalCase:
                    words = words
                        .Select(w =>
                        {
                            string word = w;
                            w = $"{word.Substring(0, 1).ToUpperInvariant()}{word.Substring(1).ToLowerInvariant()}";
                            return w;
                        })
                        .ToList();
                    break;
                case StringCaseEnum.CamelCaseWithoutSpace:
                case StringCaseEnum.CamelCaseSpaceBetween:
                case StringCaseEnum.SnakeCamelCase:
                case StringCaseEnum.KebabCamelCase:
                    words
                        .Select(w =>
                        {
                            string word = w;
                            w = $"{word.Substring(0, 1).ToUpperInvariant()}{word.Substring(1).ToLowerInvariant()}";
                            return w;
                        })
                        .ToList();
                    string firstWord = words[0];
                    words[0] = $"{firstWord.Substring(0, 1).ToLowerInvariant()}{firstWord.Substring(1)}";
                    break;
                case StringCaseEnum.ShuffleCaseWithoutSpace:
                case StringCaseEnum.ShuffleCaseSpaceBetween:
                case StringCaseEnum.ShuffleSnakeCase:
                case StringCaseEnum.ShuffleKebabCase:
                default:
                    break;
            }

            string result = string.Empty;
            switch (stringCase)
            {
                case StringCaseEnum.LowerCaseWithoutSpace:
                case StringCaseEnum.UpperCaseWithoutSpace:
                case StringCaseEnum.PascalCaseWithoutSpace:
                case StringCaseEnum.CamelCaseWithoutSpace:
                case StringCaseEnum.ShuffleCaseWithoutSpace:
                    result = String.Join("", words);
                    break;
                case StringCaseEnum.LowerCaseSpaceBetween:
                case StringCaseEnum.LowerCaseSpaceBetweenFirstCharUpperCase:
                case StringCaseEnum.UpperCaseSpaceBetween:
                case StringCaseEnum.PascalCaseSpaceBetween:
                case StringCaseEnum.CamelCaseSpaceBetween:
                case StringCaseEnum.ShuffleCaseSpaceBetween:
                    result = String.Join(" ", words);
                    break;
                case StringCaseEnum.SnakeLowerCase:
                case StringCaseEnum.SnakeUpperCase:
                case StringCaseEnum.SnakePascalCase:
                case StringCaseEnum.SnakeCamelCase:
                case StringCaseEnum.ShuffleSnakeCase:
                    result = String.Join("_", words);
                    break;
                case StringCaseEnum.KebabLowerCase:
                case StringCaseEnum.KebabUpperCase:
                case StringCaseEnum.KebabPascalCase:
                case StringCaseEnum.KebabCamelCase:
                case StringCaseEnum.ShuffleKebabCase:
                    result = String.Join("-", words);
                    break;
                default:
                    break;
            }

            result = $"{result}{endingPhrase}".Trim();

            return result;
        }

        public static string GenerateRandomSentences(int minNumberOfSentences = 1,
            int maxNumberOfSentences = 10,
            int minNumberOfSentenceWords = 1,
            int maxNumberOfSentenceWords = 10,
            string endingChars = ".!?")
        {
            Random random = new();

            if (string.IsNullOrEmpty(endingChars))
            {
                endingChars = string.Empty;
            }

            int endingCharsLength = endingChars.Length;

            if (minNumberOfSentences == 0 && maxNumberOfSentences == 0)
            {
                minNumberOfSentences = random.Next(1, 7);
                maxNumberOfSentences = random.Next(minNumberOfSentences, minNumberOfSentences + 7);
            }
            minNumberOfSentences = Math.Max(1, minNumberOfSentences);
            maxNumberOfSentences = Math.Max(minNumberOfSentences, maxNumberOfSentences);
            int numberOfSentences = random.Next(minNumberOfSentences, maxNumberOfSentences + 1);

            var sb = new StringBuilder();
            for (int i = 0; i < numberOfSentences; i++)
            {
                string endingChar = endingChars.ToArray().GetItemByIndex(random.Next(endingCharsLength)).ToString();

                string randomSentence = GenerateRandomString(minNumberOfWords: minNumberOfSentenceWords,
                    maxNumberOfWords: maxNumberOfSentenceWords,
                    stringCase: StringCaseEnum.LowerCaseSpaceBetweenFirstCharUpperCase,
                    endingPhrase: endingChar);

                if (i > 0)
                {
                    sb.Append(" ");
                }
                sb.Append(randomSentence);
            }

            string result = sb.ToString().Trim();

            return result;
        }

        public static string GenerateRandomName() => GenerateRandomString(minNumberOfWords: 1, maxNumberOfWords: 3, stringCase: StringCaseEnum.PascalCaseSpaceBetween);
        public static string GenerateRandomFullName() => GenerateRandomString(minNumberOfWords: 2, maxNumberOfWords: 4, stringCase: StringCaseEnum.PascalCaseSpaceBetween);
        public static string GenerateRandomNationalCode() => GenerateRandomString(wordsMinLength: 10, wordsMaxLength: 10, includeAlphabets: false, includeNumerals: true, stringCase: StringCaseEnum.LowerCaseWithoutSpace);
        public static string GenerateRandomPostalCode() => GenerateRandomString(wordsMinLength: 5, wordsMaxLength: 10, includeAlphabets: false, includeNumerals: true, stringCase: StringCaseEnum.LowerCaseWithoutSpace);
        public static string GenerateRandomPhoneNumber() => GenerateRandomString(wordsMinLength: 11, wordsMaxLength: 14, includeAlphabets: false, includeNumerals: true, stringCase: StringCaseEnum.LowerCaseWithoutSpace);
    }
}
