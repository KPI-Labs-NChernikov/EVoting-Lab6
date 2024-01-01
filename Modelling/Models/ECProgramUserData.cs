namespace Modelling.Models;
public sealed class ECProgramUserData
{
    public string Login { get; }
    public string Password { get; }
    public Token Token { get; }

    public ECProgramUserData(string login, string password, Token token)
    {
        Login = login;
        Password = password;
        Token = token;
    }
}
