namespace CmdTools.Core.CmdMenuAndPages
{
    public abstract class Page
    {
        public string Title { get; private set; }
        public ICmdWizard Program { get; private set; }
        public IIOWrapper IOWrapper { get; private set; }

        public Page(string title, ICmdWizard program, IIOWrapper ioWrapper)
        {
            Title = title;
            Program = program;
            IOWrapper = ioWrapper;
        }

        public void Display()
        {
            if (Program.HasBreadcrumbHeader)
            {
                IOWrapper.WritePageHeader(Program.BreadcrumbHeader);
            }
            else
            {
                IOWrapper.WritePageHeader(Title);
            }
            DisplayContent();
        }

        protected abstract void DisplayContent();
    }
}
