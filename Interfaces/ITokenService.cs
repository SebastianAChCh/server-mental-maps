using server_mental_maps.routes;

public interface ITokenService
{
    public string RenewToken(string token);

    public string CreateRefreshToken(TokenGeneration userInfo);
    public string CreateAccessToken(TokenGeneration userInfo);
    
}