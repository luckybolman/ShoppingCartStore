﻿namespace SoppingCartStore.Web.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using ShoppingCartStore.Common.Constants;
    using ShoppingCartStore.Models;
    using ShoppingCartStore.Services.DataServices;
    using System.Threading.Tasks;

    public class CartController : Controller
    {
        private ICartService _cartService;
        private UserManager<Customer> _userManager;

        public CartController(ICartService cartService, UserManager<Customer> userManager)
        {
            _cartService = cartService;
            _userManager = userManager;
        }

        [Authorize]
        public IActionResult Index()
        {
            string customerId = _userManager
                .FindByNameAsync(this.User.Identity.Name).Result.Id;
            var cartViewModel = _cartService.GetCartViewModelByCustomerId(customerId);
            return View(cartViewModel);
        }

        public async Task<IActionResult> AddToCart(string id)
        {
            if (this.User.Identity.IsAuthenticated)
            {
                await _cartService.AddToPersistedCartAsync(id, this.User.Identity.Name);
            }

            await _cartService.AddToSessionCartAsync(id, HttpContext.Session);
            return this.RedirectToAction(ActionConstants.Index, ActionConstants.Products);
        }

        public async Task<IActionResult> RemoveFromCart(string id)
        {
            string customerId = _userManager
                .FindByNameAsync(this.User.Identity.Name).Result.Id;
            await _cartService.RemoveItemFromCartAsync(id, customerId, HttpContext.Session);
            return this.RedirectToPage(PageConstants.OrdersCreate);
        }
    }
}
