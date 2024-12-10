namespace CmdTools.Core.Entities
{
    public class BarerToken
    {
        public string? AccessToken { get; set; } = null;
        public DateTime? AccessTokenExpiration { get; set; } = null;
    }
}
