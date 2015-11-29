using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Web;

namespace servicewpf
{
    class PushNotifier
    {
        public enum PushType
        {
            Badge,
            Tile,
            Toast
        };

        private string _uri;
        private string _accessToken;

        public PushNotifier(string uri, string accessToken)
        {
            _uri = uri;
            _accessToken = accessToken;
        }

        public bool SendNotification(PushType pushType, string text, string imageSource)
        {
            bool result = false;

            try
            {
                string rawType = "wns/";
                string notificationContent = String.Empty;

                switch (pushType)
                {
                    case PushType.Badge:
                        rawType += "toast";
                        string toast1 = "<?xml version=\"1.0\" encoding=\"utf-8\"?> ";
                        string toast2 = @"<toast>
                        <visual>
                            <binding template=""ToastText02"">
                                <text id=""1"">Quai la</text>
                                <text id=""2"">quai la</text>
                            </binding>
                        </visual>
                    </toast>";
                        notificationContent = toast1 + toast2;
                        break;
                    case PushType.Tile:
                        rawType += "tile";
                        notificationContent = String.Format("<?xml version='1.0' encoding='utf-8'?><tile><visual lang=\"en-US\">< binding template =\"TileWideSmallImageAndText03\"><image id=\"1\" src=\"{0}\"/><text id=\"1\">{1}</text></ binding ></ visual ></ tile > ", imageSource, text);
                        break;
                    case PushType.Toast:
                        rawType += "toast";
                        notificationContent = String.Format("<?xml version='1.0' encoding='utf-8'?><toast><visual>< binding template =\"ToastImageAndText01\"><image id=\"1\" src=\"{0}\" alt=\"Placeholder image\"/>< text id =\"1\">{1}</text></binding></visual></toast>", imageSource, text);
                        break;
                    default:
                        break;
                }

                var subscriptionUri = new Uri(_uri);
                var request = (HttpWebRequest)WebRequest.Create(subscriptionUri);
                request.Method = "POST";
                request.Headers = new WebHeaderCollection();
                request.Headers.Add("X-WNS-Type", rawType);
                request.Headers.Add("Authorization", "Bearer " + _accessToken);
                byte[] notificationMessage = Encoding.Default.GetBytes(notificationContent);
                request.ContentLength = notificationMessage.Length;

                using (Stream requestStream = request.GetRequestStream())
                {
                    requestStream.Write(notificationMessage, 0, notificationMessage.Length);
                }

                var response = (HttpWebResponse)request.GetResponse();

                result = (response.StatusCode == HttpStatusCode.OK);
            }
            catch (Exception ex )
            {
               
                result = false;
            }

            return result;
        }
    }
}
