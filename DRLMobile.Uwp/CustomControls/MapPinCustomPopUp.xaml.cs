using DRLMobile.Core.Models.DataModels;
using DRLMobile.Core.Models.UIModels;
using DRLMobile.ExceptionHandler;
using DRLMobile.Uwp.Helpers;
using DRLMobile.Uwp.Services;
using DRLMobile.Uwp.View;

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;


// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace DRLMobile.Uwp.CustomControls
{

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MapPinCustomPopUp : UserControl
    {
        private readonly App AppReference = (App)Application.Current;
        public string Latitude { get; private set; }
        public string Longitude { get; private set; }
        int _accountType;
        public ObservableCollection<MapPopAddToRouteListModel> RouteList;


        #region Constructor
        public MapPinCustomPopUp()
        {
            this.InitializeComponent();
            DataContext = this;
            RouteList = new ObservableCollection<MapPopAddToRouteListModel>();
        }

        #endregion

        #region Dependency Properties
        public string DeviceCustomerId
        {
            get { return (string)GetValue(DeviceCustomerIdProperty); }
            set { SetValue(DeviceCustomerIdProperty, value); }
        }

        //public static readonly DependencyProperty DeviceCustomerIdProperty =
        //    DependencyProperty.Register(nameof(DeviceCustomerId), typeof(string), typeof(MapPinCustomPopUp), new PropertyMetadata(defaultValue: null, propertyChangedCallback: OnDeviceCustomerIdChanged));
        public static readonly DependencyProperty DeviceCustomerIdProperty =
            DependencyProperty.Register(nameof(DeviceCustomerId), typeof(string), typeof(MapPinCustomPopUp), new PropertyMetadata(defaultValue: null, propertyChangedCallback: null));

        public int CustomerId
        {
            get { return (int)GetValue(CustomerIdProperty); }
            set { SetValue(CustomerIdProperty, value); }
        }

        public static readonly DependencyProperty CustomerIdProperty =
            DependencyProperty.Register(nameof(CustomerId), typeof(int), typeof(MapPinCustomPopUp), new PropertyMetadata(defaultValue: 0, propertyChangedCallback: OnCustomerIdChanged));

        public ICommand CloseCommad
        {
            get { return (ICommand)GetValue(CloseCommadProperty); }
            set { SetValue(CloseCommadProperty, value); }
        }

        public static readonly DependencyProperty CloseCommadProperty = DependencyProperty.Register(nameof(CloseCommad), typeof(ICommand),
            typeof(PreviewDocumentControl), new PropertyMetadata(null));

        #endregion

        #region Private Methods
        //private static void OnDeviceCustomerIdChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    var control = d as MapPinCustomPopUp;
        //    _ = control.GetPopupData();
        //}

        private static void OnCustomerIdChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as MapPinCustomPopUp;
            _ = control.GetPopupData();
        }

        public async Task GetPopupData()
        {
            //if (CustomerId > 0 && !string.IsNullOrWhiteSpace(DeviceCustomerId))
            if (CustomerId > 0)
            {
                var customer = await AppReference.QueryService.GetMapPopupCutomerData(CustomerId, DeviceCustomerId);
                if (customer != null)
                {
                    var states = await AppReference.QueryService.GetStateDict();
                    //AccountNameTextBlock.Text = customer?.CustomerName;
                    AddHyperLinkToAccountName(customer?.CustomerName);
                    AccountNumberTextBlock.Text = customer?.CustomerNumber;
                    AddressTextBlock.Text = customer?.PhysicalAddress + " "
                    + Core.Helpers.HelperMethods.GetValueFromIdNameDictionary(states, customer.PhysicalAddressStateID)
                    + " " + customer?.PhysicalAddressCityID + " " + customer?.PhysicalAddressZipCode;
                    PhoneTextBlock.Text = customer?.Phone;
                    LastCallDateTextBlock.Text = Core.Helpers.DateTimeHelper.ConvertStringDateToMM_DD_YYYY(customer?.LastCallActivityDate);
                    ManagerNameTextBlock.Text = customer?.ManagerName;
                    Latitude = customer?.Latitude;
                    Longitude = customer?.Longitude;
                    _accountType = customer.AccountType;
                }
                var activity = await AppReference.QueryService.GetMapPopupActivityData(DeviceCustomerId);
                if (activity != null && activity.Any())
                {
                    LastThreeCallActivity.Visibility = Visibility.Visible;
                    LastThreeCallActivityList.Visibility = Visibility.Visible;
                    LastThreeCallActivityList.ItemsSource = activity;
                }
                else
                {
                    LastThreeCallActivity.Visibility = Visibility.Collapsed;
                    LastThreeCallActivityList.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void AddHyperLinkToAccountName(string accountNameText)
        {
            AccountNameTextBlock.Inlines.Clear();
            Hyperlink hyperLink = new Hyperlink();
            hyperLink.Foreground = new SolidColorBrush(Windows.UI.Colors.Black);
            hyperLink.Inlines.Add(new Run { Text = accountNameText });
            hyperLink.Click += AccountNameTextBlock_Click;
            AccountNameTextBlock.Inlines.Add(hyperLink);
        }


        private async void AddtoRouteButton_Click(object sender, RoutedEventArgs e)
        {
            var routes = await AppReference.QueryService.GetRoutesForMapPopup(CustomerId);
            RouteList = new ObservableCollection<MapPopAddToRouteListModel>(routes);
            AddtoRouteList.ItemsSource = RouteList;
            FlyoutBase.ShowAttachedFlyout(sender as FrameworkElement);
        }
        #endregion

        private async Task<bool> IsSelectedCustomerAccountTypeDiff()
        {
            bool result = false;
            if (_accountType != 0 && !string.IsNullOrWhiteSpace(AppReference.SelectedCustomerId))
            {
                CustomerMaster cartCustomer = await ((App)Application.Current).QueryService.GetCartCustomerInfoAsync(Convert.ToInt32(AppReference.SelectedCustomerId));
                if (cartCustomer.CustomerID > 0)
                    result = _accountType != cartCustomer.AccountType;
            }
            return result;
        }
        private async Task<bool> IsPrevious_X_CustomerNumberAsync()
        {
            bool result = false;
            if (_accountType != 0 && !string.IsNullOrWhiteSpace(AppReference.SelectedCustomerId))
            {
                CustomerMaster cartCustomer = await ((App)Application.Current).QueryService.GetCartCustomerInfoAsync(Convert.ToInt32(AppReference.SelectedCustomerId));
                if (cartCustomer.CustomerID > 0)
                    result = !string.IsNullOrWhiteSpace(cartCustomer.CustomerNumber)
                        && cartCustomer.CustomerNumber.StartsWith("x", StringComparison.OrdinalIgnoreCase);
            }
            return result;
        }

        private async void PlaceAnOrderButtonClicked(object sender, RoutedEventArgs e)
        {
            bool isOrderDiff = false;
            if (AppReference.CartItemCount > 0)
            {
                if (AppReference.SelectedCustomerId != CustomerId.ToString()) (Application.Current as App).OrderPrintName = "";

                switch (AppReference.CartDataFromScreen)
                {
                    case 0:
                        if (AppReference.SelectedCustomerId != CustomerId.ToString())
                        {
                            //from credit to in-direct customer   // distributor to retail
                            if (_accountType != 1 && ((AppReference.IsCreditRequestOrder ?? false) || (await IsSelectedCustomerAccountTypeDiff())))
                            {
                                await EmptyCart(isOrderDiff = true, "Empty Current cart and continue with the Retail Sale Order?",
                                    () => AppReference.IsCreditRequestOrder = false);
                            }
                            //retail to distributor, not current should be x-customer
                            else if ((await IsSelectedCustomerAccountTypeDiff() && _accountType == 1) && !(AppReference.IsCreditRequestOrder ?? false)
                                && !AccountNumberTextBlock.Text.StartsWith("x", StringComparison.OrdinalIgnoreCase))
                            {
                                await EmptyCart(isOrderDiff = true, "Empty Current cart and continue with the Distributor Order?",
                                   () => AppReference.IsCreditRequestOrder = false);
                            }
                            //direct x-customer to  direct non x-customer
                            else if ((await IsPrevious_X_CustomerNumberAsync() && _accountType == 1) && !(AppReference.IsCreditRequestOrder ?? false)
                                && !AccountNumberTextBlock.Text.StartsWith("x", StringComparison.OrdinalIgnoreCase))
                            {
                                await EmptyCart(isOrderDiff = true, "Empty Current cart and continue with the Distributor Order?",
                                   () => AppReference.IsCreditRequestOrder = false);
                            }
                            //direct but not x-customer  to x-customer
                            else if (!(AppReference.IsCreditRequestOrder ?? false) && AccountNumberTextBlock.Text.StartsWith("x", StringComparison.OrdinalIgnoreCase))
                            {
                                if (!(await IsPrevious_X_CustomerNumberAsync()))
                                    await EmptyCart(isOrderDiff = true, "Empty Current cart and continue with the Sample Order?",
                                       () => { AppReference.IsCreditRequestOrder = false; AppReference.IsDistributionOptionClicked = false; });
                            }
                            // credit to direct, not current should be x-customer
                            else if (_accountType == 1 && (AppReference.IsCreditRequestOrder ?? false)
                                && !AccountNumberTextBlock.Text.StartsWith("x", StringComparison.OrdinalIgnoreCase))
                            {
                                await EmptyCart(isOrderDiff = true, "Empty Current cart and continue with the Distributor Order?",
                                   () => { AppReference.IsCreditRequestOrder = false; AppReference.IsDistributionOptionClicked = false; });
                            }
                            //retail to credit
                            //else if ((await IsSelectedCustomerAccountTypeDiff() && _accountType == 1) && (AppReference.IsCreditRequestOrder ?? false))
                            //{
                            //    await EmptyCart(isOrderDiff=true, "Empty Current cart and continue with the Credit Request ?",
                            //       () => { AppReference.IsCreditRequestOrder = false; AppReference.IsDistributionOptionClicked = false; });
                            //}
                        }
                        break;
                    case 1:
                    case 2:
                        await EmptyCart(isOrderDiff = true, $"Empty Current cart and continue with the {(_accountType != 1 ? "Retail Sale" : "Distributor")} Order?",
                     () => { AppReference.IsCreditRequestOrder = false; AppReference.IsDistributionOptionClicked = false; });
                        break;

                    default:
                        break;
                }
            }

            if (!isOrderDiff)
            {
                AppReference.SelectedCustomerId = CustomerId.ToString();
                NavigationService.NavigateShellFrame(typeof(SRCProductPage));
            }
        }


        private async Task EmptyCart(bool isOrderDiff, string msg, Action fn)
        {
            if (isOrderDiff)
            {
                bool result = await AlertHelper.Instance.ShowConfirmationAlert("", msg, "YES", "NO");
                if (result)
                {
                    var isSuccess = await ((App)Application.Current).QueryService.DeleteAllCartItems(AppReference.CurrentDeviceOrderId);
                    if (isSuccess)
                    {
                        AppReference.CartItemCount = 0;
                        AppReference.SelectedCustomerId = CustomerId.ToString();
                        (Application.Current as App).OrderPrintName = "";
                        AppReference.IsCarStockOrder = false;
                        fn();
                        NavigationService.NavigateShellFrame(typeof(SRCProductPage));
                    }
                }
            }
        }

        private void AccountNameTextBlock_Click(Hyperlink sender, RoutedEventArgs args)
        {
            NavigationService.NavigateShellFrame(typeof(CustomerDetailsPage), CustomerId);
        }

        private async void DriveDirection_ButtonClicked(object sender, RoutedEventArgs e)
        {
            var uri = string.Format("https://www.google.com/maps/dir/?api=1&origin=&destination={0},{1}", Latitude, Longitude);
            await Windows.System.Launcher.LaunchUriAsync(new Uri(uri));
        }

        private async void ShareAccount_ButtonClicked(object sender, RoutedEventArgs e)
        {
            var accountDetails = GetAccountInfoInHtmlFormat();
            var emailModel = new EmailModel() { BodyHtml = accountDetails };
            await EmailService.Instance.SendMailFromOutlook(emailModel);
        }

        private string GetAccountInfoInHtmlFormat()
        {
            StringBuilder stringBuilder = new StringBuilder();

            try
            {
                stringBuilder.Append("<h1>Account Profile Information</h1>");
                stringBuilder.Append(string.Format("<b>Account Name :</b>{0}<br>", AccountNameTextBlock.Text));
                stringBuilder.Append(string.Format("<b>Account Number :</b>{0}<br>", AccountNumberTextBlock.Text));
                stringBuilder.Append(string.Format("<b>Full Address :</b> {0}<br>", AddressTextBlock.Text));
                stringBuilder.Append(string.Format("<b>Phone Number :</b> {0}<br>", PhoneTextBlock.Text));
                stringBuilder.Append(string.Format("<b>Last Call Date :</b> {0}<br>", LastCallDateTextBlock.Text));
                stringBuilder.Append(string.Format("<b>Manager Name :</b> {0}<br><br>", ManagerNameTextBlock.Text));

                if (LastThreeCallActivity.Visibility == Visibility.Visible)
                {
                    stringBuilder.Append("<b>Last Three Call Activity Date :</b> <br>");

                    if (LastThreeCallActivityList.ItemsSource is IList)
                    {
                        var itemSource = LastThreeCallActivityList.ItemsSource as IList;
                        for (int i = 0; i < itemSource.Count; i++)
                        {
                            if (itemSource[i] is ActivityForAllCustomerUIModel)
                            {
                                var obj = itemSource[i] as ActivityForAllCustomerUIModel;
                                stringBuilder.Append(string.Format("<b>{0}.</b></br>", (i + 1)));
                                stringBuilder.Append(string.Format("Activity Type: {0}<br>", obj?.ActivityType));
                                stringBuilder.Append(string.Format("Last Call Activity Date: {0}</br>", obj?.DisplayCallDate));
                                stringBuilder.Append(string.Format("Territory Name: {0}</br>", obj?.TerritoryName ?? "<br>"));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(MapPinCustomPopUp), nameof(GetAccountInfoInHtmlFormat), ex.StackTrace);
            }
            return stringBuilder.ToString();
        }

        private void RouteListAddButton_Clicked(object sender, RoutedEventArgs e)
        {
            if (sender is Button)
            {
                var context = (sender as Button).DataContext as MapPopAddToRouteListModel;
                var index = RouteList.IndexOf(context);
                if (index >= 0)
                {
                    RouteList[index].IsSelected = !RouteList[index].IsSelected;
                }
            }
        }

        private void ClosePopupClicked(object sender, RoutedEventArgs e)
        {
            AddtoRouteFlyout.Hide();
        }

        private async void RouteList_DoneClicked(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            var selectedRoute = RouteList.Where(x => x.IsSelected);
            if (selectedRoute?.Any() == true)
            {
                var loggedInUser = await AppReference.QueryService.GetUserData(AppReference.LoginUserNameProperty, AppReference.LoginUserPinProperty);
                foreach (var item in selectedRoute)
                {
                    await AppReference.QueryService.AddRouteStationsToDb(new System.Collections.Generic.List<int>() { CustomerId }, item.RouteId, item.DeviceRouteId, loggedInUser.UserId);
                }
                await AlertHelper.Instance.ShowConfirmationAlert("Success", "Route Added Successfully", "OK");
                AddtoRouteFlyout.Hide();
            }
        }

        private void ClosePreviewIcon_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            CloseCommad?.Execute(null);
        }

        private void Copy_ButtonClick(object sender, RoutedEventArgs e)
        {
            string content = GetAccountInfoInStringFormat();
            var dataPackage = new Windows.ApplicationModel.DataTransfer.DataPackage();
            dataPackage.SetText(content);
            Windows.ApplicationModel.DataTransfer.Clipboard.SetContent(dataPackage);
        }

        private string GetAccountInfoInStringFormat()
        {
            StringBuilder stringBuilder = new StringBuilder();

            try
            {
                stringBuilder.Append("Account Profile Information\n");
                stringBuilder.Append(string.Format("Account Name : {0}\n", AccountNameTextBlock.Text));
                stringBuilder.Append(string.Format("Account Number :{0}\n", AccountNumberTextBlock.Text));
                stringBuilder.Append(string.Format("Full Address : {0}\n", AddressTextBlock.Text));
                stringBuilder.Append(string.Format("Phone Number : {0}\n", PhoneTextBlock.Text));
                stringBuilder.Append(string.Format("Last Call Date : {0}\n", LastCallDateTextBlock.Text));
                stringBuilder.Append(string.Format("Manager Name : {0}\n\n", ManagerNameTextBlock.Text));

                if (LastThreeCallActivity.Visibility == Visibility.Visible)
                {
                    stringBuilder.Append("Last Three Call Activity Date :\n");

                    if (LastThreeCallActivityList.ItemsSource is IList)
                    {
                        var itemSource = LastThreeCallActivityList.ItemsSource as IList;
                        for (int i = 0; i < itemSource.Count; i++)
                        {
                            if (itemSource[i] is ActivityForAllCustomerUIModel)
                            {
                                var obj = itemSource[i] as ActivityForAllCustomerUIModel;
                                stringBuilder.Append(string.Format("{0}.\n", (i + 1)));
                                stringBuilder.Append(string.Format("Activity Type: {0}\n", obj?.ActivityType));
                                stringBuilder.Append(string.Format("Last Call Activity Date: {0}\n", obj?.DisplayCallDate));
                                stringBuilder.Append(string.Format("Territory Name: {0}\n", obj?.TerritoryName ?? "\n"));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(MapPinCustomPopUp), nameof(GetAccountInfoInHtmlFormat), ex.StackTrace);
            }
            return stringBuilder.ToString();
        }
    }
}
