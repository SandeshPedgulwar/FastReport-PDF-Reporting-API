# 📊 Transaction Reports API

An **ASP.NET Core Web API** for generating **transaction reports** with advanced filtering, sorting, pagination, and **PDF generation using FastReport**.  
This project demonstrates backend reporting logic commonly used in **finance and enterprise applications**.

---

## 🚀 Features

- Transaction listing with:
  - Pagination
  - Date range filtering
  - Merchant filtering
  - Voucher number search
  - Monthly filtering
  - Sorting by transaction date, amount, or voucher number
- Total transaction amount calculation
- Server-side **PDF report generation** using **FastReport**
- Clean service-layer architecture
- Exception handling with meaningful error messages
- Static data source (no database) for learning and demonstration

---

## 🛠️ Tech Stack

- **ASP.NET Core Web API**
- **C#**
- **FastReport.NET**
- **LINQ**
- **DataTable**
- **PDF Export**

---

## 📄 PDF Reporting with FastReport

This project uses **FastReport** instead of HTML-to-PDF rendering to generate reports.

### Why FastReport?
- Consistent and reliable PDF output
- Better control over layout, headers, footers, and totals
- Report templates (`.frx`) for structured reporting
- Server-side generation without browser dependency

### Report Includes:
- Merchant information
- Transaction date range
- Voucher number
- Transaction amount
- Terminal ID
- Total transaction amount

---

## 📌 API Capabilities

### Get Transactions
Supports:
- `pageSize`
- `pageNumber`
- `dateFrom`
- `dateTo`
- `voucherNumber`
- `merchantId`
- `sortColumn`
- `sortOrder`
- `months`

Returns:
- List of transactions
- Pagination metadata
- Total transaction amount

---

### Generate Transaction Report (PDF)

- Loads FastReport `.frx` template
- Binds transaction data using `DataTable`
- Sets report parameters dynamically
- Exports the prepared report to **PDF**

---

## 🧠 What I Learned

- Implementing pagination, filtering, and sorting in ASP.NET Core
- Designing transaction-based domain logic
- Generating structured PDF reports using FastReport
- Working with `DataTable` for reporting
- Writing clean and maintainable service-layer code
- Applying proper exception handling in backend services

---

## 📂 Project Structure (High Level)

/Controllers
└── TransactionsController.cs

/ApplicationLayer
└── Services
└── TransactionService.cs

/Reports
└── Transaction_Report.frx

/Utils
└── ReportUtils.cs

## ▶️ How to Run

1. Clone the repository
2. Open the solution in Visual Studio
3. Restore NuGet packages
4. Run the application
5. Use Swagger or Postman to test the APIs

---

## 📌 Notes

- Uses static in-memory data instead of a database
- Easily extendable to use Entity Framework Core or Dapper
- Suitable for backend learning and portfolio demonstration

---

## 👨‍💻 Author

Built as part of continuous learning in **ASP.NET Core backend development and reporting systems**.
