using FastReport;
using FastReport.Export.PdfSimple;

namespace Transaction_Reports_App.InfrastructreLayer.Services
{
    public class ReportUtils
    {
        public static (DateTime reportDateFrom, DateTime reportDateTo) GetReportDateRange(DateTime? dateFrom, DateTime? dateTo)
        {
            DateTime defaultDateFrom = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            DateTime defaultDateTo = DateTime.Now;

            DateTime actualDateFrom = dateFrom ?? defaultDateFrom;
            DateTime actualDateTo = dateTo ?? defaultDateTo;

            return (actualDateFrom, actualDateTo);
        }

        public static byte[] ExportReportToPdf(Report report)
        {
            using (var stream = new MemoryStream())
            {
                var pdfExport = new PDFSimpleExport();
                pdfExport.Export(report, stream);
                stream.Position = 0;
                return stream.ToArray();
            }
        }
    }
}
