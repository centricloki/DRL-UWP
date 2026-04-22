using DRLMobile.Core.Enums;
using DRLMobile.Core.Helpers;
using DRLMobile.Core.Models;
using DRLMobile.Core.Models.DataModels;
using DRLMobile.Core.Models.FedExAddressValidationModels;
using DRLMobile.Core.Models.UIModels;
using DRLMobile.ExceptionHandler;
using DRLMobile.Uwp.Converters;
using DRLMobile.Uwp.CustomControls;
using DRLMobile.Uwp.Helpers;
using DRLMobile.Uwp.View;

using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Uwp.UI.Controls;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace DRLMobile.Uwp.ViewModel
{
    public class AddCustomerPageViewModel : BaseModel
    {
        private readonly App AppReference = (App)(Application.Current);
        public event EventHandler<CustomerDetailFlyoutType> FlyoutEvent;
        public event EventHandler<bool> GoBackEvent;
        private ObservableCollection<WeekdayUIModel> _weekdays;
        public ObservableCollection<WeekdayUIModel> Weekdays
        {
            get { return _weekdays; }
            set { SetProperty(ref _weekdays, value); }
        }
        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set { SetProperty(ref _isLoading, value); }
        }
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
            DeleteDistributorButtonCommand = new RelayCommand<DistributorAssignUser>(DeleteDistributorButtonCommandHandler);
            DistributionSelectionCommand = new RelayCommand<DistributorAssignUser>(DistributionSelectionCommandHandler);
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
            ShellPage shellPage = null;
            CustomerDetailFlyoutType type;
            try
            {
                var isValid = ValidateMandatoryFields(out type);

                if (isValid)
                {
                    shellPage = ((Window.Current.Content as Frame).Content as ShellPage);
                    if (shellPage != null)
                    {
                        shellPage.ViewModel.IsSideMenuItemClickable = false;
                    }
                    IsLoading = true;

                    // Run three tasks in parallel
                    Task<FedExAddressContentDialog> fedExValidPhysicalAddress = null;
                    Task<FedExAddressContentDialog> fedExValidShippingAddress = null;
                    Task<FedExAddressContentDialog> fedExValidMailingAddress = null;

                    //validate with fedEx Address Service
                    if (!string.IsNullOrWhiteSpace(UiModel.PhysicalAddress)
                        || !string.IsNullOrWhiteSpace(UiModel.PhysicalCity)
                        || !UiModel.SelectedPhysicalState.Equals("Select State", StringComparison.OrdinalIgnoreCase)
                        || !string.IsNullOrWhiteSpace(UiModel.PhysicalZip))
                    { fedExValidPhysicalAddress = FedExServiceResponseAsync("physical"); }

                    if (!UiModel.IsCheckedForSameAsPhysicalAddressShippingAddess &&
                            (
                            !string.IsNullOrWhiteSpace(UiModel.ShippingAddress)
                            || !string.IsNullOrWhiteSpace(UiModel.ShippingCity)
                            || !string.IsNullOrWhiteSpace(UiModel.SelectedShippingState)
                            || !string.IsNullOrWhiteSpace(UiModel.ShippingZip)
                            )
                        )
                    {
                        fedExValidShippingAddress = FedExServiceResponseAsync("shipping");
                    }

                    if (!UiModel.IsCheckedForSameAsPhysicalAddressMailingAddess &&
                            (
                            !string.IsNullOrWhiteSpace(UiModel.MailingAddress)
                            || !string.IsNullOrWhiteSpace(UiModel.MailingCity)
                            || !string.IsNullOrWhiteSpace(UiModel.SelectedMailingState)
                            || !string.IsNullOrWhiteSpace(UiModel.MailingZip)
                            )
                        )
                    {
                        fedExValidMailingAddress = FedExServiceResponseAsync("mailing");
                    }

                    // Filter out null tasks
                    var taskList = new List<Task<FedExAddressContentDialog>>(3) { fedExValidPhysicalAddress, fedExValidShippingAddress, fedExValidMailingAddress }
                    .Where(t => t != null).Cast<Task<FedExAddressContentDialog>>().ToList();

                    if (taskList.Any()) // Only run if there's at least one task
                    {
                        // Wait for all tasks to complete
                        FedExAddressContentDialog[] results = await System.Threading.Tasks.Task.WhenAll(taskList);
                        foreach (var item in results)
                        {
                            if (item != null)
                            {
                                if (item.IsResolved) await ShowFedExAddressPopupAsync(item);
                                else
                                {
                                    if (item.AddressType.Equals("physical", StringComparison.OrdinalIgnoreCase))
                                    {
                                        await ShowSucessMsg("Incorrect Physical Address", "It seems like you have entered an incorrect address. Please review.");
                                    }
                                    else if (item.AddressType.Equals("shipping", StringComparison.OrdinalIgnoreCase))
                                    {
                                        await ShowSucessMsg("Incorrect Shipping Address", "It seems like you have entered an incorrect address. Please review.");
                                    }
                                    else if (item.AddressType.Equals("mailing", StringComparison.OrdinalIgnoreCase))
                                    {
                                        await ShowSucessMsg("Incorrect Mailing Address", "It seems like you have entered an incorrect address. Please review.");
                                    }
                                }
                            }
                        }
                    }

                    CustomerMaster data = SetCustomerMasterModel();

                    IList<Int16> selectedWeekDays = Weekdays.Where(x => x.IsSelected).Select(x => x.Id).ToList();
                    if (selectedWeekDays.Any())
                    {
                        data.OrderDeliveryWeekDays = string.Join(',', selectedWeekDays);
                    }
                    else data.OrderDeliveryWeekDays = null;

                    var result = await AppReference.QueryService.AddNewCustomerDataToDatabase(data, UiModel.DistributorList, UiModel.LoggedInUser);

                    if (result != null)
                    {
                        await ShowSucessMsg(ResourceExtensions.GetLocalized("SUCCESS"), ResourceExtensions.GetLocalized("ADD_CUSTOMER_SUCCESS_MSG"));

                        CustomersListPage.NewlyAddedCustomer = result;

                        GoBackEvent?.Invoke(this, true);
                    }
                    else
                    {
                        await AlertHelper.Instance.ShowConfirmationAlert(ResourceExtensions.GetLocalized("ERROR"), ResourceExtensions.GetLocalized("SomethingWentWrong_Error_Msg"), ResourceExtensions.GetLocalized("OK"));
                    }
                }
                else
                {
                    FlyoutEvent?.Invoke(this, type);
                }

            }
            catch (Exception ex)
            {
                await ShowSucessMsg("Error", "Something went wrong, please try again after some time");

                ErrorLogger.WriteToErrorLog(nameof(CustomerDetailsPageViewModel), nameof(SaveButtonCommandHandler), ex);
            }
            finally
            {
                IsLoading = false;
                if (shellPage != null)
                {
                    shellPage.ViewModel.IsSideMenuItemClickable = true;
                }
            }
        }
        private async Task<FedExAddressContentDialog> FedExServiceResponseAsync(string addressType)
        {
            FedExAddressContentDialog enteredAddress = null;
            FedExAddressContentDialog suggestedAddress = null;

            if (addressType.Equals("physical", StringComparison.OrdinalIgnoreCase))
            {
                enteredAddress = new FedExAddressContentDialog
                {
                    StreetLines = UiModel.PhysicalAddress,
                    City = UiModel.PhysicalCity,
                    StateOrProvinceCode = UiModel.SelectedPhysicalState,
                    PostalCode = UiModel.PhysicalZip
                };
            }
            else if (addressType.Equals("shipping", StringComparison.OrdinalIgnoreCase))
            {
                enteredAddress = new FedExAddressContentDialog
                {
                    StreetLines = string.IsNullOrEmpty(UiModel.ShippingAddress) ? "" : UiModel.ShippingAddress,
                    City = string.IsNullOrEmpty(UiModel.ShippingCity) ? "" : UiModel.ShippingCity,
                    StateOrProvinceCode = string.IsNullOrEmpty(UiModel.SelectedShippingState) ? "" : UiModel.SelectedShippingState,
                    PostalCode = string.IsNullOrEmpty(UiModel.ShippingZip) ? "0" : UiModel.ShippingZip,
                };
            }
            else if (addressType.Equals("mailing", StringComparison.OrdinalIgnoreCase))
            {
                enteredAddress = new FedExAddressContentDialog
                {
                    StreetLines = string.IsNullOrEmpty(UiModel.MailingAddress) ? "" : UiModel.MailingAddress,
                    City = string.IsNullOrEmpty(UiModel.MailingCity) ? "" : UiModel.MailingCity,
                    StateOrProvinceCode = string.IsNullOrEmpty(UiModel.SelectedMailingState) ? "" : UiModel.SelectedMailingState,
                    PostalCode = string.IsNullOrEmpty(UiModel.MailingZip) ? "0" : UiModel.MailingZip,
                };
            }

            string requestBody = JsonConvert.SerializeObject(new FedExAddressResolveRequest
            {
                AddressesToValidate = new FedExAddressesToValidate[1] {
           new FedExAddressesToValidate
           {
            Address = new FedExAddress
                {
                    StreetLines = new string[1] { enteredAddress.StreetLines },
                    City=enteredAddress.City,
                    PostalCode=Convert.ToInt64(enteredAddress.PostalCode),
                    StateOrProvinceCode= enteredAddress.StateOrProvinceCode,
                    CountryCode ="US",
                }
           }
        }
            });

            FedExValidatedAddressResponse fedExValidatedAddressResponse =
                await AppReference.QueryService.CallFedExAddressValidationAPIAsync(requestBody);

            if (fedExValidatedAddressResponse != null)
            {
                FedExResolvedAddress fedExAddressResolved = fedExValidatedAddressResponse.Output.ResolvedAddresses.FirstOrDefault();
                if (fedExAddressResolved != null && (fedExAddressResolved.Attributes.Resolved.HasValue && fedExAddressResolved.Attributes.Resolved.Value))
                {
                    suggestedAddress = new FedExAddressContentDialog
                    {
                        StreetLines = ((fedExAddressResolved?.StreetLinesToken.Count() > 1)
                        ? string.Join(',', fedExAddressResolved?.StreetLinesToken)
                        : fedExAddressResolved?.StreetLinesToken.FirstOrDefault()),
                        City = fedExAddressResolved.City,
                        StateOrProvinceCode = fedExAddressResolved.StateOrProvinceCode,
                        PostalCode = Convert.ToString(fedExAddressResolved.ParsedPostalCode.Base),
                        AddressType = addressType,
                        IsResolved = true
                    };
                }
                else
                {
                    suggestedAddress = enteredAddress;
                    suggestedAddress.AddressType = addressType;
                }
            }
            return suggestedAddress;
        }

        private async Task ShowFedExAddressPopupAsync(FedExAddressContentDialog suggestedAddress)
        {
            FedExAddressContentDialog enteredAddress = null;
            if (suggestedAddress.AddressType.Equals("physical", StringComparison.OrdinalIgnoreCase))
            {
                enteredAddress = new FedExAddressContentDialog
                {
                    StreetLines = UiModel.PhysicalAddress,
                    City = UiModel.PhysicalCity,
                    StateOrProvinceCode = UiModel.SelectedPhysicalState,
                    PostalCode = UiModel.PhysicalZip
                };
            }
            else if (suggestedAddress.AddressType.Equals("shipping", StringComparison.OrdinalIgnoreCase))
            {
                enteredAddress = new FedExAddressContentDialog
                {
                    StreetLines = UiModel.ShippingAddress,
                    City = UiModel.ShippingCity,
                    StateOrProvinceCode = string.IsNullOrWhiteSpace(UiModel.SelectedShippingState) ? "" : UiModel.SelectedShippingState,
                    PostalCode = UiModel.ShippingZip
                };
            }
            else if (suggestedAddress.AddressType.Equals("mailing", StringComparison.OrdinalIgnoreCase))
            {
                enteredAddress = new FedExAddressContentDialog
                {
                    StreetLines = UiModel.MailingAddress,
                    City = UiModel.MailingCity,
                    StateOrProvinceCode = string.IsNullOrWhiteSpace(UiModel.SelectedMailingState) ? "" : UiModel.SelectedMailingState,
                    PostalCode = UiModel.MailingZip
                };
            }

            if ((!enteredAddress.StreetLines.Equals(suggestedAddress.StreetLines, StringComparison.OrdinalIgnoreCase)
                               || !enteredAddress.City.Equals(suggestedAddress.City, StringComparison.OrdinalIgnoreCase)
                               || (enteredAddress.StateOrProvinceCode != null && !enteredAddress.StateOrProvinceCode.Equals(suggestedAddress.StateOrProvinceCode, StringComparison.OrdinalIgnoreCase))
                               || !enteredAddress.StateOrProvinceCode.Equals(suggestedAddress.StateOrProvinceCode, StringComparison.OrdinalIgnoreCase)
                               || !enteredAddress.PostalCode.Equals(suggestedAddress.PostalCode, StringComparison.OrdinalIgnoreCase)))
            {
                AddressContentDialog contentDialog = new AddressContentDialog(enteredAddress, suggestedAddress);
                switch (suggestedAddress.AddressType)
                {
                    case "physical":
                        contentDialog.Title = "Confirm Physical Address";
                        break;
                    case "shipping":
                        contentDialog.Title = "Confirm Shipping Address";
                        break;
                    case "mailing":
                        contentDialog.Title = "Confirm Mailing Address";
                        break;
                }

                var result = await contentDialog.ShowAsync();

                if (result == ContentDialogResult.Secondary)
                {
                    if (suggestedAddress.AddressType.Equals("physical", StringComparison.OrdinalIgnoreCase))
                    {
                        UiModel.PhysicalAddress = suggestedAddress.StreetLines;
                        UiModel.PhysicalCity = suggestedAddress.City;
                        UiModel.SelectedPhysicalState = suggestedAddress.StateOrProvinceCode;
                        UiModel.PhysicalZip = suggestedAddress.PostalCode;
                    }
                    else if (suggestedAddress.AddressType.Equals("shipping", StringComparison.OrdinalIgnoreCase))
                    {
                        UiModel.ShippingAddress = suggestedAddress.StreetLines;
                        UiModel.ShippingCity = suggestedAddress.City;
                        UiModel.SelectedShippingState = suggestedAddress.StateOrProvinceCode;
                        UiModel.ShippingZip = suggestedAddress.PostalCode;
                    }
                    else if (suggestedAddress.AddressType.Equals("mailing", StringComparison.OrdinalIgnoreCase))
                    {
                        UiModel.MailingAddress = suggestedAddress.StreetLines;
                        UiModel.MailingCity = suggestedAddress.City;
                        UiModel.SelectedMailingState = suggestedAddress.StateOrProvinceCode;
                        UiModel.MailingZip = suggestedAddress.PostalCode;
                    }
                }
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
                //Set isDeleted as 1, if account classification is 'Out Of Business (38)'
                isDeleted = UiModel?.SelectedAccountClassification.AccountClassificationId == 38 ? 1 : 0,
                IsExported = 0,
                RegionId = UiModel.Region != null ? UiModel.Region.RegionID : 0,
                ZoneId = UiModel.Zone != null ? UiModel.Zone.ZoneID : 0,
                TerritoryID = UiModel?.SelectedTerritory?.TerritoryID.ToString(),
                TerritoryName = UiModel?.SelectedTerritory?.TerritoryName,
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
            else if (string.IsNullOrWhiteSpace(UiModel.SelectedPhysicalState) || UiModel.SelectedPhysicalState.Equals("Select State"))
            {
                type = CustomerDetailFlyoutType.State;
                return false;
            }
            else if (string.IsNullOrWhiteSpace(UiModel.PhysicalZip))
            {
                type = CustomerDetailFlyoutType.Zip;
                return false;
            }
            else if (string.IsNullOrWhiteSpace(UiModel.PhysicalContactEmail))
            {
                type = CustomerDetailFlyoutType.ContactEmail;
                return false;
            }

            bool isValidEmail = false;

            if (!string.IsNullOrWhiteSpace(UiModel.PhysicalContactEmail))
            {
                isValidEmail = HelperMethods.IsValidEmail(UiModel.PhysicalContactEmail);
            }

            if (!isValidEmail)
            {
                type = CustomerDetailFlyoutType.InvalidEmail;
                return false;
            }

            var phoneCheck = UiModel?.PhysicalContactPhone;
            if (!string.IsNullOrWhiteSpace(phoneCheck))
            {
                if (phoneCheck.Equals("(___)-___-____"))
                    UiModel.PhysicalContactPhone = "";
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

            var faxCheck = UiModel?.PhysicalContactFax;
            if (!string.IsNullOrWhiteSpace(faxCheck))
            {
                if (faxCheck.Equals("(___)-___-____"))
                    UiModel.PhysicalContactFax = "";
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

            UiModel.SelectedPhysicalState = null;
            Weekdays = new ObservableCollection<WeekdayUIModel>
                {
                    new WeekdayUIModel { Id=1,Name = "Monday", IsSelected = false },
                    new WeekdayUIModel { Id=2,Name = "Tuesday", IsSelected = false },
                    new WeekdayUIModel { Id=3,Name = "Wednesday", IsSelected = false },
                    new WeekdayUIModel { Id=4,Name = "Thursday", IsSelected = false },
                    new WeekdayUIModel { Id=5,Name = "Friday", IsSelected = false },
                    new WeekdayUIModel { Id=6,Name = "Saturday", IsSelected = false },
                    new WeekdayUIModel { Id=7,Name = "Sunday", IsSelected = false }
                };
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
                UiModel.DistributionFlyoutListItemSource = new ObservableCollection<DistributorAssignUser>(tempList);
            }
        }


        private void DistributionSelectionCommandHandler(DistributorAssignUser obj)
        {
            var value = UiModel.DistributionFlyoutListItemSource?.Any(x => x.CustomerID == obj.CustomerID);
            if (value.HasValue && value.Value && obj != null)
            {
                var isSelected = !obj.IsSelected;
                (UiModel.DistributionFlyoutListItemSource?.FirstOrDefault(x => x.CustomerID == obj.CustomerID)).IsSelected = isSelected;
                (UiModel.MainDistributorList?.FirstOrDefault(x => x.CustomerID == obj.CustomerID)).IsSelected = isSelected;
            }
        }

        private void DeleteDistributorButtonCommandHandler(DistributorAssignUser distributor)
        {
            try
            {
                UiModel.DistributorList?.Remove(distributor);
                UiModel.MainDistributorList.FirstOrDefault(x => x.CustomerID == distributor.CustomerID).IsSelected = false;
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(AddCustomerPageViewModel), nameof(DeleteDistributorButtonCommand), ex.StackTrace);
            }
        }
        private void DistributorDoneCommandHandler()
        {
            var tempList = UiModel.MainDistributorList.Where(x => x.IsSelected);
            UiModel.DistributorList = new ObservableCollection<DistributorAssignUser>(tempList);
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
