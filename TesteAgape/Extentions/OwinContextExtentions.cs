using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Linq;
using Microsoft.Owin;
using System.Security.Claims;
using TesteAgape.Provider;
using TesteAgape.Data;

namespace TesteAgape.Extentions
{
    public static class OwinContextExtentions
    {
        public static int GetUserId(this IOwinContext ctx)
        {
            var result = 0;
            var claim = ctx.Authentication.User.Claims.FirstOrDefault(c => c.Type == "UserID");
            if (claim != null && claim.Value != null)
            {
                result = Convert.ToInt32(claim.Value);
            }
            return result;
        }

        public static bool UserIsAdmin(this IOwinContext ctx)
        {
            var result = false;
            var claim = ctx.Authentication.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);
            if (claim != null)
            {
                result = claim.Value == RolesConvention.Administrator;
            }
            return result;
        }

        public static string GetUserEmail(this IOwinContext ctx)
        {
            var result = "";
            var claim = ctx.Authentication.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
            if (claim != null)
            {
                result = claim.Value;
            }
            return result;
        }

        public static Usuario GetUser(this IOwinContext ctx)
        {
            Usuario usuario = new Usuario();
            usuario.Admin = UserIsAdmin(ctx);
            usuario.IdUsuario = GetUserId(ctx);
            usuario.Email = GetUserEmail(ctx);
            return usuario;
        }
    }
}