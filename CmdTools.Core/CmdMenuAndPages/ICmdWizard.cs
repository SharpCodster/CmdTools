namespace CmdTools.Core.CmdMenuAndPages
{
    public interface ICmdWizard
    {
        bool HasBreadcrumbHeader { get; }
        string BreadcrumbHeader { get; }
        bool NavigationEnabled { get; }

        void Run();
        void AddPage(Page page);
        void NavigateHome();
        Page NavigateBack();
        T NavigateTo<T>() where T : Page;
    }
}
