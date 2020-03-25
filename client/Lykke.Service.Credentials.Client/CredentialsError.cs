using JetBrains.Annotations;

namespace Lykke.Service.Credentials.Client
{
    /// <summary>
    /// Enum for credentils validation error.
    /// </summary>
    [PublicAPI]
    public enum CredentialsError
    {
        /// <summary>No error.</summary>
        None = 0,
        /// <summary>Login not found.</summary>
        LoginNotFound,
        /// <summary>Password mismatch.</summary>
        PasswordMismatch,
        /// <summary>Password mismatch.</summary>
        LoginAlreadyExists,
    }
}
