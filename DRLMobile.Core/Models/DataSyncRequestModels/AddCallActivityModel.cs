namespace DRLMobile.Core.Models.DataSyncRequestModels
{
    public class AddCallActivityModel : BaseModel
    {
        public int IsVoided { get; set; }
        public string activitytype { get; set; }
        public int grandtotal { get; set; }
        public string calldate { get; set; }
        public string comments { get; set; }
        public string createdate { get; set; }
        public string devicecallactivityid { get; set; }
        public string devicecustomerid { get; set; }
        public string deviceorderid { get; set; }
        public string gratisproductused { get; set; }
        public int hours { get; set; }
        public int isthisaccountfromyourlist { get; set; }
        public string objective { get; set; }
        public string result { get; set; }
        public string orderdeviceid { get; set; }
        public string ConsumerActivationEngagement { get; set; }
        public string MarketsVisited { get; set; }
        public string CallsMadeVsGoal { get; set; }
        public string NewCustomerAcquisitions { get; set; }
        public string KeyWinsSummary { get; set; }
        public string ChallengesAndFeedback { get; set; }
        public string NextCyclePlan { get; set; }
        public string NextWeekPlan { get; set; }
    }
}