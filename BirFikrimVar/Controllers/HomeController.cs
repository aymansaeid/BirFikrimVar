using BirFikrimVar.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using static System.Net.WebRequestMethods;

namespace BirFikrimVar.Controllers
{
    public class HomeController : Controller
    {
        private readonly HttpClient _http;


     
        public HomeController(IHttpClientFactory httpClientFactory)
        {
            _http = httpClientFactory.CreateClient("ApiClient");
        }

        public async Task<IActionResult> Index()
        {
            var ideas = await _http.GetFromJsonAsync<List<IdeaDto>>("api/IdeasApi");

            if (ideas == null)
                ideas = new List<IdeaDto>();

            return View(ideas);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
