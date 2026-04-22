using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DRLMobile.Activation;
using DRLMobile.Core.Helpers;
using DRLMobile.Views;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace DRLMobile.Services
{
    internal class ActivationService
    {
        private readonly App _app;
        private readonly Type _defaultNavItem;
        private Lazy<UIElement> _shell;

        private object _lastActivationArgs;

        public ActivationService(App app, Type defaultNavItem, Lazy<UIElement> shell = null)
        {
            _app = app;
            _shell = shell;
            _defaultNavItem = defaultNavItem;
        }

        public async Task ActivateAsync(object activationArgs)
        {
            if (IsInteractive(activationArgs))
            {
                // Initialize services that you need before app activation
                // take into account that the splash screen is shown while this code runs.
                await InitializeAsync();
                _lastActivationArgs = activationArgs;


                // Do not repeat app initialization when the Window already has content,
                // just ensure that the window is active
                SetRootFrame();

                await NavigateToInitialPage();
            }

            // Depending on activationArgs one of ActivationHandlers or DefaultActivationHandler
            // will navigate to the first page

            if (IsInteractive(activationArgs))
            {
                var activation = activationArgs as IActivatedEventArgs;
                if (activation.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    await Singleton<SuspendAndResumeService>.Instance.RestoreSuspendAndResumeData();
                }

                // Ensure the current window is active
                Window.Current.Activate();

                // Tasks after activation
                await StartupAsync();
            }
        }

        private async Task NavigateToInitialPage()
        {
            if (_shell?.Value != null)
                // navigate to shell page
                await NavigateToShellPage();
            else
                // navigate to login page
                NavigateToLoginPage();
        }

        private async Task NavigateToShellPage()
        {
            ((Frame)Window.Current.Content).Navigate(_shell?.Value.GetType());
            await HandleActivationAsync(_lastActivationArgs);
        }

        private void NavigateToLoginPage()
        {
            ((Frame)Window.Current.Content).Navigate(typeof(LoginPage));
        }

        private void SetRootFrame()
        {
            if (Window.Current.Content == null)
            {
                Frame rootFrame = Window.Current.Content as Frame;
                rootFrame = new Frame();
                Window.Current.Content = rootFrame;
            }
        }

        private async Task InitializeAsync()
        {
            await Task.CompletedTask;
        }

        private async Task HandleActivationAsync(object activationArgs)
        {
            var activationHandler = GetActivationHandlers()
                                                .FirstOrDefault(h => h.CanHandle(activationArgs));

            if (activationHandler != null)
            {
                await activationHandler.HandleAsync(activationArgs);
            }

            if (IsInteractive(activationArgs))
            {
                var defaultHandler = new DefaultActivationHandler(_defaultNavItem);
                if (defaultHandler.CanHandle(activationArgs))
                {
                    await defaultHandler.HandleAsync(activationArgs);
                }
            }
        }

        private async Task StartupAsync()
        {
            await Task.CompletedTask;
        }

        private IEnumerable<ActivationHandler> GetActivationHandlers()
        {
            yield return Singleton<SuspendAndResumeService>.Instance;
        }

        private bool IsInteractive(object args)
        {
            return args is IActivatedEventArgs;
        }


        public async void NavigateShell(Lazy<UIElement> shell)
        {
            _shell = shell;
            await ActivateAsync(_lastActivationArgs);
        }
    }
}
