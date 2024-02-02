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
            return View(new HomeViewModel());
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Split(IFormFile pdf)
        {
            try
            {
                // Criando um espaço na memória para a gravação de um PDF
                var zipMemoryStream = new MemoryStream();
                var pdfWriter = new PdfWriter(zipMemoryStream);
                var pdfDocument = new PdfDocument(pdfWriter);

                // Obtendo o conteúdo do pdf
                var stream = pdf.OpenReadStream();
                var pdfReader = new PdfReader(stream);
                var pdfDocumentReader = new PdfDocument(pdfReader);
                pdfDocumentReader.CopyPagesTo(1, 1, pdfDocument);

                pdfDocumentReader.Close();
                pdfDocument.Close();

                var conteudoPDF = zipMemoryStream.ToArray();


                return File(conteudoPDF, "application/pdf", "nome_do_arquivo.pdf");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
