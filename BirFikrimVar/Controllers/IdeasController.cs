using BirFikrimVar.Models;
using Mapster;
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

            foreach (var idea in ideas)
            {
                idea.LikeCount = await _http.GetFromJsonAsync<int>($"api/LikesApi/count/{idea.IdeaId}");

                idea.CommentCount = await _http.GetFromJsonAsync<int>($"api/CommentsApi/count/{idea.IdeaId}");
            }

            return View(ideas);
        }

        // GET: /Ideas/Details/5
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                // Get the idea
                var idea = await _http.GetFromJsonAsync<IdeaDto>($"api/IdeasApi/{id}");
                if (idea == null)
                {
                    return NotFound();
                }

                // Get comments for this idea
                var comments = await _http.GetFromJsonAsync<List<CommentDto>>($"api/CommentsApi/idea/{id}");

                // Get like count
                var likeCount = await _http.GetFromJsonAsync<int>($"api/LikesApi/count/{id}");

                // Check if current user liked this idea
                bool userLiked = false;
                var userId = GetCurrentUserId();
                if (userId.HasValue)
                {
                    userLiked = await _http.GetFromJsonAsync<bool>($"api/LikesApi/check/{id}/{userId}");
                }

                var viewModel = new IdeaDetailsViewModel
                {
                    Idea = idea,
                    Comments = comments ?? new List<CommentDto>(),
                    LikeCount = likeCount,
                    UserLiked = userLiked
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                // Log the exception
                TempData["Error"] = "Failed to load idea details.";
                return RedirectToAction("Index");
            }
        }
        private int? GetCurrentUserId()
        {
            return HttpContext.Session.GetInt32("UserId");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleLike(int ideaId)
        {
            var userId = GetCurrentUserId(); // Replace with your auth logic
            if (userId == null)
            {
                TempData["Error"] = "You must be logged in to like an idea.";
                return RedirectToAction("Login", "Account");
            }

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
            else if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
            {
                // User already liked → send DELETE request to unlike
                var deleteResponse = await _http.DeleteAsync($"api/LikesApi/{ideaId}/{userId.Value}");
                if (deleteResponse.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Like removed successfully.";
                }
                else
                {
                    TempData["Error"] = "Failed to remove like.";
                }
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                TempData["Error"] = $"Failed to like the idea. {errorContent}";
            }

            // Redirect back to the page where the request came from
            var referer = Request.Headers["Referer"].ToString();
            return Redirect(!string.IsNullOrEmpty(referer) ? referer : Url.Action("Index", "Ideas"));
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
            {
                return View(dto);
            }

            var response = await _http.PutAsJsonAsync($"api/IdeasApi/{id}", dto);

            if (response.IsSuccessStatusCode)
            {
                // Redirect back to profile after successful edit
                var userId = HttpContext.Session.GetInt32("UserId");
                return RedirectToAction("Profile", "Users", new { id = userId });
            }

            // If API fails, show error
            ModelState.AddModelError(string.Empty, "Unable to save changes. Please try again.");
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

        [HttpPost]
        public async Task<IActionResult> AddComment(int ideaId, string content)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue) return RedirectToAction("Login", "Users");

            var dto = new CreateCommentDto
            {
                IdeaId = ideaId,
                UserId = userId.Value,
                Content = content
            };

            await _http.PostAsJsonAsync("api/CommentsApi", dto);
            return RedirectToAction("Details", new { id = ideaId });
        }

    
    }
}
