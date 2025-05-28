using EnigmaVault.WPF.Client.Enums;
using EnigmaVault.WPF.Client.Factories.Abstractions;
using EnigmaVault.WPF.Client.Services.Abstractions;
using EnigmaVault.WPF.Client.ViewModels.Abstractions;
using System.Windows.Controls;

namespace EnigmaVault.WPF.Client.Services.Implementations
{
    internal class PageNavigationService : IPageNavigationService
    {
        private Dictionary<FrameName, Frame> _frames = [];
        private Dictionary<PageName, Page> _pages = [];
        private readonly Dictionary<string, IPageFactory> _pagesFactories = [];

        public PageNavigationService(IEnumerable<IPageFactory> pageFactories)
        {
            _pagesFactories = pageFactories.ToDictionary(f => f.GetType().Name.Replace("Factory", ""), f => f);
        }

        public void RegisterFrame(FrameName frameName, Frame frame)
        {
            if (!_frames.TryAdd(frameName, frame))
                throw new Exception($"Frame с именем '{frameName}' уже зарегистрирована.");
        }

        public void Navigate(PageName pageName, FrameName frameName)
        {
            if (_pages.TryGetValue(pageName, out var pageExist))
            {
                if (_frames.TryGetValue(frameName, out var frame))
                {
                    frame.Navigate(pageExist);
                }
                else throw new Exception($"Frame с именем '{frameName}' не зарегистрировано.");
            }
            else
            {
                if (_pagesFactories.TryGetValue(pageName.ToString(), out var factory))
                {
                    if (_frames.TryGetValue(frameName, out var frame))
                    {
                        var page = factory.CreatePage();
                        _pages.TryAdd(pageName, page);
                        frame.Navigate(page);
                    }
                    else throw new Exception($"Frame с именем '{frameName}' не зарегистрирован.");
                }
                else throw new Exception($"Factory у страницы с именем '{pageName}' не существует.");
            }
        }

        public void TransmittingValue<TData>(PageName pageName, FrameName frameName, TData value, TransmittingParameter typeParameter = TransmittingParameter.None, bool isNavigateAfterTransmitting = true, bool forceOpenPage = true)
        {
            if (_pages.TryGetValue(pageName, out var pageExist))
            {
                if (pageExist.DataContext is IUpdatable viewModel)
                {
                    if (_frames.TryGetValue(frameName, out var frame))
                    {
                        viewModel.Update(value, typeParameter);

                        if (isNavigateAfterTransmitting)
                            frame.Navigate(pageExist);
                    }
                    else throw new Exception($"Frame с именем '{frameName}' не зарегистрировано.");
                }
                else throw new Exception($"У ViewModel страницы '{pageName}' не реализован интерфейс IUpdatable.");
            }
            else
            {
                if (forceOpenPage)
                    Navigate(pageName, frameName);
            }
        }

        public void Close(PageName pageName, FrameName frameName)
        {
            if (_pages.ContainsKey(pageName) && _frames.TryGetValue(frameName, out var frameExist))
            {
                frameExist.Navigate(null);
                _pages.Remove(pageName);
            }
            else throw new Exception("Такой страницы либо фрейма не существует.");
        }
    }
}