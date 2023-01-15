using System;
using System.Diagnostics;
using System.Threading.Tasks;
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
        private readonly UserManager<User> _userManager;
        private readonly UserService _userService;
        private readonly SignInManager<User> _signInManager;

        public HomeController(ILogger<HomeController> logger, UserService userService, UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _logger = logger;
            _userService = userService;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Index(HomeIndexViewModel model)
        {
            return View();
        }

        public IActionResult Exchange()
        {
            //User storage'a göre alıncak
            var user = new User();
            
            
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ConvertEthTo42xToken(string eth = "0.2")
        {
            //User storage'a göre alıncak
            var currentEthValue = Math.Round(await GetCryptoValue.GetEthPriceAsync(),2);
            var ourToken = Math.Round(currentEthValue * double.Parse(eth, System.Globalization.CultureInfo.InvariantCulture),2);


            return RedirectToAction("Exchange", "Home");
        }
        [HttpPost]
        public async Task<IActionResult> CreateOrLogin([FromBody] CreateOrLoginModel model)
        {
            //check user has wallet
            var haveUser = _userService.CheckIfUserExists(model.WalletAddress);
            //create new user
            if (!haveUser)
            {
                var user = new User()
                {
                    Wallet = model.WalletAddress,
                    EthAmount = Math.Round(double.Parse(model.EthValue, System.Globalization.CultureInfo.InvariantCulture),2),
                    UserName = model.WalletAddress,
                    Email = "42xbetuser@gmail.com"
                };
                var result = await _userManager.CreateAsync(user, model.WalletAddress);
                var result2= await _signInManager.PasswordSignInAsync(user, model.WalletAddress, true, false);
                return RedirectToAction("Index", "Home");
            }
            var existedUser = await _userManager.FindByNameAsync(model.WalletAddress);
            await _signInManager.PasswordSignInAsync(existedUser, model.WalletAddress, true, false);
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Assets()
        {
            var ethPriceAsync = await GetCryptoValue.GetEthPriceAsync();
            var btcPriceAsync = await GetCryptoValue.GetBtcPriceAsync();
            var model = new AssetViewModel()
            {
                My42Asset = "23",
                MyEthAsset = "123",
                EthPrice = ethPriceAsync.ToString(),
                BtcPrice = btcPriceAsync.ToString()
            };
            return View(model);
        }

        public IActionResult Login()
        {
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
            var model = new RouletteViewModel
            {
                Cash = 2000
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
        public async Task<JsonResult> UpdateRouletteWithRefresh([FromBody] RouletteViewModel model)
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
        public async Task<JsonResult> UpdateBlackjackWithRefresh([FromBody] BlackjackViewmodel model)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            user.AssetTokens = model.LastAsset;
            _userService.Update(user.Id, user);
            return Json(user);
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