namespace Pims.Core.Models
{
    public sealed record UserCredentials(string UserId, string PrimaryPassword, string SecondaryPassword);
}
