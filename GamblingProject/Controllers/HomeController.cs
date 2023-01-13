using System;
using System.Diagnostics;
using System.Threading.Tasks;
using GamblingProject.Helpers.Extensions;
using GamblingProject.Models;
using GamblingProject.Services;
using GamblingProject.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GamblingProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserService _userService;
        private readonly UserManager<User> _userManager;

        public HomeController(ILogger<HomeController> logger, UserService userService, UserManager<User> userManager)
        {
            _logger = logger;
            _userService = userService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = new User()
            {
                Email = "hhhhhh51@gmail.com",
                AssetTokens = 300,
                UserName = "ege36",
            };
            return View();
        }

        public async Task<IActionResult> Blackjack()
        {
            var user = await _userManager.FindByIdAsync("02aee5d7-4695-4eed-84a9-66c08fafba5e");
            var model = new BlackjackViewmodel
            {
                Cash = Math.Floor(user.AssetTokens)
            };

            return View(model);
        }

        public async Task<IActionResult> Roulette()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var model = new RouletteViewModel()
            {
                Cash = Math.Floor(user.AssetTokens)
            };
            return View(model);
        }
        
        [HttpPost]
        public async Task<IActionResult> UpdateRouletteAsset(RouletteViewModel model)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            user.AssetTokens = model.LastAsset;
            _userService.Update(user.Id, user);
            return RedirectToAction("Index", "Home");
        }
        [Consumes("application/json")]
        [HttpPost]
        public async Task<JsonResult> UpdateRouletteWithRefresh([FromBody]RouletteViewModel model)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            user.AssetTokens = model.LastAsset;
            _userService.Update(user.Id, user);
            return Json(user);
        }
        
        [HttpPost]
        public async Task<IActionResult> UpdateBlackjackAsset(BlackjackViewmodel model)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            user.AssetTokens = model.LastAsset;
            _userService.Update(user.Id, user);
            return RedirectToAction("Index", "Home");
        }
        
        [Consumes("application/json")]
        [HttpPost]
        public async Task<JsonResult> UpdateBlackjackWithRefresh([FromBody]BlackjackViewmodel model)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            user.AssetTokens = model.LastAsset;
            _userService.Update(user.Id, user);
            return Json(user);
        }

        public async Task<IActionResult> Exchange()
        {
            var result = await _userService.ConvertEthToTokens("63aa8b7c88048b81af783d53", 100);
            if (result.Status == "success")
                TempData.Put("message", new AlertMessage
                {
                    Title = result.Title,
                    Message = result.Message,
                    AlertType = "success"
                });
            else
                TempData.Put("message", new AlertMessage
                {
                    Title = result.Title,
                    Message = result.Message,
                    AlertType = "danger"
                });
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}