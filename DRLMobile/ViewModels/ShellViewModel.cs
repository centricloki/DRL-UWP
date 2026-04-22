using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using DRLMobile.Constants;
using DRLMobile.Core.Helpers;
using DRLMobile.Core.Models;
using DRLMobile.Core.Models.UIModels;
using DRLMobile.Core.Services;
using DRLMobile.Helpers;
using DRLMobile.Services;
using DRLMobile.Views;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Windows.ApplicationModel.Resources;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

using WinUI = Microsoft.UI.Xaml.Controls;

namespace DRLMobile.ViewModels
{
    public class ShellViewModel : BaseModel
    {
        private readonly KeyboardAccelerator _altLeftKeyboardAccelerator = BuildKeyboardAccelerator(VirtualKey.Left, VirtualKeyModifiers.Menu);
        private readonly KeyboardAccelerator _backKeyboardAccelerator = BuildKeyboardAccelerator(VirtualKey.GoBack);

        private bool _isBackEnabled;
        private string _tempuserName;

        private IList<KeyboardAccelerator> _keyboardAccelerators;
        private WinUI.NavigationView _navigationView;
        private WinUI.NavigationViewItem _selected;

        private ICommand _loadedCommand;
        private ICommand _itemInvokedCommand;

        public ICommand NavigatedToCommand { private set; get; }
        public ICommand NavigatingFromCommand { private set; get; }

        private LoggedInUserDetailsUIModel _loggedInUserInfo;
        public LoggedInUserDetailsUIModel UserInformation
        {
            get => _loggedInUserInfo;
            set
            {
                _loggedInUserInfo = value;
                OnPropertyChanged();
            }
        }

        private string _lastSyncDateTime;
        public string LastSyncDateTime
        {
            get { return _lastSyncDateTime; }
            set { SetProperty(ref _lastSyncDateTime, value); }
        }
        public bool IsBackEnabled
        {
            get { return _isBackEnabled; }
            set { SetProperty(ref _isBackEnabled, value); }
        }

        public string TempUserName
        {
            get { return _tempuserName; }
            set { SetProperty(ref _tempuserName, value); }
        }

        public WinUI.NavigationViewItem Selected
        {
            get { return _selected; }
            set { SetProperty(ref _selected, value); }
        }

        public ICommand LoadedCommand => _loadedCommand ?? (_loadedCommand = new RelayCommand(OnLoaded));

        public ICommand ItemInvokedCommand => _itemInvokedCommand ?? (_itemInvokedCommand = new RelayCommand<WinUI.NavigationViewItemInvokedEventArgs>(OnItemInvoked));

        public ShellViewModel()
        {
            NavigatedToCommand = new RelayCommand(NavigatedToCommandHandler);
            NavigatingFromCommand = new RelayCommand(NavigatingFromCommandHandler);
        }

        public void Initialize(Frame frame, WinUI.NavigationView navigationView, IList<KeyboardAccelerator> keyboardAccelerators)
        {
            _navigationView = navigationView;
            _keyboardAccelerators = keyboardAccelerators;
            NavigationService.Frame = frame;
            NavigationService.NavigationFailed += Frame_NavigationFailed;
            NavigationService.Navigated += Frame_Navigated;
            _navigationView.BackRequested += OnBackRequested;
        }

        private async void OnLoaded()
        {
            // Keyboard accelerators are added here to avoid showing 'Alt + left' tooltip on the page.
            _keyboardAccelerators.Add(_altLeftKeyboardAccelerator);
            _keyboardAccelerators.Add(_backKeyboardAccelerator);

            UserInformation = await (Application.Current as App).QueryService.GetLoggedInUserInformation(Convert.ToInt32(((App)Application.Current).LoginUserIdProperty));

            LastSyncDateTime = DateTimeHelper.ConvertStringToSyncDateTimeFormat(((App)Application.Current).LastSyncDateTimeProperty);

            TempUserName = "Logged in as : " + UserInformation.FirstName + " " + UserInformation.LastName;

            await Task.CompletedTask;
        }

        private void OnItemInvoked(WinUI.NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked)
            {
                // Navigate to the settings page - implement as appropriate if needed
            }
            else
            {
                var selectedItem = args.InvokedItemContainer as WinUI.NavigationViewItem;
                var pageType = selectedItem?.GetValue(NavHelper.NavigateToProperty) as Type;

                if (pageType != null)
                {
                    NavigationService.Navigate(pageType, null, args.RecommendedNavigationTransitionInfo);
                }
            }
        }

        private void OnBackRequested(WinUI.NavigationView sender, WinUI.NavigationViewBackRequestedEventArgs args)
        {
            NavigationService.GoBack();
        }

        private void Frame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw e.Exception;
        }

        private void Frame_Navigated(object sender, NavigationEventArgs e)
        {
            IsBackEnabled = NavigationService.CanGoBack;
            var selectedItem = GetSelectedItem(_navigationView.MenuItems, e.SourcePageType);
            if (selectedItem != null)
            {
                Selected = selectedItem;
            }
        }

        private WinUI.NavigationViewItem GetSelectedItem(IEnumerable<object> menuItems, Type pageType)
        {
            foreach (var item in menuItems.OfType<WinUI.NavigationViewItem>())
            {
                if (IsMenuItemForPageType(item, pageType))
                {
                    return item;
                }

                var selectedChild = GetSelectedItem(item.MenuItems, pageType);
                if (selectedChild != null)
                {
                    return selectedChild;
                }
            }

            return null;
        }

        private bool IsMenuItemForPageType(WinUI.NavigationViewItem menuItem, Type sourcePageType)
        {
            var pageType = menuItem.GetValue(NavHelper.NavigateToProperty) as Type;
            return pageType == sourcePageType;
        }

        private static KeyboardAccelerator BuildKeyboardAccelerator(VirtualKey key, VirtualKeyModifiers? modifiers = null)
        {
            var keyboardAccelerator = new KeyboardAccelerator() { Key = key };
            if (modifiers.HasValue)
            {
                keyboardAccelerator.Modifiers = modifiers.Value;
            }

            keyboardAccelerator.Invoked += OnKeyboardAcceleratorInvoked;
            return keyboardAccelerator;
        }

        private static void OnKeyboardAcceleratorInvoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            var result = NavigationService.GoBack();
            args.Handled = result;
        }

        private void NavigatedToCommandHandler()
        {
            CustomerPageViewModel.NavigationEventHandler += NavigationEvent;
        }

        private void NavigatingFromCommandHandler()
        {
            CustomerPageViewModel.NavigationEventHandler -= NavigationEvent;
        }

        private void NavigationEvent(object sender, int id)
        {
            if (id != 0)
                NavigationService.Navigate<CustomerDetailsPage>(id);
            else
                NavigationService.Navigate<AddCustomerPage>(id);
        }
    }
}
