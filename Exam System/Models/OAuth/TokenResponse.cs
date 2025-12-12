using System.Text.Json.Serialization;

namespace ExamSystem.Models.OAuth
{
    public class TokenResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken{ get; set; }
        [JsonPropertyName("id_token")]
        public string IdToken{ get; set; }
        [JsonPropertyName("expires_in")]
        public int ExpiresIn{ get; set; }
    }

}
