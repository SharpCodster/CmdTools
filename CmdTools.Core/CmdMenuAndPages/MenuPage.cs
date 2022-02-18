namespace CmdTools.Core.CmdMenuAndPages
{
    public abstract class MenuPage : Page
    {
        protected Menu Menu { get; set; }
        private string _choiceText;

        public MenuPage(string title, string choiceText, ICmdWizard program, IIOWrapper ioWrapper, params Option[] options)
            : base(title, program, ioWrapper)
        {
            _choiceText = choiceText;
            Menu = new Menu(ioWrapper);

            foreach (var option in options)
                Menu.Add(option);
        }

        protected override void DisplayContent()
        {
            if (Program.NavigationEnabled && !Menu.Contains("Go back"))
            {
                Menu.Add("Go back", () => { Program.NavigateBack(); });
            }

            Menu.Display(_choiceText);
        }
    }
}
