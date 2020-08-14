using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Infrastructure;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using TesteAgape.Aplicacao;
using TesteAgape.Data;
using TesteAgape.Provider;

namespace TesteAgape
{
    public class OAuthAppProvider : OAuthAuthorizationServerProvider
    {
        public override Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            return Task.Factory.StartNew(() =>
            {
                context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });
                
                var username = context.UserName;
                var password = context.Password;
                
                UsuarioService usuarioService = new UsuarioService();
                Usuario usuario = usuarioService.ObterPorLogin(username);

                if (usuario != null && EncriptarSenha.CompararSenhas(context.Password, usuario.Senha))
                {
                    List<Claim> claims = gerarClaimsPorUsuario(usuario);

                    ClaimsIdentity oAutIdentity = new ClaimsIdentity(claims, authenticationType: "");
                    context.Validated(new AuthenticationTicket(oAutIdentity, new AuthenticationProperties() { }));
                }
                else
                {
                    context.SetError("invalid_grant", "Login inválido.");
                }
            });
        }

        private static List<Claim> gerarClaimsPorUsuario(Usuario usuario)
        {
            return new List<Claim>()
                    {
                        new Claim(ClaimTypes.Name, usuario.Login),
                        new Claim("UserID", usuario.IdUsuario.ToString()),
                        new Claim(ClaimTypes.Email, usuario.Email),
                        new Claim(ClaimTypes.Role, usuario.Admin ? RolesConvention.Administrator : RolesConvention.Default)
                    };
        }

        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            if (context.ClientId == null)
            {
                context.Validated();
            }
            return Task.FromResult<object>(null);
        }
    }
}