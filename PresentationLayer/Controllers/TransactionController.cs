using Microsoft.AspNetCore.Mvc;
using Transaction_Reports_App.ApplicationLayer.Services.Abstract;

namespace Transaction_Reports_App.PresentationLayer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {

        private readonly ITransactionService _transactionService;

        public TransactionController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpGet("get-transactions")]
        public async Task<IActionResult> GetTransactions(
            [FromQuery] int pageSize = 5,
            [FromQuery] int pageNumber = 1,
            [FromQuery] DateTime? dateFrom = null,
            [FromQuery] DateTime? dateTo = null,
            [FromQuery] string voucherNumber = "",
            [FromQuery] string? merchantIds = null,
            [FromQuery] long? clientId = null,
            [FromQuery] string? months = null,
            [FromQuery] string sortColumn = "transaction_date",
            [FromQuery] string sortOrder = "ASC",
            [FromQuery] string fileType = "json")
        {
            long[] merchantIdsList = [];

            if (!string.IsNullOrEmpty(merchantIds))
            {
                merchantIdsList = merchantIds.Split(',')
                                          .Select(id => Convert.ToInt64(id.Trim()))
                                          .ToArray();
            }

            var monthsList = string.IsNullOrEmpty(months)
                ? new List<string>()
                : months.Split(',').Select(m => m.Trim()).ToList();

            long incomingClientId = 1001;

            var (transactions, paginationMetadata, totalAmount) = await _transactionService.GetTransactionsAsync(
                pageSize, pageNumber, dateFrom, dateTo, voucherNumber, merchantIdsList, incomingClientId, sortColumn, sortOrder, monthsList);

            return Ok(new
            {
                Transactions = transactions,
                TotalAmount = totalAmount,
                Pagination = paginationMetadata,
            });
        }


        [HttpGet("transaction-report")]
        public async Task<IActionResult> TestTransactionReport()
        {
            try
            {
                var pdfBytes = await _transactionService.GenerateTransactionReport(
                    clientName: "Test Client",
                    pageSize: 20,
                    pageNumber: 1,
                    dateFrom: new DateTime(2025, 3, 1),
                    dateTo: new DateTime(2025, 12, 31),
                    voucherNumber: "",
                    merchantId: null,
                    clientId: null,
                    sortColumn: "transaction_date",
                    sortOrder: "asc",
                    months: null
                );

                // Return PDF file for download
                return File(pdfBytes, "application/pdf", "TestTransactionReport.pdf");
            }
            catch (Exception)
            {
                return BadRequest($"Error generating report");
            }
        }

    }
}
