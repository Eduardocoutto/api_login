
namespace TesteAgape.Aplicacao
{
    public static class EncriptarSenha
    {
        private static readonly string salt = "^YaGa$pe~JJ";
        public static string GerarHashSenha(string userPassword)
        {
            string pwdToHash = userPassword + salt;
            return BCrypt.Net.BCrypt.HashPassword(pwdToHash, BCrypt.Net.BCrypt.GenerateSalt());
        }
        public static bool CompararSenhas(string senha, string hashBancoDeDados)
        {
            return BCrypt.Net.BCrypt.Verify(senha + salt, hashBancoDeDados);
        }
    }
}