using coremvctest.Data;
using coremvctest.Helpers;
using coremvctest.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System.Drawing;

using PdfSharpCore.Fonts;
using DinkToPdf;
using System.Text;

namespace coremvctest.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _db;

        public AdminController(ApplicationDbContext db)
        {
            _db = db;

        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Home()
        {
            return View();
        }
        public IActionResult ManageFoodsView() { return View(); }
        public IActionResult SearchedFoods() { return PartialView("_SearchedFoods"); }
        public IActionResult ShortLivedFoods() { return PartialView("_ShortLivedFoods"); }
        [HttpGet]
        public IActionResult SearchFood(string search)
        {
            var searchResults = _db.Foods
       .Where(m => m.FoodName.Contains(search))
       .ToList();
            return Json(searchResults);
        }
        [HttpGet]
        public async Task<IActionResult> SearchShortDurationFoods()
        {
            try
            {
                // Execute the stored procedure and retrieve the results
                var medicines = await _db.Foods
                    .FromSqlRaw("CALL GetFoodsWithShortRemainingDays")
                    .ToListAsync();

                return Json(medicines);
            }
            catch (Exception ex)
            {
                // Handle any exceptions, log, and return an error response if needed
                return BadRequest("An error occurred: " + ex.Message);
            }
        }
        public IActionResult InquiryFoodsView()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                return BadRequest("An error occurred: " + ex.Message);
            }
        }
        public IActionResult InquiredFoodsData(string search)
        {
            var inquiredNGOList = _db.RequestedFoodsByNGO.ToList();
            var uniqueRecords = inquiredNGOList
        .GroupBy(x => new { x.NGOId, x.DeliveryTime })
        .Select(group => group.First())
        .ToList();
            return Json(uniqueRecords);
        }
        public IActionResult GetRequestedFoodReports(int ngoId, string deliveryTime)
        {
            try
            {
                List<RequestedInquiryFoodsResult> result = _db.RequestedInquiryFoodsResult
                    .FromSqlRaw("CALL GetFoodUpdatesForInquiries(@ngoId, @deliveryTime)",
                        new MySqlParameter("@ngoId", MySqlDbType.Int32) { Value = ngoId },
                        new MySqlParameter("@deliveryTime", MySqlDbType.DateTime) { Value = deliveryTime })
                    .ToList();

                // Create HTML content dynamically based on your result data
                string htmlContent = GenerateHtmlContent(result);

                // Convert HTML to PDF
                var converter = new BasicConverter(new PdfTools());

                var options = new HtmlToPdfDocument
                {
                    GlobalSettings =
            {
                ColorMode = ColorMode.Color,
                Orientation = Orientation.Landscape,
                PaperSize = PaperKind.A4,
            },
                    Objects =
            {
                new ObjectSettings
                {
                    PagesCount = true,
                    HtmlContent = $@"
                        <!DOCTYPE html>
                        <html>
                        <head>
                            <style>
                                body {{
                                    font-family: 'Arial', sans-serif;
                                }}
                                table {{
                                    width: 100%;
                                    border-collapse: collapse;
                                }}
                                th, td {{
                                    border: 1px solid #dddddd;
                                    text-align: left;
                                    padding: 8px;
                                }}
                                th {{
                                    background-color: #f2f2f2;
                                }}
                                .green {{
                                    color: green;
                                    font-weight: bold;
                                }}
                                .red {{
                                    color: red;
                                    font-weight: bold;
                                }}
                            </style>
                        </head>
                        <body>
                            {htmlContent}
                        </body>
                        </html>"
                }
                }
                };

                var pdfBytes = converter.Convert(options);

                // Return the generated PDF file
                return File(pdfBytes, "application/pdf", "MedicineReport.pdf",true);
            }
            catch (Exception ex)
            {
                return Json($"Error: {ex.Message}");
            }
        }

        private string GenerateHtmlContent(List<RequestedInquiryFoodsResult> result)
        {
            // Generate HTML content dynamically based on your result data
            // Include the dynamic data from 'result' in the table rows
            // Apply the green and red color classes based on your condition

            // Example:
            StringBuilder htmlBuilder = new StringBuilder();

            htmlBuilder.AppendLine("<div style=\"background-color: #4CAF50; padding: 1em; text-align: center;\">");
            htmlBuilder.AppendLine("<h1 style=\"color: white;\">MedShare</h1>");
            htmlBuilder.AppendLine("</div>");

            // Table
            htmlBuilder.AppendLine("<table style=\"width: 100%; border-collapse: collapse;\">");
            htmlBuilder.AppendLine("<tr style=\"background-color: #4CAF50; color: black;\"><th>Number</th><th>NGO ID</th><th>Category</th><th>Total Requested Quantity</th><th>Quantity</th><th>Delivery Time</th></tr>");
            foreach (var row in result)
            {
                            htmlBuilder.AppendLine("<tr>");
                htmlBuilder.AppendLine($"<td>{row.Id}</td>");
                htmlBuilder.AppendLine($"<td>{row.NGOId}</td>");
                htmlBuilder.AppendLine($"<td>{row.Category}</td>");
                string quantityClass = (row.Quantity < row.TotalRequestedQuantity) ? "green" : "red";
                htmlBuilder.AppendLine($"<td class=\"{quantityClass}\">{row.Quantity}</td>");
                htmlBuilder.AppendLine($"<td>{row.TotalRequestedQuantity}</td>");
                htmlBuilder.AppendLine($"<td>{row.DeliveryTime.ToString("yyyy-MM-dd")}</td>");
                htmlBuilder.AppendLine("</tr>");                     
            }
            htmlBuilder.AppendLine("</table>");
            htmlBuilder.AppendLine("<div style=\"background-color: #4CAF50; padding: 1em; text-align: center; color: white;\">");
            htmlBuilder.AppendLine("");
            htmlBuilder.AppendLine("</div>");
            
            return htmlBuilder.ToString();
        }




    }
}
