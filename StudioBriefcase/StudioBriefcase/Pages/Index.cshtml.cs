using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace StudioBriefcase.Pages
{
    public class IndexModel : PageModel
    {
        //public string? _test1 { get; set; }
        //public string? _test2 { get; set; }
        private readonly ILogger<IndexModel> _logger;
        private readonly IConfiguration _configuration;


        public IndexModel(ILogger<IndexModel> logger, IConfiguration iconfiguration)
        {
            _logger = logger;
            _configuration = iconfiguration;

            //_test1 = _configuration["test5"];
            //_test2 = _configuration["test6"];
            
        }
        public void OnGet()
        {
        }


    }
}
