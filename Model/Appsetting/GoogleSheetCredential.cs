using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Model.Appsetting
{
    public class GoogleSheetCredential
    {
        [JsonPropertyName("installed")]
        public Installed Installed { get; set; }
    }

    public class Installed
    {
        [JsonPropertyName("client_id")]
        public string Client_id { get; set; }
        [JsonPropertyName("project_id")]
        public string Project_id { get; set; }
        [JsonPropertyName("auth_uri")]
        public string Auth_uri { get; set; }
        [JsonPropertyName("token_uri")]
        public string Token_uri { get; set; }
        [JsonPropertyName("auth_provider_x509_cert_url")]
        public string Auth_provider_x509_cert_url { get; set; }
        [JsonPropertyName("client_secret")]
        public string Client_secret { get; set; }
        [JsonPropertyName("redirect_uris")]
        public string[] Redirect_uris { get; set; }
    }
}
