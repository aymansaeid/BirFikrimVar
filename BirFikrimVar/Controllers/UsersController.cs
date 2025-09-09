using BirFikrimVar.Models;
using Microsoft.AspNetCore.Mvc;

public class UsersController : Controller
{
    private readonly HttpClient _http;

    public UsersController(IHttpClientFactory httpClientFactory)
    {
        _http = httpClientFactory.CreateClient("ApiClient");
    }

    // GET: /Users
    public async Task<IActionResult> Index()
    {
        var users = await _http.GetFromJsonAsync<List<UserDto>>("api/UsersApi");
        return View(users);
    }

    // GET: /Users/Register
    public IActionResult Register()
    {
        return View();
    }

    // POST: /Users/Register
    [HttpPost]
    public async Task<IActionResult> Register(RegisterUserDto dto)
    {
        var response = await _http.PostAsJsonAsync("api/UsersApi/register", dto);
        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction("Index");
        }
        return View(dto);
    }

    // GET: /Users/Login
    public IActionResult Login()
    {
        return View();
    }

    // POST: /Users/Login
    [HttpPost]
    public async Task<IActionResult> Login(LoginUserDto dto)
    {
        var response = await _http.PostAsJsonAsync("api/UsersApi/login", dto);
        if (response.IsSuccessStatusCode)
        {
            var user = await response.Content.ReadFromJsonAsync<UserDto>();
            // TODO: store user in session or cookie
            TempData["Message"] = $"Welcome {user.FullName}";
            return RedirectToAction("Index");
        }
        ModelState.AddModelError("", "Invalid email or password.");
        return View(dto);
    }

    // GET: /Users/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var user = await _http.GetFromJsonAsync<UserDto>($"api/UsersApi/{id}");
        if (user == null) return NotFound();
        return View(user);
    }

    // POST: /Users/Edit/5
    [HttpPost]
    public async Task<IActionResult> Edit(int id, UpdateUserDto dto)
    {
        var response = await _http.PutAsJsonAsync($"api/UsersApi/{id}", dto);
        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction(nameof(Index));
        }
        return View(dto);
    }

    // GET: /Users/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        var user = await _http.GetFromJsonAsync<UserDto>($"api/UsersApi/{id}");
        if (user == null) return NotFound();
        return View(user);
    }

    // POST: /Users/Delete/5
    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _http.DeleteAsync($"api/UsersApi/{id}");
        return RedirectToAction(nameof(Index));
    }
}
