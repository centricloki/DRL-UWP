using DRLMobile.Converters;
using DRLMobile.Core.Enums;
using DRLMobile.Core.Models.DataModels;
using DRLMobile.Core.Models.UIModels;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using DRLMobile.Core.Helpers;
using DRLMobile.Helpers;
using DRLMobile.Views;

namespace DRLMobile.ViewModels
{
    public class AddCustomerPageViewModel : ObservableObject
    {
        private readonly App AppReference = (App)(Application.Current);
        public event EventHandler<CustomerDetailFlyoutType> FlyoutEvent;
        public event EventHandler<bool> GoBackEvent;

        #region command
        public ICommand FlyoutTextChangedCommand { get; private set; }
        public ICommand ShippingAddressCheckBoxCommand { get; private set; }
        public ICommand MailingAddressCheckBoxCommand { get; private set; }
        public ICommand DistributorDoneCommand { get; private set; }
        public ICommand SelectDristributonTappedCommand { get; private set; }
        public ICommand DistributionSelectionCommand { get; private set; }
        public ICommand NavigatedToCommand { get; private set; }
        public ICommand DeleteDistributorButtonCommand { get; private set; }
        public ICommand SaveButtonCommand { get; private set; }
        public ICommand CancelDistributorCommand { get; private set; }

        #endregion

        private AddCustomerUiModel _uiModel;

        public AddCustomerUiModel UiModel
        {
            get { return _uiModel; }
            set { SetProperty(ref _uiModel, value); }
        }


        #region input validation flags

        private bool _isEmailValid;
        public bool IsEmailValid
        {
            get { return _isEmailValid; }
            set { SetProperty(ref _isEmailValid, value); }
        }


        #endregion


        public AddCustomerPageViewModel()
        {
            FlyoutTextChangedCommand = new RelayCommand<string>(FlyoutTextChangedCommandHandler);
            NavigatedToCommand = new AsyncRelayCommand(NavigatedToCommandHandler);
            DistributorDoneCommand = new RelayCommand(DistributorDoneCommandHandler);
            CancelDistributorCommand = new RelayCommand(CancelDistributorCommandHandler);
            DeleteDistributorButtonCommand = new RelayCommand<DistributorMaster>(DeleteDistributorButtonCommandHandler);
            DistributionSelectionCommand = new RelayCommand<DistributorMaster>(DistributionSelectionCommandHandler);
            ShippingAddressCheckBoxCommand = new RelayCommand<bool>(ShippingAddressCheckBoxCommandHandler);
            MailingAddressCheckBoxCommand = new RelayCommand<bool>(MailingAddressCheckBoxCommandHandler);
            SaveButtonCommand = new AsyncRelayCommand(SaveButtonCommandHandler);
        }

        private void CancelDistributorCommandHandler()
        {
            FlyoutEvent?.Invoke(this, CustomerDetailFlyoutType.Close);
        }

        private async Task SaveButtonCommandHandler()
        {
            CustomerDetailFlyoutType type;
            var isValid = ValidateMandatoryFields(out type);
            if (isValid)
            {
                CustomerMaster data = SetCustomerMasterModel();
                var result = await AppReference.QueryService.AddNewCustomerDataToDatabase(data, UiModel.DistributorList, UiModel.LoggedInUser);
                if (result != null)
                {
                    await ShowSucessMsg(ResourceExtensions.GetLocalized("SUCCESS"), ResourceExtensions.GetLocalized("ADD_CUSTOMER_SUCCESS_MSG"));
                    CustomerPage.NewlyAddedCustomer = result;
                    GoBackEvent?.Invoke(this, true);
                }
                else
                {
                    await AlertHelper.Instance.ShowConfirmationAlert(ResourceExtensions.GetLocalized("ERROR"), ResourceExtensions.GetLocalized("SomethingWentWrong_Error_Msg"),ResourceExtensions.GetLocalized("OK"));
                }
            }
            else
            {
                FlyoutEvent?.Invoke(this, type);
            }
        }

        private CustomerMaster SetCustomerMasterModel()
        {
            var customer = new CustomerMaster()
            {
                CustomerName = UiModel?.CustomerName,
                AccountType = 2,
                AccountClassification = UiModel?.SelectedAccountClassification.AccountClassificationId.ToString(),
                Rank = UiModel?.SelectedRank,
                PhysicalAddress = UiModel?.PhysicalAddress,
                PhysicalAddressCityID = UiModel?.PhysicalCity,
                PhysicalAddressStateID = HelperMethods.GetKeyFromIdNameDictionary(UiModel.States, UiModel.SelectedPhysicalState),
                PhysicalAddressZipCode = UiModel?.PhysicalZip,
                ContactName = UiModel?.PhysicalContactPersonName,
                ContactRole = UiModel?.PhysicalContactPersonRole,
                ContactEmail = UiModel?.PhysicalContactEmail,
                Phone = UiModel?.PhysicalContactPhone,
                Fax = UiModel?.PhysicalContactFax,
                ShippingAddress = UiModel?.ShippingAddress,
                ShippingAddressCityID = UiModel?.ShippingCity,
                ShippingAddressStateID = HelperMethods.GetKeyFromIdNameDictionary(UiModel.States, UiModel.SelectedShippingState),
                ShippingAddressZipCode = UiModel?.ShippingZip,
                MailingAddress = UiModel?.MailingAddress,
                MailingAddressCityID = UiModel?.MailingCity,
                MailingAddressStateID = HelperMethods.GetKeyFromIdNameDictionary(UiModel.States, UiModel.SelectedMailingState),
                MailingAddressZipID = UiModel?.MailingZip,
                GeneralComments = UiModel?.GeneralComments,
                StoreCount = UiModel?.NumberOfLocations,
                StateTobaccoLicense = UiModel?.TobaccoLicense,
                RetailerLicense = UiModel?.RetailLicense,
                RetailerSalesTaxCertificate = UiModel?.SalesTaxCertificate,
                isDeleted = 0,
                IsExported = 0,
                RegionId = UiModel.Region != null ? UiModel.Region.RegionID : 0,
                ZoneId = UiModel.Zone != null ? UiModel.Zone.ZoneID : 0,
                TerritoryID = UiModel?.SelectedTerritory?.TerritoryID.ToString(),
                CreatedBy = UiModel?.LoggedInUser?.UserId.ToString(),
                CreatedDate = DateTimeHelper.ConvertToDbInsertDateTimeFormat(DateTime.Now)
            };
            return customer;
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

            if (!string.IsNullOrWhiteSpace(UiModel.PhysicalContactEmail) && !IsEmailValid)
            {
                type = CustomerDetailFlyoutType.InvalidEmail;
                return false;
            }

            if (!string.IsNullOrWhiteSpace(UiModel?.PhysicalContactPhone))
            {
                var isPhoneValid = Constants.Constants.PhoneNumbRegex.Match((string)new StringToPhoneNumberConverter().Convert(UiModel?.PhysicalContactPhone, typeof(string), null, null));
                if (!isPhoneValid.Success)
                {
                    type = CustomerDetailFlyoutType.InvalidPhone;
                    return false;
                }
            }
            if (!string.IsNullOrWhiteSpace(UiModel?.PhysicalContactFax))
            {
                var isFaxValid = Constants.Constants.PhoneNumbRegex.Match((string)new StringToPhoneNumberConverter().Convert(UiModel?.PhysicalContactFax, typeof(string), null, null));
                if (!isFaxValid.Success)
                {
                    type = CustomerDetailFlyoutType.InvalidFax;
                    return false;
                }
            }

            return true;
        }

        private async Task NavigatedToCommandHandler()
        {
            UiModel = await AppReference.QueryService.GetAddCustomerPageData(AppReference.LoginUserNameProperty, AppReference.LoginUserPinProperty);
        }

        private void FlyoutTextChangedCommandHandler(string text)
        {
            if (string.IsNullOrWhiteSpace(text) || text.Length < 3)
                UiModel.DistributionFlyoutListItemSource.Clear();
            else
            {
                var tempList = UiModel.MainDistributorList.Where(x => x.CustomerName.ToLower().Contains(text.ToLower())).ToList();
                tempList.ForEach(x =>
                {
                    if (UiModel.DistributorList.Count > 0 && UiModel.DistributorList.Any(y => y.CustomerID == x.CustomerID))
                    {
                        x.IsSelected = true;
                    }
                });
                UiModel.DistributionFlyoutListItemSource = new ObservableCollection<DistributorMaster>(tempList);
            }
        }


        private void DistributionSelectionCommandHandler(DistributorMaster obj)
        {
            var value = UiModel.DistributionFlyoutListItemSource?.Any(x => x.CustomerID == obj.CustomerID);
            if (value.HasValue && value.Value && obj != null)
            {
                var isSelected = !obj.IsSelected;
                (UiModel.DistributionFlyoutListItemSource?.FirstOrDefault(x => x.CustomerID == obj.CustomerID)).IsSelected = isSelected;
                (UiModel.MainDistributorList?.FirstOrDefault(x => x.CustomerID == obj.CustomerID)).IsSelected = isSelected;
            }
        }

        private void DeleteDistributorButtonCommandHandler(DistributorMaster distributor)
        {
            UiModel.DistributorList?.Remove(distributor);
        }
        private void DistributorDoneCommandHandler()
        {
            var tempList = UiModel.MainDistributorList.Where(x => x.IsSelected);
            UiModel.DistributorList = new ObservableCollection<DistributorMaster>(tempList);
            UiModel.DistributionFlyoutListItemSource.Clear();
            FlyoutEvent?.Invoke(this, CustomerDetailFlyoutType.Close);
        }

        private void MailingAddressCheckBoxCommandHandler(bool isChecked)
        {
            if (isChecked)
            {
                UiModel.MailingAddress = UiModel.PhysicalAddress;
                UiModel.MailingCity = UiModel.PhysicalCity;
                UiModel.SelectedMailingState = UiModel.SelectedPhysicalState;
                UiModel.MailingZip = UiModel.PhysicalZip;
            }
            else
            {
                UiModel.MailingAddress = string.Empty;
                UiModel.MailingCity = string.Empty;
                UiModel.SelectedMailingState = null;
                UiModel.MailingZip = string.Empty;
            }
        }

        private void ShippingAddressCheckBoxCommandHandler(bool isChecked)
        {
            if (isChecked)
            {
                UiModel.ShippingAddress = UiModel.PhysicalAddress;
                UiModel.ShippingCity = UiModel.PhysicalCity;
                UiModel.SelectedShippingState = UiModel.SelectedPhysicalState;
                UiModel.ShippingZip = UiModel.PhysicalZip;
            }
            else
            {
                UiModel.ShippingAddress = string.Empty;
                UiModel.ShippingCity = string.Empty;
                UiModel.SelectedShippingState = null;
                UiModel.ShippingZip = string.Empty;
            }
        }

        private async Task ShowSucessMsg(string title, string msg)
        {
            ContentDialog userLogoutDialog = new ContentDialog
            {
                Title = title,
                Content = msg,
                PrimaryButtonText = Helpers.ResourceExtensions.GetLocalized("OK"),
                SecondaryButtonText = string.Empty
            };

            await userLogoutDialog.ShowAsync();
            userLogoutDialog.Hide();
        }



    }
}
