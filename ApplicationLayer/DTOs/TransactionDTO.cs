namespace Transaction_Reports_App.ApplicationLayer.DTOs
{
    public class TransactionDTO
    {
        public long MerchantId { get; set; }
        public DateTime TransactionDate { get; set; }
        public string? VoucherNumber { get; set; }
        public DateTime ExpiredDate { get; set; }
        public string? Description { get; set; }
        public decimal Amount { get; set; }
        public string? merchantName { get; set; }
        public long? TotalCount { get; set; }
        public long TerminalId { get; set; }
    }
}
