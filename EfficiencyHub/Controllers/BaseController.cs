﻿using EfficiencyHub.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EfficiencyHub.Web.Controllers
{
    public abstract class BaseController : Controller
    {
        protected readonly ILogger<BaseController> _logger;
        protected readonly UserManager<ApplicationUser> _userManager;

        protected BaseController(ILogger<BaseController> logger, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _userManager = userManager;
        }

        // Метод за извличане на текущия потребител
        protected async Task<ApplicationUser?> GetCurrentUserAsync()
        {
            return await _userManager.GetUserAsync(User);
        }

        // Общ метод за логване на грешки
        protected void LogError(string message, Exception ex)
        {
            _logger.LogError(ex, message);
        }
    }
}