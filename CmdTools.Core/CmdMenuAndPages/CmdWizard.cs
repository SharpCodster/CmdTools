namespace CmdTools.Core.CmdMenuAndPages
{
    public abstract class CmdWizard : ICmdWizard
    {   
        public bool HasBreadcrumbHeader { get; private set; }  
        public bool NavigationEnabled { get { return _history.Count > 1; } }    
        public string BreadcrumbHeader 
        {
            get
            {
                string breadcrumb = null;

                if (_history.Count > 0)
                {

                    foreach (var title in _history.Select((page) => page.Title).Reverse())
                    {
                        breadcrumb += title + " > ";
                    }
                    breadcrumb = breadcrumb.Remove(breadcrumb.Length - 3);
                }
                return breadcrumb;
            }
        }
        protected Page CurrentPage
        {
            get
            {
                return _history.Any() ? _history.Peek() : null;
            }
        }


        private string _title;
        private Stack<Page> _history;
        private Dictionary<Type, Page> _pages;
        private IIOWrapper _ioWrapper;

        protected CmdWizard(string title, bool breadcrumbHeader, IIOWrapper ioWrapper)
        {
            _title = title;
            _pages = new Dictionary<Type, Page>();
            _history = new Stack<Page>();
            HasBreadcrumbHeader = breadcrumbHeader;
            _ioWrapper = ioWrapper;
        }

        public virtual void Run()
        {
            try
            {
                Console.Title = _title;
                if (CurrentPage != null)
                {
                    CurrentPage.Display();
                }
            }
            catch (Exception e)
            {
                _ioWrapper.WriteException(e);
            }
            finally
            {
                _ioWrapper.ReadString("Press [Enter] to exit");
            }
        }

        public void AddPage(Page page)
        {
            Type pageType = page.GetType();

            if (_pages.ContainsKey(pageType))
            {
                _pages[pageType] = page;
            }   
            else
            {
                _pages.Add(pageType, page);
            }     
        }

        public void NavigateHome()
        {
            while (_history.Count > 1)
            {
                _history.Pop();
            }
            Console.Clear();
            CurrentPage.Display();
        }

        public Page NavigateBack()
        {
            _history.Pop();
            Console.Clear();
            CurrentPage.Display();
            return CurrentPage;
        }

        public T NavigateTo<T>() where T : Page
        {
            SetPage<T>();
            Console.Clear();
            CurrentPage.Display();
            return CurrentPage as T;
        }

        protected void SetPage<T>() where T : Page
        {
            Type pageType = typeof(T);

            if (CurrentPage != null && CurrentPage.GetType() == pageType)
            {
                return;
            }
            
            Page nextPage;
            if (!_pages.TryGetValue(pageType, out nextPage))
            {
                throw new KeyNotFoundException($"The page \"{pageType.Name}\" was not foud.");
            }

            _history.Push(nextPage);
        } 
    }
}
