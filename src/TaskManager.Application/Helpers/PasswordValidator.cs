namespace TaskManager.Application.Helpers;

public static class PasswordValidator
{
    public static bool IsPasswordComplex(string password)
    {
        // Minimum length
        return password.Length >= 8 &&
               // Require at least one special character
               (password.Any(char.IsSymbol) || password.Any(char.IsPunctuation)) &&
               // Require at least one uppercase letter
               password.Any(char.IsUpper) &&
               // Require at least one digit
               password.Any(char.IsDigit);
    }

}