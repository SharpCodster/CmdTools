using Spectre.Console;

namespace CmdTools.Core.CmdMenuAndPages
{
    public class IOWrapper : IIOWrapper
    {
        public void WritePageHeader(string header)
        {
            var rule = new Rule($"{header}");
            rule.Style = Style.Parse("lightsalmon3_1 dim");
            rule.Alignment = Justify.Left;
            AnsiConsole.Write(rule);

            AnsiConsole.WriteLine();
        }

        public void WriteLine(string message)
        {
            AnsiConsole.WriteLine(message);
        }

        public void WriteException(Exception ex)
        {
            AnsiConsole.WriteException(ex, ExceptionFormats.Default);
        }

        public void DisplayPrompt(string format, params object[] args)
        {
            format = format.Trim() + " ";
            Console.Write(format, args);
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
                            return ValidationResult.Error("[red]You must at least be 1 years old[/]");
                        }
                        if (value > max)
                        {
                            return ValidationResult.Error("[red]You must be younger than the oldest person alive[/]");
                        }

                        return ValidationResult.Success();

                    }));
        }

        

        public int ReadInt(int min, int max)
        {
            int value = ReadInt();

            while (value < min || value > max)
            {
                DisplayPrompt("Please enter an integer between {0} and {1} (inclusive)", min, max);
                value = ReadInt();
            }

            return value;
        }

        public int? ReadNullableInt(int min, int max)
        {
            int? value = ReadNullableInt<int>("", 0);

            //while (value.HasValue && (value < min || value > max))
            //{
            //    DisplayPrompt("Please enter an integer between {0} and {1} (inclusive)", min, max);
            //    value = ReadNullableInt();
            //}

            return value;
        }

        public int ReadInt()
        {
            string input = Console.ReadLine();
            int value;

            while (!int.TryParse(input, out value))
            {
                DisplayPrompt("Please enter an integer");
                input = Console.ReadLine();
            }

            return value;
        }

        public T ReadNullableInt<T>(string prompt, T defaultValue)
        {

            return AnsiConsole.Prompt(
                new TextPrompt<T>("prompt")
                    .PromptStyle("green")
                    .DefaultValue(defaultValue)
                    .ValidationErrorMessage("[red]That's not a valid age[/]")
                    .Validate(age =>
                    {
                        return age switch
                        {
                            <= 0 => ValidationResult.Error("[red]You must at least be 1 years old[/]"),
                            >= 123 => ValidationResult.Error("[red]You must be younger than the oldest person alive[/]"),
                            _ => ValidationResult.Success(),
                        };
                    }));

            //string input = Console.ReadLine();
            //int? res = null; 

            //if (!String.IsNullOrEmpty(input))
            //{
            //    int value;
            //    while (!int.TryParse(input, out value))
            //    {
            //        DisplayPrompt("Please enter an integer");
            //        input = Console.ReadLine();
            //    }
            //    res = value;
            //}

            //return res;
        }

        public string ReadString(string prompt)
        {
            DisplayPrompt(prompt);
            return Console.ReadLine();
        }

        //public TEnum ReadEnum<TEnum>(string prompt) where TEnum : struct, IConvertible, IComparable, IFormattable
        //{
        //    Type type = typeof(TEnum);

        //    if (!type.IsEnum)
        //        throw new ArgumentException("TEnum must be an enumerated type");

        //    WriteLine(prompt);
        //    Menu menu = new Menu(this);

        //    TEnum choice = default(TEnum);
        //    foreach (var value in Enum.GetValues(type))
        //        menu.Add(Enum.GetName(type, value), () => { choice = (TEnum)value; });
        //    menu.Display("");

        //    return choice;
        //}
    }
}
