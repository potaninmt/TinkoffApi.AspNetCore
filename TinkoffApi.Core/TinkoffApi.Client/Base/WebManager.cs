using System.IO;
using System.Net;

namespace TinkoffApi.Client.Base
{
    internal class WebManager
    {
        public static string CreateGetRequest(string url)
        {
            var request = WebRequest.Create(url);
            var responseText = GetResponseText(request);

            return responseText;

        }

        public static string GetResponseText(WebRequest request)
        {
            using (var stream = new StreamReader(request.GetResponse().GetResponseStream()))
            {
                return stream.ReadToEnd();
            }
        }
    }
}
