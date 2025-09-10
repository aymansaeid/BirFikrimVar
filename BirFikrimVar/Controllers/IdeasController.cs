using BirFikrimVar.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Json;

namespace BirFikrimVar.Controllers
{
    public class IdeasController : Controller
    {
        private readonly HttpClient _http;

        public IdeasController(IHttpClientFactory httpClientFactory)
        {
            _http = httpClientFactory.CreateClient("ApiClient");
        }

        // GET: /Ideas
        public async Task<IActionResult> Index()
        {
            var ideas = await _http.GetFromJsonAsync<List<IdeaDto>>("api/IdeasApi");
            return View(ideas);
        }

        // GET: /Ideas/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var idea = await _http.GetFromJsonAsync<IdeaDto>($"api/IdeasApi/{id}");
            if (idea == null) return NotFound();

            return View(idea);
        }

        // GET: /Ideas/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Ideas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateIdeaDto dto)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Users");
            }

            dto.UserId = userId.Value;

            var response = await _http.PostAsJsonAsync("api/IdeasApi", dto);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

            return View(dto);
        }

        // GET: /Ideas/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var idea = await _http.GetFromJsonAsync<IdeaDto>($"api/IdeasApi/{id}");
            if (idea == null) return NotFound();

            var dto = new UpdateIdeaDto
            {
                Title = idea.Title,
                Content = idea.Content
            };

            return View(dto);
        }

        // POST: /Ideas/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateIdeaDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var response = await _http.PutAsJsonAsync($"api/IdeasApi/{id}", dto);

            if (response.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            ModelState.AddModelError("", "Failed to update idea.");
            return View(dto);
        }

        // GET: /Ideas/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var idea = await _http.GetFromJsonAsync<IdeaDto>($"api/IdeasApi/{id}");
            if (idea == null) return NotFound();

            return View(idea);
        }

        // POST: /Ideas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var response = await _http.DeleteAsync($"api/IdeasApi/{id}");

            if (response.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            return Problem("Failed to delete idea.");
        }
    }
}
