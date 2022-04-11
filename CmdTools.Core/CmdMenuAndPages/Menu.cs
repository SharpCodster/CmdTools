using Spectre.Console;

namespace CmdTools.Core.CmdMenuAndPages
{
    public sealed class Menu
    {
        private IList<Option> Options { get; set; }
        public IIOWrapper IOWrapper { get; private set; }

        public Menu(IIOWrapper ioWrapper)
        {
            IOWrapper = ioWrapper;
            Options = new List<Option>();
        }

        public void Display(string title)
        {
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<Option>()
                    .Title(title)
                    .PageSize(10)
                    .MoreChoicesText("[grey](Move up and down to reveal more choices)[/]")
                    .AddChoices(Options));

            if (choice.Callback != null)
            {
                choice.Callback();
            }
        }

        public Menu Add(string option, Action callback)
        {
            return Add(new Option(option, callback));
        }

        public Menu Add(Option option)
        {
            Options.Add(option);
            return this;
        }

        public bool Contains(string option)
        {
            return Options.FirstOrDefault((op) => op.Name.Equals(option)) != null;
        }
    }
}
