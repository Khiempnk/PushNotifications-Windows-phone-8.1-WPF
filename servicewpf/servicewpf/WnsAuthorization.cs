
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Net;
using System.Web;
using System.IO;
using System.Runtime.Serialization.Json;

namespace servicewpf
{
    class WnsAuthorization
    {
        [DataContract]
        public class OAuthToken
        {
            [DataMember(Name = "access_token")]
            public string AccessToken { get; set; }
            [DataMember(Name = "token_type")]
            public string TokenType { get; set; }
        }

        public static OAuthToken GetOAuthToken(string secretKey, string sid)
        {
            secretKey = "EsaNc0zagro6UghERCW8IvGmhddreXt8";
            sid = "ms-app://s-1-15-2-4192832740-1946729845-2184712059-3221769376-2003405578-3760119916-4211362";
            string encodedSid = HttpUtility.UrlEncode(String.Format("{0}", sid));
            string encodedSecretKey = HttpUtility.UrlEncode(secretKey);

            string message = String.Format("grant_type=client_credentials&client_id={0}&client_secret={1}&scope=notify.windows.com", sid, secretKey);
            WebClient client = new WebClient();
            client.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

            //     string response = client.UploadString("https://login.live.com/accesstoken.srf", message);
            using (var clients = new WebClient())
            {
                clients.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                var response = clients.UploadString("https://login.live.com/accesstoken.srf", message);

                OAuthToken oAuthToken;

                using (MemoryStream memStream = new MemoryStream(Encoding.Unicode.GetBytes(response)))
                {
                    DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(OAuthToken));
                    oAuthToken = (OAuthToken)jsonSerializer.ReadObject(memStream);
                }
                return oAuthToken;
            }
        }
    }
}
