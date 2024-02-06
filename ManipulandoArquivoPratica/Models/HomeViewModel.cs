namespace ManipulandoArquivoPratica.Models
{
    public class HomeViewModel
    {
        public IFormFile Pdf {  get; set; }

        public List<IFormFile> PDFs { get; set; }

        public HomeViewModel()
        {
            PDFs = new List<IFormFile>();
        }
    }
}
