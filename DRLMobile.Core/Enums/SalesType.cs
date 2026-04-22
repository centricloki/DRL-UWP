namespace DRLMobile.Core.Enums
{
    public enum SalesType
    {
        CashSale = 1,
        Prebook = 2,
        BillThrough = 3,
        SuggestedOrder = 4,
        RackPOS = 5,
        Distributor = 6,
        DistributorInvoice = 7,
        CreditRequest = 8,
        CashSalesInitiative = 9,
        ChainDistribution = 12,
        CreditCardSales = 13,
        SampleOrder = 11,
        POP = 10,
        CarStockOrder = 14
    }

    public enum CustomerTeamType
    {
        AssignedAccount = 1,
        TeamMember = 2,
        Both = 3,
    }
}
