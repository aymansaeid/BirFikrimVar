using BirFikrimVar.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Json;

namespace BirFikrimVar.Controllers
{
    public class CommentsController : Controller
    {
        private readonly HttpClient _http;

        public CommentsController(IHttpClientFactory httpClientFactory)
        {
            _http = httpClientFactory.CreateClient("ApiClient");
        }

        // GET: /Comments
        public async Task<IActionResult> Index()
        {
            var comments = await _http.GetFromJsonAsync<List<CommentDto>>("api/CommentsApi");
            return View(comments);
        }

        // GET: /Comments/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Comments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCommentDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var response = await _http.PostAsJsonAsync("api/CommentsApi", dto);

            if (response.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            ModelState.AddModelError("", "Failed to create comment.");
            return View(dto);
        }

        // GET: /Comments/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var comment = await _http.GetFromJsonAsync<CommentDto>($"api/CommentsApi/{id}");
            if (comment == null) return NotFound();

            var dto = new UpdateCommentDto
            {
                Content = comment.Content
            };

            return View(dto);
        }

        // POST: /Comments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateCommentDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var response = await _http.PutAsJsonAsync($"api/CommentsApi/{id}", dto);

            if (response.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            ModelState.AddModelError("", "Failed to update comment.");
            return View(dto);
        }

        // GET: /Comments/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var comment = await _http.GetFromJsonAsync<CommentDto>($"api/CommentsApi/{id}");
            if (comment == null) return NotFound();

            return View(comment);
        }

        // POST: /Comments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var response = await _http.DeleteAsync($"api/CommentsApi/{id}");

            if (response.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            return Problem("Failed to delete comment.");
        }
    }
}
