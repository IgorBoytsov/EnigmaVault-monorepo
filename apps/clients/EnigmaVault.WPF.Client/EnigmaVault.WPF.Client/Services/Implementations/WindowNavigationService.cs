using EnigmaVault.WPF.Client.Enums;
using EnigmaVault.WPF.Client.Factories.Abstractions;
using EnigmaVault.WPF.Client.Services.Abstractions;
using EnigmaVault.WPF.Client.ViewModels.Abstractions;
using System.Windows;

namespace EnigmaVault.WPF.Client.Services.Implementations
{
    internal class WindowNavigationService : IWindowNavigationService
    {
        private Dictionary<WindowName, Window> _windows = [];
        private readonly Dictionary<string, IWindowFactory> _windowFactories = [];

        public WindowNavigationService(IEnumerable<IWindowFactory> windowFactories)
        {
            _windowFactories = windowFactories.ToDictionary(f => f.GetType().Name.Replace("Factory", ""), f => f);
        }

        public void Open(WindowName name, bool isOpenDialog = false)
        {
            if (_windows.TryGetValue(name, out Window? wnd) && wnd is not null)
                (isOpenDialog ? () => { wnd.ShowDialog(); } : (Action)wnd.Show)();
            else
                OpenWindow(name, isOpenDialog);
        }

        private void OpenWindow(WindowName windowName, bool isOpenDialog = false)
        {
            if (_windowFactories.TryGetValue(windowName.ToString(), out var factory))
            {
                var window = factory.CreateWindow();

                _windows[windowName] = window;
                
                window.Closed += (c, e) => _windows.Remove(windowName);

                (isOpenDialog ? () => { window.ShowDialog(); } : (Action)window.Show)();
            }
            else
                throw new Exception($"Такое окно не зарегистрировано {windowName}");
        }

        public void TransmittingValue<TData>(WindowName windowName, TData value, TransmittingParameter parameter = TransmittingParameter.None, bool isActive = false)
        {
            if (_windows.TryGetValue(windowName, out Window? window))
            {
                if (window.DataContext is IUpdatable viewModel)
                {
                    viewModel.Update<TData>(value, parameter);

                    if (isActive)
                        window.Activate();  
                }
            }
        }

        public void Close(WindowName windowName)
        {
            if (_windows.TryGetValue(windowName, out Window? window))
                window.Close();
        }

        public void MinimizeWindow(WindowName windowName)
        {
            if (_windows.TryGetValue(windowName, out Window? window))
                SystemCommands.MinimizeWindow(window);
        }

        public void MaximizeWindow(WindowName windowName)
        {
            if (_windows.TryGetValue(windowName, out Window? window))
                SystemCommands.MaximizeWindow(window);
        }

        public void RestoreWindow(WindowName windowName)
        {
            if (_windows.TryGetValue(windowName, out Window? window))
                SystemCommands.RestoreWindow(window);
        }
    }
}