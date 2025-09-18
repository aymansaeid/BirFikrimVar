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

        private int? GetCurrentUserId()
        {
            return HttpContext.Session.GetInt32("UserId");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleLike(int ideaId)
        {
            var userId = GetCurrentUserId();

            if (userId == null)
            {
                TempData["Error"] = "You must be logged in to like an idea.";
                return RedirectToAction("Login", "Users");
            }

            try
            {
                // Check if user already liked this idea
                var hasLiked = await _http.GetFromJsonAsync<bool>($"api/LikesApi/check/{ideaId}/{userId}");

                if (hasLiked)
                {
                    // User already liked, so we need to unlike using the new endpoint
                    var response = await _http.DeleteAsync($"api/LikesApi/unlike/{ideaId}/{userId}");
                    if (!response.IsSuccessStatusCode)
                    {
                        TempData["Error"] = "Failed to unlike the idea.";
                    }
                    else
                    {
                        TempData["Success"] = "Idea unliked successfully.";
                    }
                }
                else
                {
                    // User hasn't liked, so create a new like
                    var createLikeDto = new CreateLikeDto
                    {
                        IdeaId = ideaId,
                        UserId = userId.Value
                    };

                    var response = await _http.PostAsJsonAsync("api/LikesApi", createLikeDto);
                    if (response.IsSuccessStatusCode)
                    {
                        TempData["Success"] = "Idea liked successfully.";
                    }
                    else
                    {
                        TempData["Error"] = "Failed to like the idea.";
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = "Network error occurred while updating like status.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An unexpected error occurred.";
            }

            // Redirect back to the referring page or to Ideas index
            string referer = Request.Headers["Referer"].ToString();
            return Redirect(!string.IsNullOrEmpty(referer) ? referer : Url.Action("Index", "Ideas"));
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