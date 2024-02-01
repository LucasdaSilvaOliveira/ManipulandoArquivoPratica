using iText.Kernel.Pdf;
using ManipulandoArquivoPratica.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ManipulandoArquivoPratica.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Split(IFormFile pdf)
        {
            // Criando um espaço na memória para a gravação de um PDF
            using var zipMemoryStream = new MemoryStream();
            using var pdfWriter = new PdfWriter(zipMemoryStream);
            using var pdfDocument = new PdfDocument(pdfWriter);

            // Obtendo o conteúdo do pdf
            using var stream = pdf.OpenReadStream();
            using var pdfReader = new PdfReader(stream);
            using var pdfDocumentReader = new PdfDocument(pdfReader);

        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
