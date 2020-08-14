using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using TesteAgape.Aplicacao;
using TesteAgape.Data;
using TesteAgape.Extentions;
using System.Web.Http;
using System.Net.Http;
using TesteAgape.Filters;

namespace TesteAgape.Controllers
{
    //[RoutePrefix("api/usuario")]
    public class UsuarioController : ApiController
    {
        
        private UsuarioService _service = new UsuarioService();


        private int GetIdUsuarioLogado(HttpRequestMessage request)
        {
            string token = Request.Headers.Authorization.Parameter;
            if (token != null && token.Length > 0)
                return JwtManager.GetIdUsuario(token);
            else return 0;
        }

        // GET api/usuario
        [System.Web.Http.HttpGet]
        [JwtAuthentication]
        public IHttpActionResult Get()
        {
            try
            {
                _service.SetUsuarioLogado(GetIdUsuarioLogado(Request));

                return Ok(_service.ListarTodos());
            }
            catch (Exception erro)
            {
                return BadRequest(erro.Message);
            }
        }

        [JwtAuthentication]
        [System.Web.Http.HttpGet]
        public IHttpActionResult Get(int id)
        {
            try
            {
                _service.SetUsuarioLogado(GetIdUsuarioLogado(Request));

                var Usuario = _service.ObterPorId(id);
                if (Usuario != null)
                    return Ok(Usuario);
                else
                    return NotFound();
            }
            catch (Exception erro)
            {
                return BadRequest(erro.Message);
            }
        }

        [ActionName("paginacao")]
        [JwtAuthentication]
        [System.Web.Http.HttpGet]
        public IHttpActionResult Paginacao(string search,int limite, int page, string sort, int tipoSort)
        {
            try
            {
                _service.SetUsuarioLogado(GetIdUsuarioLogado(Request));

                var Usuario = _service.ListarTodosPaginacao(page, sort,tipoSort,search,limite);
                if (Usuario != null)
                    return Ok(Usuario);
                else
                    return NotFound();
            }
            catch (Exception erro)
            {
                return BadRequest(erro.Message);
            }
        }

        [ActionName("atualizarsenha")]
        [JwtAuthentication]
        [System.Web.Http.HttpPut]
        public IHttpActionResult AtualizarSenha(string senha)
        {
            try
            {
                _service.SetUsuarioLogado(GetIdUsuarioLogado(Request));

                _service.UsuarioLogado.Senha = senha;

                if (_service.Atualizar(_service.UsuarioLogado, atualizarSenha: true))
                {
                    _service.UsuarioLogado.Senha = "";
                    return Ok(_service.UsuarioLogado);
                }
                    
                return BadRequest();
            }
            catch (Exception erro)
            {
                return BadRequest(erro.Message);
            }
        }

        [AllowAnonymous]
        [ActionName("recuperar")]
        [System.Web.Http.HttpPost]
        public IHttpActionResult Recuperar(string email)
        {
            try
            {
                Usuario usuarioEmail = _service.ObterPorEmail(email);

                if (usuarioEmail == null)
                    return NotFound();

                string token = JwtManager.GenerateToken(usuarioEmail.IdUsuario.ToString());
                
                var emailEnviado = _service.EnviarEmailRecuperacao(email, token);

                if (emailEnviado)
                    return Ok();
                else
                    return NotFound();
            }
            catch (Exception erro)
            {
                return BadRequest(erro.Message);
            }
        }

        [AllowAnonymous]
        [System.Web.Http.HttpPost]
        //[Route("api/usuario")]
        public IHttpActionResult Post([FromBody] Usuario Usuario)
        {
            try
            {
                Usuario usuarioCriado = _service.Incluir(Usuario);
                if (usuarioCriado != null && usuarioCriado.IdUsuario > 0)
                    return Ok(usuarioCriado);
                return BadRequest();
            }
            catch (Exception erro)
            {
                return BadRequest(erro.Message);
            }
        }

        [JwtAuthentication]
        [System.Web.Http.HttpPut]
        public IHttpActionResult Put([FromBody] Usuario Usuario)
        {
            try
            {
                _service.SetUsuarioLogado(GetIdUsuarioLogado(Request));

                if (_service.Atualizar(Usuario))
                    return Ok();
                return BadRequest();
            }
            catch (Exception erro)
            {
                return BadRequest(erro.Message);
            }
        }


        [JwtAuthentication]
        [System.Web.Http.HttpDelete]
        public IHttpActionResult Delete(int id)
        {
            try
            {
                _service.SetUsuarioLogado(GetIdUsuarioLogado(Request));

                if (_service.Excluir(id))
                    return Ok();
                return BadRequest();
            }
            catch (Exception erro)
            {
                return BadRequest(erro.Message);
            }
        }
    }
}
