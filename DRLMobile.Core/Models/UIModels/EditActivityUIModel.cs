using System;

namespace DRLMobile.Core.Models.UIModels
{
    public class EditActivityUIModel : BaseModel
    {
        private DateTimeOffset? _callDate;
        public DateTimeOffset? CallDate
        {
            get { return _callDate; }
            set { SetProperty(ref _callDate, value); }
        }

        private string _selectedActivityType;
        public string SelectedActivityType
        {
            get { return _selectedActivityType; }
            set { SetProperty(ref _selectedActivityType, value); }
        }

        private string _createdBy;
        public string CreatedBy
        {
            get { return _createdBy; }
            set { SetProperty(ref _createdBy, value); }
        }

        private string _team;
        public string Team
        {
            get { return _team; }
            set { SetProperty(ref _team, value); }
        }

        private string _accountNo;
        public string AccountNo
        {
            get { return _accountNo; }
            set { SetProperty(ref _accountNo, value); }
        }


        private string _accountName;
        public string AccountName
        {
            get { return _accountName; }
            set { SetProperty(ref _accountName, value); }
        }


        private string _city;
        public string City
        {
            get { return _city; }
            set { SetProperty(ref _city, value); }
        }

        private string _state;
        public string State
        {
            get { return _state; }
            set { SetProperty(ref _state, value); }
        }

        private string _amount;
        public string Amount
        {
            get { return _amount; }
            set { SetProperty(ref _amount, value); }
        }

        private string _hours;
        public string Hours
        {
            get { return _hours; }
            set { SetProperty(ref _hours, value); }
        }

        private string _notes;
        public string Notes
        {
            get { return _notes; }
            set { SetProperty(ref _notes, value); }
        }

        private string _orderNo;
        public string OrderNo
        {
            get { return _orderNo; }
            set { SetProperty(ref _orderNo, value); }
        }

        private string _orderType;
        public string OrderType
        {
            get { return _orderType; }
            set { SetProperty(ref _orderType, value); }
        }

        private string _dateCreated;
        public string DateCreated
        {
            get { return _dateCreated; }
            set { SetProperty(ref _dateCreated, value); }
        }

        private string _dateModified;
        public string DateModified
        {
            get { return _dateModified; }
            set { SetProperty(ref _dateModified, value); }
        }

        public string CustomerDeviceId { get; set; }
        public int UserId { get; set; }

        private string _consumerActivationEngagement;
        public string ConsumerActivationEngagement
        {
            get { return _consumerActivationEngagement; }
            set { VerifyActivationEngagement(value); SetProperty(ref _consumerActivationEngagement, value); }
        }
        private bool _isInValidConsumerActivationEngagement;
        public bool IsInValidConsumerActivationEngagement
        {
            get { return _isInValidConsumerActivationEngagement; }
            set { SetProperty(ref _isInValidConsumerActivationEngagement, value); }
        }
        private string _errorConsumerActivationEngagement;
        public string ErrorConsumerActivationEngagement
        {
            get { return _errorConsumerActivationEngagement; }
            set { SetProperty(ref _errorConsumerActivationEngagement, value); }
        }
        private string _marketsvisited;
        public string MarketsVisited
        {
            get { return _marketsvisited; }
            set { SetProperty(ref _marketsvisited, value); }
        }

        private string _callsmadevsgoal;
        public string CallsMadeVsGoal
        {
            get { return _callsmadevsgoal; }
            set { SetProperty(ref _callsmadevsgoal, value); }
        }
        private string _newcustomeracquisitions;
        public string NewCustomerAcquisitions
        {
            get { return _newcustomeracquisitions; }
            set { SetProperty(ref _newcustomeracquisitions, value); }
        }
        private string _keywinssummary;
        public string KeyWinsSummary
        {
            get { return _keywinssummary; }
            set { SetProperty(ref _keywinssummary, value); }
        }
        private string _challengesandfeedback;
        public string ChallengesAndFeedback
        {
            get { return _challengesandfeedback; }
            set { SetProperty(ref _challengesandfeedback, value); }
        }
        private string _nextcycleplan;
        public string NextCyclePlan
        {
            get { return _nextcycleplan; }
            set { SetProperty(ref _nextcycleplan, value); }
        }
        private string _nextweekplan;
        public string NextWeekPlan
        {
            get { return _nextweekplan; }
            set { SetProperty(ref _nextweekplan, value); }
        }




        private void VerifyActivationEngagement(string inputValue)
        {
            IsInValidConsumerActivationEngagement = true;
            ErrorConsumerActivationEngagement = "Please enter a valid number of up to 6 digits.";

            if (!string.IsNullOrWhiteSpace(inputValue))
            {
                // Consumer Activation Engagement Max-Length should not more than 6 digit long
                if (int.TryParse(inputValue, out int result))
                {
                    if (result <= 999999)
                    {
                        IsInValidConsumerActivationEngagement = false;
                        ErrorConsumerActivationEngagement = string.Empty;
                    }
                }
            }
            else
            {
                IsInValidConsumerActivationEngagement = false;
                ErrorConsumerActivationEngagement = string.Empty;
            }
        }

    }
}