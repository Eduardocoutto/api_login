using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using TesteAgape.Data;

namespace TesteAgape.Aplicacao
{
    public class UsuarioService
    {
        private HelloAgapeEntities _context;

        private Usuario _usuarioLogado = null;
        public Usuario UsuarioLogado
        {
            get
            {
                return _usuarioLogado;
            }

            set
            {
                _usuarioLogado = value;
            }
        }

        public bool SetUsuarioLogado(int idUsuario)
        {
            if (idUsuario > 0)
            {
                _usuarioLogado = _context.Usuarios.Where(x => x.IdUsuario == idUsuario).FirstOrDefault();
            }
            else
                _usuarioLogado = null;
            return _usuarioLogado != null;
        }

        public UsuarioService()
        {
            _context = new HelloAgapeEntities();
        }

        public Usuario ObterPorId(int idUsuario)
        {
            verificarPermissao(idUsuario);
            if (idUsuario > 0)
            {
                return _context.Usuarios.Where(
                    x => x.IdUsuario == idUsuario).FirstOrDefault();
            }
            else
                return null;
        }

        public Usuario ObterPorLogin(string login)
        {
            if (!String.IsNullOrEmpty(login))
            {
                return _context.Usuarios.Where(
                    x => x.Login == login).FirstOrDefault();
            }
            else
                return null;
        }

        public Usuario ObterPorEmail(string email)
        {
            if (!String.IsNullOrEmpty(email))
            {
                return _context.Usuarios.Where(
                    x => x.Email == email).FirstOrDefault();
            }
            else
                return null;
        }

        public List<Usuario> ListarTodos(string pesquisalogin = "")
        {
            return pesquisarUsuarios(pesquisalogin);
        }

        public object ListarTodosPaginacao(int page, string sort, int tipoSort, string pesquisalogin = "", int PageSize = 10)
        {
            if (pesquisalogin == null)
                pesquisalogin = "";
            List<Usuario> source = pesquisarUsuarios(pesquisalogin);

            int qtdRegistros = source.Count();
            int CurrentPage = page;
            int TotalCount = qtdRegistros;
            int TotalPages = (int)Math.Ceiling(qtdRegistros / (double)PageSize);
            var items = source.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();

            // Object which we are going to send in header   
            var paginationMetadata = new
            {
                data = items,
                totalCount = TotalCount,
                pageSize = PageSize,
                currentPage = CurrentPage,
                totalPages = TotalPages,
            };
            return paginationMetadata;
        }

        private List<Usuario> pesquisarUsuarios(string pesquisalogin)
        {
            return _context.Usuarios
                            .Where(usuario => (
                                        (UsuarioLogado.Admin ? (true) : (usuario.IdUsuario == _usuarioLogado.IdUsuario))
                                        && (pesquisalogin.Length == 0 || usuario.Login.Contains(pesquisalogin))
                            ))
                            .OrderBy(usuario => usuario.Login)
                            .ToList();
        }

        public Usuario Incluir(Usuario dadosUsuario)
        {
            Resultado resultado = DadosValidos(dadosUsuario);

            if (resultado.Inconsistencias.Count == 0)
            {
                dadosUsuario.Senha = EncriptarSenha.GerarHashSenha(dadosUsuario.Senha);
                _context.Usuarios.Add(dadosUsuario);
                _context.SaveChanges();
                return dadosUsuario;
            }
            else
            {
                string error = "";
                resultado.Inconsistencias.ForEach(inconsistencia => error += inconsistencia + " ");
                throw new Exception(error.Trim());
            }
        }

        public bool Atualizar(Usuario dadosUsuario, bool atualizarSenha = false)
        {
            if (dadosUsuario == null)
                throw new Exception("Informe o usuário");

            verificarPermissao(dadosUsuario.IdUsuario);

            Resultado resultado = DadosValidos(dadosUsuario, editando: true);

            if (resultado.Inconsistencias.Count == 0)
            {
                Usuario Usuario = _context.Usuarios.Where(
                     p => p.IdUsuario == dadosUsuario.IdUsuario).FirstOrDefault();

                if (Usuario != null)
                {
                    Usuario.Email = dadosUsuario.Email;
                    Usuario.Login = dadosUsuario.Login;
                    Usuario.Admin = dadosUsuario.Admin;
                    if(atualizarSenha)
                        Usuario.Senha = EncriptarSenha.GerarHashSenha(dadosUsuario.Senha);

                    Usuario.Status = dadosUsuario.Status;
                    _context.SaveChanges();
                    return true;
                }
            }
            else
            {
                string error = "";
                resultado.Inconsistencias.ForEach(inconsistencia => error += inconsistencia + ". ");
                throw new Exception(error.Trim());
            }
            return false;
        }

        public bool Excluir(int IdUsuario)
        {
            verificarPermissao(IdUsuario);
           
            Usuario Usuario = ObterPorId(IdUsuario);
            if (Usuario == null)
            {
                throw new Exception("Usuário não encontrado");
            }
            else
            {
                _context.Usuarios.Remove(Usuario);
                _context.SaveChanges();
                return true;
            }
        }

        public bool EnviarEmailRecuperacao(string emailRecuperacao, string token)
        {
            try
            {
                string urlSite = "http://localhost:51180";
                string emailApp = "envio.teste31@gmail.com";
                string senhaEmailApp = "testeenvioemail";
                
                MailMessage objEmail = new MailMessage();
                objEmail.From = new MailAddress(emailApp, emailApp);
                objEmail.To.Add(emailRecuperacao);
                objEmail.Priority = MailPriority.Normal;
                objEmail.IsBodyHtml = true;
                objEmail.BodyEncoding = Encoding.UTF8;
                objEmail.SubjectEncoding = Encoding.UTF8;

                char aspasDuplas = '\"';
                objEmail.Body = $@"<h2>Recuperar Senha AppLogin</h2><p><a href={aspasDuplas}{urlSite}/login/DefinirSenha?t={token}{aspasDuplas}>Definir uma nova senha</a></p>";
                objEmail.Subject = "Recuperação de senha AppLogin";

                bool enviado = false;
                
                SmtpClient objSmtp = null;
                try
                {
                    objSmtp = new SmtpClient("smtp.gmail.com", 587);
                    objSmtp.Credentials = new NetworkCredential(emailApp, senhaEmailApp);
                    objSmtp.EnableSsl = true;
                    objSmtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                    objSmtp.Send(objEmail);
                    enviado = true;
                }
                finally
                {
                    objEmail.Attachments.Dispose();
                    objEmail.Dispose();

                    if (objSmtp != null)
                        objSmtp.Dispose();
                }
                return enviado;
            }
            catch (Exception erro)
            {
                throw new Exception(erro.Message);
            }
        }

        private void verificarPermissao(int idUsuarioConsulta)
        {
            if (_usuarioLogado.Admin || UsuarioLogado.IdUsuario == idUsuarioConsulta)
                return;

            throw new Exception("Usuário sem permissão");
        }

        private Resultado DadosValidos(Usuario Usuario, bool editando =false)
        {
            var resultado = new Resultado();
            if (Usuario == null)
            {
                resultado.Inconsistencias.Add(
                    "Preencha os Dados do Usuario");
            }
            else
            {
                if (Usuario.IdUsuario == 0)
                {
                    if (_context.Usuarios.Where(
                        usuarioBusca => usuarioBusca.Login == Usuario.Login && usuarioBusca.IdUsuario != Usuario.IdUsuario).Count() > 0)
                        resultado.Inconsistencias.Add("Login já em uso.");
                }

                if (String.IsNullOrWhiteSpace(Usuario.Login))
                {
                    resultado.Inconsistencias.Add(
                        "Preencha o Login.");
                }
                if (String.IsNullOrWhiteSpace(Usuario.Email))
                {
                    resultado.Inconsistencias.Add(
                        "Informe o email.");
                } else 
                    if (_context.Usuarios.Where(
                        usuarioBusca => usuarioBusca.Email == Usuario.Email && usuarioBusca.IdUsuario != Usuario.IdUsuario).Count() > 0)
                        resultado.Inconsistencias.Add("Email já em uso.");
                
                if (!editando && String.IsNullOrWhiteSpace(Usuario.Senha))
                {
                    resultado.Inconsistencias.Add(
                        "Informe a senha.");
                }
            }

            return resultado;
        }


    }
}