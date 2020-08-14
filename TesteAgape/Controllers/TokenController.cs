using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using TesteAgape.Aplicacao;
using TesteAgape.Data;
using TesteAgape.Extentions;
using System.Web.Http;
using System.Net.Http;
using System.Net;
using System.Security.Claims;
using TesteAgape.Provider;
using Newtonsoft.Json;

namespace TesteAgape.Controllers
{
    public class TokenController : ApiController
    {
        // THis is naive endpoint for demo, it should use Basic authentication to provide token or POST request
        [AllowAnonymous]
        public IHttpActionResult Get(string username, string password)
        {
            JwtResult objJwtResult = new JwtResult();
            try
            {
                UsuarioService usuarioService = new UsuarioService();
                Usuario usuario = usuarioService.ObterPorLogin(username);
                

                if (usuario != null && EncriptarSenha.CompararSenhas(password, usuario.Senha))
                {
                    string token = JwtManager.GenerateToken(usuario.IdUsuario.ToString());

                    objJwtResult.access_token = token;

                    return Ok(objJwtResult);
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (Exception erro)
            {
                return BadRequest(erro.Message);
            }
        }
    }
}
