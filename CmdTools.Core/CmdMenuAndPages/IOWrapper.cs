using Spectre.Console;

namespace CmdTools.Core.CmdMenuAndPages
{
    public class IOWrapper : IIOWrapper
    {
        public void WritePageHeader(string header)
        {
            var rule = new Rule($"{header}");
            rule.Style = Style.Parse("lightsalmon3_1 dim");
            //rule.Alignment = Justify.Left;
            AnsiConsole.Write(rule);

            AnsiConsole.WriteLine();
        }

        public void WriteLine(string message)
        {
            AnsiConsole.MarkupLine(message);
        }

        public void WriteException(Exception ex)
        {
            AnsiConsole.WriteException(ex, ExceptionFormats.Default);
        }

        public bool GetConfirmation(string text, bool defaultValue = true)
        {
            return AnsiConsole.Confirm(text, defaultValue);
        }

        public int ReadInt(string prompt, int defaultValue, int min, int max)
        {
            return AnsiConsole.Prompt(
                new TextPrompt<int>(prompt)
                    .PromptStyle("green")
                    .DefaultValue(defaultValue)
                    .ValidationErrorMessage("[red]That's not a valid age[/]")
                    .Validate(value =>
                    {
                        if (value < min)
                        {
                            return ValidationResult.Error($"[red]The number must be greater than {min}[/]");
                        }
                        if (value > max)
                        {
                            return ValidationResult.Error($"[red]The number must be less than {max}[/]");
                        }

                        return ValidationResult.Success();

                    }));
        }



        public string ReadString(string prompt)
        {
            var name = AnsiConsole.Ask<string>(prompt);
            return name;
        }

    }
}
