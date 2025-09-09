using BirFikrimVar.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Json;

namespace BirFikrimVar.Controllers
{
    public class LikesController : Controller
    {
        private readonly HttpClient _http;

        public LikesController(IHttpClientFactory httpClientFactory)
        {
            _http = httpClientFactory.CreateClient("ApiClient");
        }

        // GET: /Likes
        public async Task<IActionResult> Index()
        {
            var likes = await _http.GetFromJsonAsync<List<LikeResponseDto>>("api/LikesApi");
            return View(likes);
        }

        // GET: /Likes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Likes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateLikeDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var response = await _http.PostAsJsonAsync("api/LikesApi", dto);

            if (response.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            ModelState.AddModelError("", "Failed to create like.");
            return View(dto);
        }

        // GET: /Likes/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var like = await _http.GetFromJsonAsync<LikeResponseDto>($"api/LikesApi/{id}");
            if (like == null) return NotFound();

            return View(like);
        }

        // POST: /Likes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var response = await _http.DeleteAsync($"api/LikesApi/{id}");

            if (response.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            return Problem("Failed to delete like.");
        }
    }
}
