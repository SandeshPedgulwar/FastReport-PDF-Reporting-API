using Transaction_Reports_App.ApplicationLayer.DTOs;

namespace Transaction_Reports_App.ApplicationLayer.Services.Abstract
{
    public interface ITransactionService
    {
        Task<(IEnumerable<TransactionDTO> transactions, PaginationMetadata paginationMetadata, decimal totalAmount)> GetTransactionsAsync(
        int pageSize,
        int pageNumber,
        DateTime? dateFrom,
        DateTime? dateTo,
        string voucherNumber,
        long[]? merchantId,
        long? clientId,
        string sortColumn,
        string sortOrder,
        List<string> months
        );

        Task<List<TransactionDTO>> GetStaticTransactions();

        Task<byte[]> GenerateTransactionReport(
        string clientName,
        int pageSize,
        int pageNumber,
        DateTime? dateFrom,
        DateTime? dateTo,
        string voucherNumber,
        long[]? merchantId,
        long? clientId,
        string sortColumn,
        string sortOrder,
        List<string>? months
        );

    }
}
