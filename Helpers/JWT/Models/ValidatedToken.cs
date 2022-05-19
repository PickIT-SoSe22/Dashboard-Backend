public class ValidatedToken
{
    public bool Valid { get; set; }
    public string Issuer { get; set; }

    public string Username { get; set; }

    public string Message { get; set; }
}