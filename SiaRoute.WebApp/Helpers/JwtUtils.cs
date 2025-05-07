using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.JsonWebTokens;

namespace SiaRoute.WebApp.Helpers
{
    public class JwtUtils
    {
        public record DecodedToken(
            string KeyId,
            string Issuer,
            List<string> Audience,
            List<(string Type, string Value)> Claims,
            DateTime ValidTo,
            string SignatureAlgorithm,
            string RawData,
            string Subject,
            DateTime ValidFrom,
            string EncodedHeader,
            string EncodedPayload
        );
        public static JwtSecurityToken ConvertJwtStringToJwtSecurityToken(string? jwt)
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(jwt);

            return token;
        }
        public static DecodedToken DecodeJwt(JwtSecurityToken token)
        {
            var keyId = token.Header.Kid;
            var audience = token.Audiences.ToList();
            var claims = token.Claims.Select(claim => (claim.Type, claim.Value)).ToList();
            return new DecodedToken(
                keyId,
                token.Issuer,
                audience,
                claims,
                token.ValidTo,
                token.SignatureAlgorithm,
                token.RawData,
                token.Subject,
                token.ValidFrom,
                token.EncodedHeader,
                token.EncodedPayload
            );
        }
    }
}
