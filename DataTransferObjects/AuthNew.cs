using System.Diagnostics.CodeAnalysis;

public class AuthNew
{
    [NotNull] 
    public string Username { get; set; }

    [NotNull] 
    public string Password { get; set; }
    
    [NotNull]
    public string Email { get; set; }
}