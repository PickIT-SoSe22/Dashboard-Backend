public static class RandomHelper
{
    public static string RandomString(int length = 0)
    {
        var random = new Random();
        if (length == 0)
        {
            length = random.Next(30, 100);
        }

        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var stringChars = new char[length];

        for (var i = 0; i < stringChars.Length; i++)
        {
            stringChars[i] = chars[random.Next(chars.Length)];
        }

        return new string(stringChars);
    }
}