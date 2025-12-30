using System.Text.RegularExpressions;
using MetaHammer.Domain.Exceptions;

namespace MetaHammer.Domain.Validation;

public static partial class NameFormatValidator
{
    private static readonly Regex PascalCaseRegex = GeneratePascalCaseRegex();
    private static readonly Regex SnakeCaseRegex = GenerateSnakeCaseRegex();

    public static void ValidatePascalCase(string name, string context)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException($"{context} name cannot be empty");

        if (!PascalCaseRegex.IsMatch(name))
            throw new DomainException($"{context} name '{name}' must be in PascalCase format (e.g., 'Person', 'OrderLine')");
    }

    public static void ValidateSnakeCase(string name, string context)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException($"{context} name cannot be empty");

        if (!SnakeCaseRegex.IsMatch(name))
            throw new DomainException($"{context} name '{name}' must be in snake_case format (e.g., 'first_name', 'order_date')");
    }

    [GeneratedRegex(@"^[A-Z][a-zA-Z0-9]*$", RegexOptions.Compiled)]
    private static partial Regex GeneratePascalCaseRegex();

    [GeneratedRegex(@"^[a-z][a-z0-9]*(_[a-z0-9]+)*$", RegexOptions.Compiled)]
    private static partial Regex GenerateSnakeCaseRegex();
}
