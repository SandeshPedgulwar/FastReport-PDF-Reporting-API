using FastReport;
using System.Data;
using System.Globalization;
using Transaction_Reports_App.ApplicationLayer.DTOs;
using Transaction_Reports_App.ApplicationLayer.Services.Abstract;
using Transaction_Reports_App.InfrastructreLayer.Services;

namespace Transaction_Reports_App.ApplicationLayer.Services.Concrete
{
    public class TransactionService : ITransactionService
    {

        public async Task<( IEnumerable<TransactionDTO> transactions, PaginationMetadata paginationMetadata, decimal totalAmount)> GetTransactionsAsync(
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
        )
        {
            try
            {
                var transactions = new List<TransactionDTO>();
                PaginationMetadata pagination = new PaginationMetadata();
                long? totalCount = null;

                // Ensure that pageSize and pageNumber are valid
                pageSize = pageSize <= 0 ? 10 : pageSize;
                pageNumber = pageNumber <= 0 ? 1 : pageNumber;

                // ---------- STATIC DATA INSTEAD OF DATABASE ----------
                var allDataTask = GetStaticTransactions();
                var allData = await allDataTask;

                // Apply filters
                if (dateFrom.HasValue)
                {
                    allData = allData
                        .Where(x => x.TransactionDate >= dateFrom.Value)
                        .ToList();
                }

                if (dateTo.HasValue)
                {
                    allData = allData
                        .Where(x => x.TransactionDate <= dateTo.Value)
                        .ToList();
                }

                if (!string.IsNullOrEmpty(voucherNumber))
                {
                    allData = allData
                        .Where(x => x.VoucherNumber != null &&
                                    x.VoucherNumber.StartsWith(voucherNumber, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                }

                if (merchantId != null && merchantId.Length > 0)
                {
                    allData = allData
                        .Where(x => merchantId.Contains(x.MerchantId))
                        .ToList();
                }

                if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortOrder))
                {
                    var ascending = sortOrder.Equals("asc", StringComparison.OrdinalIgnoreCase);

                    switch (sortColumn.ToLowerInvariant())
                    {
                        case "transaction_date":
                            allData = ascending
                                ? allData.OrderBy(x => x.TransactionDate).ToList()
                                : allData.OrderByDescending(x => x.TransactionDate).ToList();
                            break;

                        case "amount":
                            allData = ascending
                                ? allData.OrderBy(x => x.Amount).ToList()
                                : allData.OrderByDescending(x => x.Amount).ToList();
                            break;

                        case "voucher_number":
                            allData = ascending
                                ? allData.OrderBy(x => x.VoucherNumber).ToList()
                                : allData.OrderByDescending(x => x.VoucherNumber).ToList();
                            break;

                        default:
                            break;
                    }
                }

                var overallCount = allData.Count;
                foreach (var item in allData)
                {
                    item.TotalCount = overallCount;
                }

                transactions = allData;

                decimal totalAmount = 0;
                if (months?.Any() == true)
                {
                    transactions = transactions
                        .Where(r => r.TransactionDate != default(DateTime) &&
                                    months.Contains($"{r.TransactionDate.Year}-{r.TransactionDate.Month:D2}"))
                        .ToList();

                    totalCount = transactions.Count();

                    transactions = transactions
                        .Skip((pageNumber - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();
                }
                else
                {
                    totalCount = transactions.FirstOrDefault()?.TotalCount ?? transactions.Count;

                    transactions = transactions
                        .Skip((pageNumber - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();
                }

                // Calculate total amount of the paginated results
                totalAmount = transactions.Sum(x => x.Amount);

                pagination = new PaginationMetadata
                {
                    CurrentPage = pageNumber,
                    PageSize = pageSize,
                    TotalCount = (int)totalCount,
                    TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
                };

                return (transactions, pagination, totalAmount);
            }
            catch (Exception)
            {
                // Log exception here if logging is configured
                throw new ApplicationException(
                    "An error occurred while retrieving transaction data. Please try again later."
                );
            }
        }


        public async Task<List<TransactionDTO>> GetStaticTransactions()
        {
            return await Task.FromResult(new List<TransactionDTO>
            {
                new TransactionDTO
                {
                    MerchantId = 1001,
                    TerminalId = 10101,
                    TransactionDate = new DateTime(2025, 01, 05, 10, 15, 00),
                    VoucherNumber = "E310000000001",
                    ExpiredDate = new DateTime(2025, 02, 05),
                    Description = "Grocery purchase - supermarket invoice",
                    Amount = 1850.75m,
                    TotalCount = null,
                    merchantName = "Supermercado Primavera"
                },
                new TransactionDTO
                {
                    MerchantId = 1001,
                    TerminalId = 10102,
                    TransactionDate = new DateTime(2025, 01, 12, 18, 30, 00),
                    VoucherNumber = "E310000000002",
                    ExpiredDate = new DateTime(2025, 02, 12),
                    Description = "Household items and cleaning products",
                    Amount = 2360.20m,
                    TotalCount = null,
                    merchantName = "Supermercado Primavera"
                },
                new TransactionDTO
                {
                    MerchantId = 2001,
                    TerminalId = 20101,
                    TransactionDate = new DateTime(2025, 02, 03, 09, 05, 00),
                    VoucherNumber = "E320000000101",
                    ExpiredDate = new DateTime(2025, 03, 03),
                    Description = "Fuel purchase - gasoline 95",
                    Amount = 4200.00m,
                    TotalCount = null,
                    merchantName = "Estación de Servicios Central"
                },
                new TransactionDTO
                {
                    MerchantId = 2001,
                    TerminalId = 20102,
                    TransactionDate = new DateTime(2025, 02, 15, 14, 45, 00),
                    VoucherNumber = "E320000000102",
                    ExpiredDate = new DateTime(2025, 03, 15),
                    Description = "Fuel + car wash service",
                    Amount = 3150.50m,
                    TotalCount = null,
                    merchantName = "Estación de Servicios Central"
                },
                new TransactionDTO
                {
                    MerchantId = 3001,
                    TerminalId = 30101,
                    TransactionDate = new DateTime(2025, 03, 01, 11, 10, 00),
                    VoucherNumber = "E330000000501",
                    ExpiredDate = new DateTime(2025, 04, 01),
                    Description = "Lunch invoice - restaurant service",
                    Amount = 980.00m,
                    TotalCount = null,
                    merchantName = "Restaurante El Buen Sabor"
                },
                new TransactionDTO
                {
                    MerchantId = 3001,
                    TerminalId = 30102,
                    TransactionDate = new DateTime(2025, 03, 18, 20, 20, 00),
                    VoucherNumber = "E330000000502",
                    ExpiredDate = new DateTime(2025, 04, 18),
                    Description = "Dinner invoice - restaurant service",
                    Amount = 1725.30m,
                    TotalCount = null,
                    merchantName = "Restaurante El Buen Sabor"
                },
                new TransactionDTO
                {
                    MerchantId = 4001,
                    TerminalId = 40101,
                    TransactionDate = new DateTime(2025, 04, 04, 08, 40, 00),
                    VoucherNumber = "E340000000801",
                    ExpiredDate = new DateTime(2025, 05, 04),
                    Description = "Pharmacy invoice - medicines",
                    Amount = 650.90m,
                    TotalCount = null,
                    merchantName = "Farmacia Central"
                },
                new TransactionDTO
                {
                    MerchantId = 4001,
                    TerminalId = 40102,
                    TransactionDate = new DateTime(2025, 04, 22, 16, 30, 00),
                    VoucherNumber = "E340000000802",
                    ExpiredDate = new DateTime(2025, 05, 22),
                    Description = "Personal care products and medicines",
                    Amount = 1890.40m,
                    TotalCount = null,
                    merchantName = "Farmacia Central"
                },
                new TransactionDTO
                {
                    MerchantId = 5001,
                    TerminalId = 50101,
                    TransactionDate = new DateTime(2025, 05, 07, 13, 00, 00),
                    VoucherNumber = "E350000001201",
                    ExpiredDate = new DateTime(2025, 06, 07),
                    Description = "Electronics purchase - smartphone",
                    Amount = 24500.00m,
                    TotalCount = null,
                    merchantName = "Tecnología Caribe SRL"
                },
                new TransactionDTO
                {
                    MerchantId = 5001,
                    TerminalId = 50102,
                    TransactionDate = new DateTime(2025, 05, 19, 17, 45, 00),
                    VoucherNumber = "E350000001202",
                    ExpiredDate = new DateTime(2025, 06, 19),
                    Description = "Electronics purchase - laptop",
                    Amount = 48500.99m,
                    TotalCount = null,
                    merchantName = "Tecnología Caribe SRL"
                },
                new TransactionDTO
                {
                    MerchantId = 6001,
                    TerminalId = 60101,
                    TransactionDate = new DateTime(2025, 06, 02, 09, 25, 00),
                    VoucherNumber = "E360000001801",
                    ExpiredDate = new DateTime(2025, 07, 02),
                    Description = "Taxi / transportation service invoice",
                    Amount = 420.00m,
                    TotalCount = null,
                    merchantName = "Transporte Urbano RD"
                },
                new TransactionDTO
                {
                    MerchantId = 6001,
                    TerminalId = 60102,
                    TransactionDate = new DateTime(2025, 06, 16, 21, 10, 00),
                    VoucherNumber = "E360000001802",
                    ExpiredDate = new DateTime(2025, 07, 16),
                    Description = "Private transfer service invoice",
                    Amount = 1350.00m,
                    TotalCount = null,
                    merchantName = "Transporte Urbano RD"
                },
                new TransactionDTO
                {
                    MerchantId = 7001,
                    TerminalId = 70101,
                    TransactionDate = new DateTime(2025, 07, 03, 15, 05, 00),
                    VoucherNumber = "E370000002101",
                    ExpiredDate = new DateTime(2025, 08, 03),
                    Description = "Monthly gym membership",
                    Amount = 2500.00m,
                    TotalCount = null,
                    merchantName = "Gimnasio Vida Sana"
                },
                new TransactionDTO
                {
                    MerchantId = 7001,
                    TerminalId = 70102,
                    TransactionDate = new DateTime(2025, 07, 21, 07, 45, 00),
                    VoucherNumber = "E370000002102",
                    ExpiredDate = new DateTime(2025, 08, 21),
                    Description = "Personal training session package",
                    Amount = 7800.00m,
                    TotalCount = null,
                    merchantName = "Gimnasio Vida Sana"
                },
                new TransactionDTO
                {
                    MerchantId = 8001,
                    TerminalId = 80101,
                    TransactionDate = new DateTime(2025, 08, 05, 19, 30, 00),
                    VoucherNumber = "E380000002501",
                    ExpiredDate = new DateTime(2025, 09, 05),
                    Description = "Hotel lodging - 2 nights",
                    Amount = 18500.00m,
                    TotalCount = null,
                    merchantName = "Hotel Caribe Plaza"
                }

            });
        }

        public async Task<byte[]> GenerateTransactionReport(string clientName, int pageSize, int pageNumber, DateTime? dateFrom, DateTime? dateTo, string voucherNumber, long[]? merchantId, long? clientId, string sortColumn, string sortOrder, List<string>? months)
        {
            try
            {
                var (transactions, pages, totalAmount) = await GetTransactionsAsync(pageSize, pageNumber, dateFrom, dateTo, voucherNumber, merchantId, clientId, sortColumn, sortOrder, months);
                var (reportDateFrom, reportDateTo) = ReportUtils.GetReportDateRange(dateFrom, dateTo);

                var transactionTable = CreateTransactionTable(transactions);
                var culture = CultureInfo.InvariantCulture;

                using var report = new Report();
                var reportPath = Path.Combine("Reports", "Transaction_Report.frx");

                if (!File.Exists(reportPath))
                    throw new FileNotFoundException("Report file not found.");

                report.Load(reportPath);
                report.RegisterData(transactionTable, "TransactionOverview");

                var firstTransaction = transactions.FirstOrDefault();
                report.SetParameterValue("MerchantName", clientName);
                report.SetParameterValue("MerchantId", firstTransaction?.MerchantId.ToString() ?? "0");
                report.SetParameterValue("From", reportDateFrom.ToString("dd/MM/yyyy", culture));
                report.SetParameterValue("To", reportDateTo.ToString("dd/MM/yyyy", culture));
                report.SetParameterValue("TotalAmount", totalAmount);

                report.Prepare();
                return ReportUtils.ExportReportToPdf(report);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static DataTable CreateTransactionTable(IEnumerable<dynamic> transactions)
        {
            var table = new DataTable("TransactionOverview");
            table.Columns.Add("AFFILIATE NUMBER", typeof(string));
            table.Columns.Add("TRANSACTION DATE", typeof(string));
            table.Columns.Add("VOUCHER NUMBER", typeof(string));
            table.Columns.Add("EXPIRATION DATE", typeof(string));
            table.Columns.Add("DESCRIPTION", typeof(string));
            table.Columns.Add("AMOUNT", typeof(decimal));
            table.Columns.Add("TERMINAL ID", typeof(string));

            var culture = CultureInfo.InvariantCulture;
            foreach (var transaction in transactions)
            {
                table.Rows.Add(
                    transaction.MerchantId,
                    transaction.TransactionDate.ToString("dd/MM/yyyy", culture),
                    transaction.VoucherNumber,
                    transaction.ExpiredDate.ToString("dd/MM/yyyy", culture),
                    transaction.Description,
                    transaction.Amount,
                    transaction.TerminalId
                );
            }

            return table;
        }

    }
}