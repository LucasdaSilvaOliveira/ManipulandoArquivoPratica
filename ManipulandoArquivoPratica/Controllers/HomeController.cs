using iText.IO.Source;
using iText.Kernel.Pdf;
using ManipulandoArquivoPratica.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.IO.Compression;
using System.Reflection.Metadata;

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

        //MÉTODO QUE COPIA 1 PDF E GERA UM NOVO COM SUCESSO
        [HttpPost]
        public IActionResult SplitAntigo(IFormFile pdf)
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

        [HttpPost]
        public IActionResult Split(IFormFile pdf)
        {
            try
            {
                var zipMemoryStream = new MemoryStream();
                using (var zipArchive = new ZipArchive(zipMemoryStream, ZipArchiveMode.Create, true))
                {
                    var pageCount = ObterQuantidadePaginas(pdf);

                    for (var page = 1; page <= pageCount; page++)
                    {
                        // Criando um espaço na memória para a gravação de um PDF específico
                        var pdfMemoryStream = new MemoryStream();
                        var pdfWriter = new PdfWriter(pdfMemoryStream);
                        var pdfDocument = new PdfDocument(pdfWriter);
                        //pdfMemoryStream.Position = 0;

                        // Obtendo o conteúdo do pdf
                        var stream = pdf.OpenReadStream();
                        var pdfReader = new PdfReader(stream);
                        //stream.Position = 0;
                        var pdfDocumentReader = new PdfDocument(pdfReader);

                        pdfDocumentReader.CopyPagesTo(page, page, pdfDocument);

                        // Adicionando o PDF específico ao arquivo ZIP
                        var entry = zipArchive.CreateEntry($"pagina_{page}.pdf");
                        using (var entryStream = entry.Open())
                        {
                            var writ = new PdfWriter(entryStream);
                            var doc = new PdfDocument(writ);

                            //pdfMemoryStream.Position = 0;
                            //pdfMemoryStream.Seek(0, SeekOrigin.Begin);
                            //pdfMemoryStream.CopyTo(entryStream);
                            pdfDocumentReader.CopyPagesTo(page, page, doc);

                            doc.Close();
                        }


                        pdfDocumentReader.Close();
                        pdfReader.Close();
                        stream.Close();
                        pdfDocument.Close();
                        pdfWriter.Close();
                        pdfMemoryStream.Close();
                    }
              
                }

                //zipMemoryStream.Position = 0;
                //zipMemoryStream.Seek(0, SeekOrigin.Begin);

                var conteudoZIP = zipMemoryStream.ToArray();

                zipMemoryStream.Close();

                return File(conteudoZIP, "application/zip", "nome_do_arquivo.zip");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        private int ObterQuantidadePaginas(IFormFile pdf)
        {
            var stream = pdf.OpenReadStream();
            var reader = new PdfReader(stream);
            var doc = new PdfDocument(reader);

            var quantidadePaginas = doc.GetNumberOfPages();

            stream.Close();
            reader.Close();
            doc.Close();

            return quantidadePaginas;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
