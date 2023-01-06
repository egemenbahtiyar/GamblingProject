using System;
using System.Diagnostics;
using System.Threading.Tasks;
using GamblingProject.Helpers.Extensions;
using GamblingProject.Models;
using GamblingProject.Services;
using GamblingProject.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GamblingProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserService _userService;

        public HomeController(ILogger<HomeController> logger, UserService userService)
        {
            _logger = logger;
            _userService = userService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Blackjack()
        {
            var user = _userService.Get("63aa8b7c88048b81af783d53");
            var model = new BlackjackViewmodel
            {
                Cash = Math.Floor(user.Tokens)
            };

            return View(model);
        }

        public IActionResult Roulette()
        {
            var user = _userService.Get("63aa8b7c88048b81af783d53");
            var model = new RouletteViewModel()
            {
                Cash = Math.Floor(user.Tokens)
            };
            return View(model);
        }
        
        [HttpPost]
        public IActionResult UpdateRouletteAsset(RouletteViewModel model)
        {
            var user = _userService.Get("63aa8b7c88048b81af783d53");
            user.Tokens = model.LastAsset;
            _userService.Update(user.Id, user);
            return RedirectToAction("Index", "Home");
        }
        [Consumes("application/json")]
        [HttpPost]
        public JsonResult UpdateRouletteWithRefresh([FromBody]RouletteViewModel model)
        {
            var user = _userService.Get("63aa8b7c88048b81af783d53");
            user.Tokens = model.LastAsset;
            _userService.Update(user.Id, user);
            return Json(user);
        }
        
        [HttpPost]
        public IActionResult UpdateBlackjackAsset(BlackjackViewmodel model)
        {
            var user = _userService.Get("63aa8b7c88048b81af783d53");
            user.Tokens = model.LastAsset;
            _userService.Update(user.Id, user);
            return RedirectToAction("Index", "Home");
        }
        
        [Consumes("application/json")]
        [HttpPost]
        public JsonResult UpdateBlackjackWithRefresh([FromBody]BlackjackViewmodel model)
        {
            var user = _userService.Get("63aa8b7c88048b81af783d53");
            user.Tokens = model.LastAsset;
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