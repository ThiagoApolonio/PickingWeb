using Picking_Web.Helpers;
using Picking_Web.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;
using System.Web.Mvc;

namespace Picking_Web.Controllers
{
    [System.Web.Mvc.Authorize]
    public class AccountController : MyController
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public AccountController() { }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Account/Login
        [System.Web.Mvc.AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }



        //
        // POST: /Account/SairdoSistema
        [System.Web.Mvc.AllowAnonymous]
        public ActionResult SairdoSistema(string x)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }       

        // PUT /account/1
        [System.Web.Mvc.HttpPut]
        public IHttpActionResult AtualizarEmpresaEmSessao(int id)
        {
            return new OkResult(new HttpRequestMessage());
        }

        //
        // POST: /Account/Login
        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string user_name = "";
            var user = await UserManager.FindByEmailAsync(model.Email);

            Empresa empresa = null;

            if (user != null)
            {
                user_name = user.UserName;

                if (!user.Ativo)
                {
                    ModelState.AddModelError("", "Usuário inativo.");
                    return View(model);
                }
                else if (!user.Licenciado)
                {
                    ModelState.AddModelError("", "Usuário não possui licença.");
                    return View(model);
                }
                else
                {
                    empresa = HttpContext.GetOwinContext().GetUserManager<ApplicationDbContext>().Empresa.Find(user.EmpresaId);
                    if (empresa == null)
                    {
                        ModelState.AddModelError("", "A Empresa que o usuário foi atribuído não existe.");
                        return View(model);
                    }
                    else if (!empresa.Ativo)
                    {
                        ModelState.AddModelError("", "A Empresa que o usuário foi atribuído está inativa.");
                        return View(model);
                    }
                }
            }


            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(user_name, model.Password, model.RememberMe, shouldLockout: false);

            if (result == SignInStatus.Success)
            {
                
                return RedirectToLocal(returnUrl);
             
            }
            else if (result == SignInStatus.LockedOut)
            {
                return View("Lockout");
            }
            else if (result == SignInStatus.RequiresVerification)
            {
                return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
            }
            else
            {
                ModelState.AddModelError("", "E-mail ou senha não encontradas no sistema.");
                return View(model);
            }
        }

        //
        // GET: /Account/VerifyCode
        [System.Web.Mvc.AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        //
        // GET: /Account/Register
        public ActionResult Editar(string id)
        {
            ApplicationUser user = HttpContext.GetOwinContext().GetUserManager<ApplicationDbContext>().Users
                .Where(i => i.Id == id)
                .Single();

            if (user == null)
            {
                return HttpNotFound();
            }

            var roles = UserManager.GetRolesAsync(id);
            var viewModel = new RegisterViewModel()
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Ativo = user.Ativo,
                Licenciado = user.Licenciado,
                Operador = user.Operador,
                PodeGerenciarAdmin = roles.Result.Contains(Privilegios.PodeGerenciarAdmin),
                PodeGerenciarListaPicking = roles.Result.Contains(Privilegios.PodeGerenciarListaPicking),
                PodeConferirCodigoBarras = roles.Result.Contains(Privilegios.PodeConferirCodigoBarras),
                PodeGerenciarEtiqueta = roles.Result.Contains(Privilegios.PodeGerenciarEtiqueta),
                PodeGerenciarRecebimento = roles.Result.Contains(Privilegios.PodeGerenciarRecebimento),
                PodeGerenciarDocumentos = roles.Result.Contains(Privilegios.PodeGerenciarDocumentos),
                PodeGerenciarBaixa = roles.Result.Contains(Privilegios.PodeGerenciarBaixa),
                Titulo = "Editar Usuário",
                UsuariosSAP = SAPHelper.GetUsuariosSap(HttpContext.GetOwinContext().GetUserManager<ApplicationDbContext>(), user.EmpresaId),
                UsuariosSAPId = user.UsuarioSAPId,
                Empresas = HttpContext.GetOwinContext().GetUserManager<ApplicationDbContext>().Empresa.ToList(),
                EmpresaId = user.EmpresaId,
                QuantidadeLicencas = GlobalHelper.RetornaQuantidadeLicencasDisponiveis(HttpContext.GetOwinContext().GetUserManager<ApplicationDbContext>())
            };

            SetUserIdInViewBag();

            return View("Register", viewModel);
        }

        public ActionResult Register()
        {
            var viewModel = new RegisterViewModel()
            {
                Ativo = true,
                Titulo = "Novo Usuário",
                UsuariosSAP = new List<UsuarioSAP>() { },
                Empresas = HttpContext.GetOwinContext().GetUserManager<ApplicationDbContext>().Empresa.ToList(),
                QuantidadeLicencas = GlobalHelper.RetornaQuantidadeLicencasDisponiveis(HttpContext.GetOwinContext().GetUserManager<ApplicationDbContext>())
            };

            SetUserIdInViewBag();

            return View(viewModel);
        }

        //
        // POST: /Account/Register
        [System.Web.Mvc.HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            bool tem_erro = false;
            if (ModelState.IsValid)
            {
                bool licenca_ok = true;

                if (String.IsNullOrEmpty(model.Id))
                {
                    int qtd_licencas = model.Licenciado ? 1 : 0;
                    licenca_ok = GlobalHelper.PodeAdicionarNovaLicenca(HttpContext.GetOwinContext().GetUserManager<ApplicationDbContext>(), qtd_licencas);
                }
                else
                {
                    licenca_ok = GlobalHelper.PodeAtualizarUsuarioLicenca(HttpContext.GetOwinContext().GetUserManager<ApplicationDbContext>(), model.Id, model.Licenciado);
                }

                if (licenca_ok)
                {
                    // cria as funções/roles
                    var roleStore = new RoleStore<IdentityRole>(new ApplicationDbContext());
                    var roleManager = new RoleManager<IdentityRole>(roleStore);

                    if (String.IsNullOrEmpty(model.Id))
                    {
                        var user = new ApplicationUser
                        {
                            UserName = model.UserName,
                            Email = model.Email,
                            Ativo = model.Ativo,
                            Licenciado = model.Licenciado,
                            Operador = model.Operador,
                            UsuarioSAPId = model.UsuariosSAPId,
                            EmpresaId = model.EmpresaId,
                        };

                        var result = await UserManager.CreateAsync(user, model.Password);
                        if (result.Succeeded)
                        {
                            if (model.PodeGerenciarAdmin)
                            {
                                await UserManager.AddToRoleAsync(user.Id, Privilegios.PodeGerenciarAdmin);
                            }
                            if (model.PodeGerenciarListaPicking)
                            {
                                await UserManager.AddToRoleAsync(user.Id, Privilegios.PodeGerenciarListaPicking);
                            }
                            if (model.PodeConferirCodigoBarras)
                            {
                                await UserManager.AddToRoleAsync(user.Id, Privilegios.PodeConferirCodigoBarras);
                            }
                            if (model.PodeGerenciarEtiqueta)
                            {
                                await UserManager.AddToRoleAsync(user.Id, Privilegios.PodeGerenciarEtiqueta);
                            }
                            if (model.PodeGerenciarRecebimento)
                            {
                                await UserManager.AddToRoleAsync(user.Id, Privilegios.PodeGerenciarRecebimento);
                            }
                            if (model.PodeGerenciarDocumentos)
                            {
                                await UserManager.AddToRoleAsync(user.Id, Privilegios.PodeGerenciarDocumentos);
                            }
                            if (model.PodeGerenciarBaixa)
                            {
                                await UserManager.AddToRoleAsync(user.Id, Privilegios.PodeGerenciarBaixa);
                            }
                        }
                        AddErrors(result);
                    }
                    else
                    {
                        var userToUpdate = HttpContext.GetOwinContext().GetUserManager<ApplicationDbContext>().Users
                            .Where(i => i.Id == model.Id)
                            .Single();

                        userToUpdate.UserName = model.UserName;
                        userToUpdate.Email = model.Email;
                        userToUpdate.Ativo = model.Ativo;
                        userToUpdate.Licenciado = model.Licenciado;
                        userToUpdate.Operador = model.Operador;
                        userToUpdate.UsuarioSAPId = model.UsuariosSAPId;
                        userToUpdate.EmpresaId = model.EmpresaId;

                        HttpContext.GetOwinContext().GetUserManager<ApplicationDbContext>().Entry(userToUpdate).State = EntityState.Modified;
                        await UserManager.UpdateAsync(userToUpdate);

                        if (model.PodeGerenciarAdmin)
                        {
                            await UserManager.AddToRoleAsync(userToUpdate.Id, Privilegios.PodeGerenciarAdmin);
                        }
                        else
                        {
                            await UserManager.RemoveFromRolesAsync(model.Id, Privilegios.PodeGerenciarAdmin);
                        }

                        if (model.PodeGerenciarListaPicking)
                        {
                            await UserManager.AddToRoleAsync(userToUpdate.Id, Privilegios.PodeGerenciarListaPicking);
                        }
                        else
                        {
                            await UserManager.RemoveFromRolesAsync(model.Id, Privilegios.PodeGerenciarListaPicking);
                        }

                        if (model.PodeConferirCodigoBarras)
                        {
                            await UserManager.AddToRoleAsync(userToUpdate.Id, Privilegios.PodeConferirCodigoBarras);
                        }
                        else
                        {
                            await UserManager.RemoveFromRolesAsync(model.Id, Privilegios.PodeConferirCodigoBarras);
                        }

                        if (model.PodeGerenciarEtiqueta)
                        {
                            await UserManager.AddToRoleAsync(userToUpdate.Id, Privilegios.PodeGerenciarEtiqueta);
                        }
                        else
                        {
                            await UserManager.RemoveFromRolesAsync(model.Id, Privilegios.PodeGerenciarEtiqueta);
                        }

                        if (model.PodeGerenciarRecebimento)
                        {
                            await UserManager.AddToRoleAsync(userToUpdate.Id, Privilegios.PodeGerenciarRecebimento);
                        }
                        else
                        {
                            await UserManager.RemoveFromRolesAsync(model.Id, Privilegios.PodeGerenciarRecebimento);
                        }
                        if (model.PodeGerenciarDocumentos)
                        {
                            await UserManager.AddToRoleAsync(userToUpdate.Id, Privilegios.PodeGerenciarDocumentos);
                        }
                        else
                        {
                            await UserManager.RemoveFromRolesAsync(model.Id, Privilegios.PodeGerenciarDocumentos);
                        }
                        if (model.PodeGerenciarBaixa)
                        {
                            await UserManager.AddToRoleAsync(userToUpdate.Id, Privilegios.PodeGerenciarBaixa);
                        }
                        else
                        {
                            await UserManager.RemoveFromRolesAsync(model.Id, Privilegios.PodeGerenciarBaixa);
                        }
                    }
                }
                else
                {
                    IdentityResult idresult = new IdentityResult(new[] { "Quantidade de licenças excede o limite." });
                    AddErrors(idresult);
                    tem_erro = true;
                    model.UsuariosSAP = SAPHelper.GetUsuariosSap(HttpContext.GetOwinContext().GetUserManager<ApplicationDbContext>(), 1);
                    model.Empresas = HttpContext.GetOwinContext().GetUserManager<ApplicationDbContext>().Empresa.ToList();
                }
            }

            if (tem_erro)
            {
                SetUserIdInViewBag();

                // If we got this far, something failed, redisplay form
                return View(model);
            }
            else
            {
                return RedirectToAction("Index", "Usuarios");
            }
        }

        //
        // GET: /Account/ConfirmEmail
        [System.Web.Mvc.AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [System.Web.Mvc.AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                // string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                // var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
                // await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
                // return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [System.Web.Mvc.AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [System.Web.Mvc.AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [System.Web.Mvc.AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [System.Web.Mvc.AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [System.Web.Mvc.AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index","Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff      
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index","Home");
        }    

        //
        // GET: /Account/ExternalLoginFailure
        [System.Web.Mvc.AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region :: Helpers

        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }

        #endregion
    }
}