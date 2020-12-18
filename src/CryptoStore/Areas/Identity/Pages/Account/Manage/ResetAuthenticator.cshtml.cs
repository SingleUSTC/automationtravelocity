namespace CryptoStore.Areas.Identity.Pages.Account.Manage
{
    using System.Threading.Tasks;
    using CryptoStore.Data.Models;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using Microsoft.Extensions.Logging; 

    public class ResetAuthenticatorModel : PageModel
    {
        UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        ILogger<ResetAuthenticatorModel> logger;

        public ResetAuthenticatorModel(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            ILogger<ResetAuthenticatorModel> logger)
        {
            this.userManager = userManager;
            this