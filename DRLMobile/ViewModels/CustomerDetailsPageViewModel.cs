using DRLMobile.Converters;
using DRLMobile.Core.Enums;
using DRLMobile.Core.Helpers;
using DRLMobile.Core.Models.DataModels;
using DRLMobile.Core.Models.UIModels;
using DRLMobile.ExceptionHandler;
using DRLMobile.Helpers;
using DRLMobile.Services;
using DRLMobile.Views;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace DRLMobile.ViewModels
{
    public class CustomerDetailsPageViewModel : ObservableObject
    {
        #region properties

        private readonly App AppReference = (App)(Application.Current);

        private CustomerDetailScreenUIModel _uiModel;
        public CustomerDetailScreenUIModel UiModel
        {
            get { return _uiModel; }
            set { SetProperty(ref _uiModel, value); }
        }

        private bool _previewDocumentVisibility;
        public bool PreviewDocumentVisibility
        {
            get { return _previewDocumentVisibility; }
            set { SetProperty(ref _previewDocumentVisibility, value); }
        }


        private bool _addDocumentVisibility;
        public bool AddDocumentVisibility
        {
            get { return _addDocumentVisibility; }
            set { SetProperty(ref _addDocumentVisibility, value); }
        }


        private string _previewUrl;
        public string PreviewUrl
        {
            get { return _previewUrl; }
            set { SetProperty(ref _previewUrl, value); }
        }

        public event EventHandler<CustomerDetailFlyoutType> FlyoutEvent;

        #endregion

        #region command
        public ICommand OnNavigatedToCommand { get; private set; }
        public ICommand OnNavigatingFromCommand { get; private set; }
        public ICommand SelectDristributonTappedCommand { get; private set; }
        public ICommand EditSaveCommand { get; private set; }
        public ICommand DistributorCancelCommand { get; private set; }
        public ICommand DistributorDoneCommand { get; private set; }
        public ICommand CancelDistributorCommand { get; private set; }
        public ICommand PlaceAnOderButtonCommand { get; private set; }
        public ICommand CreditRequestButtonCommand { get; private set; }
        public ICommand AddDocumentCommand { get; private set; }
        public ICommand PreviewDocumentCommand { get; private set; }
        public ICommand OrderHistoryButtonCommand { get; private set; }
        public ICommand DistributorButtonCommand { get; private set; }
        public ICommand RackOrderButtonCommand { get; private set; }

        #endregion

        #region constructor
        public CustomerDetailsPageViewModel()
        {
            UiModel = new CustomerDetailScreenUIModel();
            EditSaveCommand = new AsyncRelayCommand(EditSaveCommandHandler);
            OnNavigatingFromCommand = new RelayCommand(OnNavigatingFromCommandHandler);
            OnNavigatedToCommand = new AsyncRelayCommand<int>(OnNavigatedToCommandHandler);
            SelectDristributonTappedCommand = new RelayCommand(SelectDristributonTappedCommandHandler);
            DistributorCancelCommand = new RelayCommand(DistributorCancelCommandHandler);
            DistributorDoneCommand = new RelayCommand(DistributorDoneCommandHandler);
            PlaceAnOderButtonCommand = new AsyncRelayCommand(PlaceAnOderButtonCommandHandler);
            CancelDistributorCommand = new RelayCommand(CancelDistributorCommandHandler);
            AddDocumentCommand = new AsyncRelayCommand(AddDocumentCommandHandler);
            PreviewDocumentCommand = new RelayCommand<string>(PreviewDocumentCommandHandler);
            OrderHistoryButtonCommand = new RelayCommand(OrderHistoryButtonCommandHandler);
            CreditRequestButtonCommand = new RelayCommand(CreditRequestButtonCommandHandler);
            DistributorButtonCommand = new RelayCommand(DistributorButtonCommandHandler);
            RackOrderButtonCommand = new AsyncRelayCommand(RackOrderButtonCommandHandler);

            AddDocumentVisibility = false;
            PreviewDocumentVisibility = false;
        }

        #endregion


        #region private methods
        private void PreviewDocumentCommandHandler(string url)
        {
            if (!string.IsNullOrEmpty(url))
            {
                PreviewUrl = null;
                PreviewUrl = url;
                PreviewDocumentVisibility = true;
            }
            else
                PreviewDocumentVisibility = false;

        }

        private async Task AddDocumentCommandHandler()
        {
            UiModel.CustomerDocuments = await AppReference.QueryService.GetCustomerDocuments(UiModel.CustomerData.CustomerID.Value);
            AddDocumentVisibility = !AddDocumentVisibility;
        }

        private void CancelDistributorCommandHandler()
        {
            FlyoutEvent?.Invoke(this, CustomerDetailFlyoutType.Close);
        }

        private async Task PlaceAnOderButtonCommandHandler()
        {
            AppReference.SelectedCustomerId = UiModel?.CustomerData?.CustomerID.ToString();

            if (AppReference.CurrentOrderId != 0 && (bool)AppReference.IsCreditRequestOrder)
            {
                AppReference.IsOrderTypeChanged = true;
            }
            else
            {
                AppReference.IsOrderTypeChanged = false;
            }

            if ((bool)AppReference.IsCreditRequestOrder)
            {
                AppReference.IsCreditRequestOrder = false;
            }
            
            if (AppReference.CartItemCount > 0)
            {
                if (AppReference.CartDataFromScreen != 0)
                {
                    var result = await AlertHelper.Instance.ShowConfirmationAlert("", ResourceExtensions.GetLocalized("EmptyRackOrPopCart"), "YES", "NO");
                    if (result)
                    {
                        var isSuccess = await ((App)Application.Current).QueryService.DeleteAllCartItems(AppReference.CurrentOrderId);
                        if (isSuccess)
                        {
                            AppReference.CartItemCount = 0;
                            NavigationService.Navigate<SRCProductPage>();
                        }
                    }
                }
                else
                {
                    NavigationService.Navigate<SRCProductPage>();

                }

            }
            else if (AppReference.CartItemCount == 0)
            {
                NavigationService.Navigate<SRCProductPage>();

            }
        }
        private async Task RackOrderButtonCommandHandler()
        {
            AppReference.SelectedCustomerId = UiModel?.CustomerData?.CustomerID.ToString();
            //var productIdDictionary = await AppReference.QueryService.GetCartItemsCount(AppReference.CurrentOrderId.ToString());
            //if (productIdDictionary.Count > 0)
            if (AppReference.CartItemCount > 0)
            {
                if (AppReference.CartDataFromScreen != 1)
                {
                    var result = await AlertHelper.Instance.ShowConfirmationAlert("", ResourceExtensions.GetLocalized("EmptySrcOrPopCart"), "YES", "NO");
                    if (result)
                    {
                        var isSuccess = await ((App)Application.Current).QueryService.DeleteAllCartItems(AppReference.CurrentOrderId);
                        if (isSuccess)
                        {
                            AppReference.CartItemCount = 0;
                            NavigationService.Navigate<RackOrderListPage>();
                        }
                    }
                }
                else
                {
                    NavigationService.Navigate<RackOrderListPage>();
                }

            }
            else if (AppReference.CartItemCount == 0)
            {
                NavigationService.Navigate<RackOrderListPage>();

            }
        }
        
        private void CreditRequestButtonCommandHandler()
        {
            AppReference.SelectedCustomerId = UiModel?.CustomerData?.CustomerID.ToString();

            AppReference.IsCreditRequestOrder = true;

            if (AppReference.CurrentOrderId != 0)
            {
                AppReference.IsOrderTypeChanged = true;
            }
            else
            {
                AppReference.IsOrderTypeChanged = false;
            }

            NavigationService.Navigate<SRCProductPage>();
        }

        private void DistributorButtonCommandHandler()
        {
            AppReference.SelectedCustomerId = UiModel?.CustomerData?.CustomerID.ToString();

            NavigationService.Navigate<SRCProductPage>();
        }

        private async Task EditSaveCommandHandler()
        {
            try
            {
                CustomerDetailFlyoutType type;
                if (UiModel.IsInEditMode)
                {
                    if (UiModel.CustomerData.AccountType == 2)
                    {
                        //indirect customer
                        var isValid = ValidateMandatoryFields(out type);
                        if (isValid)
                        {
                            UiModel.IsInEditMode = !UiModel.IsInEditMode;
                            await Task.Delay(200);
                            await SaveIndirectCustomer();
                            await ShowSucessMsg("Success", "Customer has been updated successfully to your device");
                        }
                        else
                        {
                            FlyoutEvent?.Invoke(this, type);
                        }
                    }
                    else
                    {
                        UiModel.SetContactForUpdate();
                        bool isAllContactValids = ValidateContactsForDirectCustomer(out type);
                        if (isAllContactValids)
                        {
                            UiModel.IsInEditMode = !UiModel.IsInEditMode;
                            await Task.Delay(300);
                            await SaveContact();
                            await ShowSucessMsg("Success", "Contact Saved Successfully");
                        }
                        else
                        {
                            FlyoutEvent?.Invoke(this, type);
                        }
                    }
                }
                else
                {
                    UiModel.IsInEditMode = !UiModel.IsInEditMode;
                    UiModel.SetMailingEditable();
                    UiModel.SetShippingEditable();
                    UiModel.HandleContactEditForDirectCustomer();
                }
            }
            catch (Exception ex)
            {
                await ShowSucessMsg("Error", "Something went wrong");
                ErrorHandler.LogException(nameof(CustomerDetailsPageViewModel), nameof(EditSaveCommandHandler), ex.Message);
            }

        }

        private bool ValidateContactsForDirectCustomer(out CustomerDetailFlyoutType type)
        {
            type = CustomerDetailFlyoutType.None;
            if (UiModel.ContactListItemSource.Count > 0)
            {
                foreach (var contact in UiModel.ContactListItemSource)
                {
                    if (string.IsNullOrEmpty(contact.ContactEmail) && string.IsNullOrEmpty(contact.SelectedRank) && string.IsNullOrEmpty(contact.ContactName) && string.IsNullOrEmpty(contact.ContactPhone))
                    {
                        type = CustomerDetailFlyoutType.ContactAllMandatoryFeilds;
                        return false;
                    }
                    else if (string.IsNullOrEmpty(contact.ContactName))
                    {
                        type = CustomerDetailFlyoutType.ContactName;
                        return false;
                    }
                    else if (string.IsNullOrEmpty(contact.ContactEmail))
                    {
                        type = CustomerDetailFlyoutType.ContactEmail;
                        return false;
                    }
                    else if (string.IsNullOrEmpty(contact.ContactPhone))
                    {
                        type = CustomerDetailFlyoutType.ContactPhone;
                        return false;
                    }
                    else if (string.IsNullOrEmpty(contact.SelectedRank))
                    {
                        type = CustomerDetailFlyoutType.ContactRank;
                        return false;
                    }



                    var isPhoneValid = Constants.Constants.PhoneNumbRegex.Match((string)new StringToPhoneNumberConverter().Convert(contact.ContactPhone, typeof(string), null, null));
                    if (!isPhoneValid.Success)
                    {
                        type = CustomerDetailFlyoutType.InvalidContactPhone;
                        return false;
                    }
                    if (!string.IsNullOrWhiteSpace(contact.ContactFax))
                    {
                        var isFaxValid = Constants.Constants.PhoneNumbRegex.Match((string)new StringToPhoneNumberConverter().Convert(contact.ContactFax, typeof(string), null, null));
                        if (!isFaxValid.Success)
                        {
                            type = CustomerDetailFlyoutType.InvalidContactFax;
                            return false;
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(contact.ContactEmail) && !contact.IsEmailValid)
                    {
                        type = CustomerDetailFlyoutType.InValidContactEmail;
                        return false;
                    }

                }
                return true;
            }
            else
                return true;
        }

        private bool ValidateMandatoryFields(out CustomerDetailFlyoutType type)
        {
            type = CustomerDetailFlyoutType.None;
            if (string.IsNullOrWhiteSpace(UiModel.CustomerName))
            {
                type = CustomerDetailFlyoutType.CustomerName;
                return false;
            }
            else if (UiModel.SelectedAccountClassification == null)
            {
                type = CustomerDetailFlyoutType.AccountClassification;
                return false;
            }
            else if (string.IsNullOrWhiteSpace(UiModel.SelectedRank))
            {
                type = CustomerDetailFlyoutType.Rank;
                return false;
            }
            else if (string.IsNullOrWhiteSpace(UiModel.PhysicalAddress))
            {
                type = CustomerDetailFlyoutType.Address;
                return false;
            }
            else if (string.IsNullOrWhiteSpace(UiModel.PhysicalCity))
            {
                type = CustomerDetailFlyoutType.City;
                return false;
            }
            else if (string.IsNullOrWhiteSpace(UiModel.SelectedPhysicalState))
            {
                type = CustomerDetailFlyoutType.State;
                return false;
            }
            else if (string.IsNullOrWhiteSpace(UiModel.PhysicalZip))
            {
                type = CustomerDetailFlyoutType.Zip;
                return false;
            }


            if (!string.IsNullOrWhiteSpace(UiModel.PhysicalContactEmail) && !UiModel.IsEmailValid)
            {
                type = CustomerDetailFlyoutType.InvalidEmail;
                return false;
            }


            var isPhoneValid = Constants.Constants.PhoneNumbRegex.Match((string)new StringToPhoneNumberConverter().Convert(UiModel.PhysicalContactPhone, typeof(string), null, null));
            if (!isPhoneValid.Success && !string.IsNullOrWhiteSpace(UiModel.PhysicalContactPhone))
            {
                type = CustomerDetailFlyoutType.InvalidPhone;
                return false;
            }


            var isFaxValid = Constants.Constants.PhoneNumbRegex.Match((string)new StringToPhoneNumberConverter().Convert(UiModel.PhysicalContactFax, typeof(string), null, null));
            if (!isFaxValid.Success && !string.IsNullOrWhiteSpace(UiModel.PhysicalContactFax))
            {
                type = CustomerDetailFlyoutType.InvalidFax;
                return false;
            }

            return true;
        }


        private async Task ShowSucessMsg(string title, string msg)
        {
            ContentDialog userLogoutDialog = new ContentDialog
            {
                Title = title,
                Content = msg,
                PrimaryButtonText = ResourceExtensions.GetLocalized("OK"),
                SecondaryButtonText = string.Empty
            };

            await userLogoutDialog.ShowAsync();
            userLogoutDialog.Hide();
        }


        private async Task SaveContact()
        {
            List<ContactMaster> deletedRecords = UiModel.GetDeletedContactRecords();
            await AppReference.QueryService.InsertOrUpdateContactForCustomerProfilePage(deletedRecords: deletedRecords,
                updatedRecords: UiModel.ContactListItemSource, userName: AppReference.LoginUserNameProperty,
                pin: AppReference.LoginUserPinProperty,
                selectedCustomerId: UiModel.CustomerData.CustomerID.ToString());

        }

        private async Task SaveIndirectCustomer()
        {
            UiModel.SetUpdateCustomerObject();
            await AppReference.QueryService.UpdateOrInsertCustomerData(UiModel.CustomerData, AppReference.LoginUserNameProperty, AppReference.LoginUserPinProperty);

            List<DistributorMaster> deletedRecords = UiModel.GetDeletedDristributorRecords();

            if (UiModel.DistributorList != null)
            {
                await AppReference.QueryService.InsertOrUpdateDistributorForCustomerProfilePage(deletedRecords: deletedRecords,
                                                updadtedRecords: UiModel.DistributorList,
                                                userName: AppReference.LoginUserNameProperty,
                                                pin: AppReference.LoginUserPinProperty,
                                                selectedCustomerDeviceId: UiModel.CustomerData.DeviceCustomerID);
            }
        }

        private async Task OnNavigatedToCommandHandler(int customerId)
        {
            UiModel = await AppReference.QueryService.GetCustomerDetailsDataForViewAndEditAsync(customerId);
        }

        private void OnNavigatingFromCommandHandler()
        {
            throw new NotImplementedException();
        }

        private void SelectDristributonTappedCommandHandler()
        {
            if (UiModel.IsInEditMode)
            {
                FlyoutEvent?.Invoke(this, CustomerDetailFlyoutType.DistributorFlyout);
            }
        }

        private void DistributorCancelCommandHandler()
        {
            FlyoutEvent?.Invoke(this, CustomerDetailFlyoutType.Close);
        }


        private void DistributorDoneCommandHandler()
        {
            UiModel.DistributorDoneCommandHandler();
            FlyoutEvent?.Invoke(this, CustomerDetailFlyoutType.Close);
        }

        private void OrderHistoryButtonCommandHandler()
        {
            NavigationService.Navigate<OrderHistoryPage>(UiModel.CustomerData);
        }

        #endregion


    }
}
