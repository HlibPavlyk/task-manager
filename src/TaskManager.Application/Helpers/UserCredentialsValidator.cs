namespace TaskManager.Application.Helpers;

public static class UserCredentialsValidator
{
    // Validates if the password meets basic complexity requirements.
    public static bool IsPasswordComplex(string password)
    {
        // Check for minimum length of 8 characters.
        return password.Length >= 8 &&
               // Require at least one special character or punctuation.
               (password.Any(char.IsSymbol) || password.Any(char.IsPunctuation)) &&
               // Require at least one uppercase letter.
               password.Any(char.IsUpper) &&
               // Require at least one digit.
               password.Any(char.IsDigit);
    }

    // Validates if the provided email is in the correct format.
    public static bool IsCredentialEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email)) return false;

        try
        {
            // Try creating a MailAddress instance to verify email format.
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false; // Return false if email is invalid.
        }
    }
    
    // Validates if the username consists of a single word (no spaces).
    public static bool IsSingleWordUsername(string username)
    {
        if (string.IsNullOrWhiteSpace(username)) return false;

        // Returns true if there are no spaces in the username.
        return !username.Contains(' ');
    }
}
