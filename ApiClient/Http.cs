using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;

namespace Screeps.ApiClient
{
    public class Http
    {
        public delegate void CompletedHandler(HttpWebResponse response);
        public event CompletedHandler OnCompleted = (r) => { };
        private WebHeaderCollection requestHeader = new WebHeaderCollection();
        private WebHeaderCollection responseHeader = new WebHeaderCollection();
        private JavaScriptSerializer js;
        public delegate object DeserializeHandler(string response);
        public DeserializeHandler Deserializer = null;
        private NetworkCredential credential = null;
        public string UserAgent = null;

        public Http()
        {
            js = new JavaScriptSerializer();
        }

        private object Deserialize(string response)
        {
            if (Deserializer != null)
                return Deserializer.DynamicInvoke(response);
            else return response;
        }

        public void SetHeader(string name, string value)
        {
            if (requestHeader[name] == null)
                requestHeader.Add(name, value);
            else requestHeader[name] = value;
        }
        public string GetHeader(string name)
        {
            return responseHeader[name];
        }
        public string UpdateHeader(string name)
        {
            var h = GetHeader(name);
            if (h != null) SetHeader(name, h);
            return h;
        }

        public void SetCredential(string username, string password)
        {
            credential = new NetworkCredential(username, password);
        }

        public object Post(string baseUrl, string path, object postData)
        {
            var json = js.Serialize(postData);
            return Post(baseUrl + path, json);
        }
        public object Post(string baseUrl, string path, string json)
        {
            return Post(baseUrl + path, json);
        }
        public object Post(string url, object postData)
        {
            var json = js.Serialize(postData);
            return Post(url, json);
        }
        public object Post(string url, string json)
        {
            // create a request
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.Headers = requestHeader;
            if (!string.IsNullOrEmpty(UserAgent))
                httpWebRequest.UserAgent = UserAgent;
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            if (credential != null)
                httpWebRequest.Credentials = credential;

            if (!string.IsNullOrWhiteSpace(json))
            {
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(json);
                }
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            string result = null;
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                result = streamReader.ReadToEnd();
            }
            responseHeader = httpResponse.Headers;
            OnCompleted(httpResponse);
            return Deserialize(result);
        }

        public object Get(string baseUrl, string path, params UrlParam[] args)
        {
            return Get(baseUrl + path, args);
        }
        public object Get(string url, params UrlParam[] args)
        {
            // create a request
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(Url(url, args));
            httpWebRequest.Headers = requestHeader;
            if (!string.IsNullOrEmpty(UserAgent))
                httpWebRequest.UserAgent = UserAgent;
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "GET";
            if (credential != null)
                httpWebRequest.Credentials = credential;

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            string result = null;
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                result = streamReader.ReadToEnd();
            }
            responseHeader = httpResponse.Headers;
            OnCompleted(httpResponse);
            return Deserialize(result);
        }

        private string Url(string path, params UrlParam[] args)
        {
            if (args != null && args.Length > 0)
            {
                StringBuilder url = new StringBuilder(path);
                url.Append("?");
                url.Append(
                    string.Join("&",
                        args.Select(
                            arg => string.Format("{0}={1}", arg.Key, arg.Value))));
                return url.ToString();
            }
            else return path;
        }
   
    }

    public class UrlParam
    {
        public string Key;
        public object Value;
        public UrlParam(string key, object value)
        {
            Key = key;
            Value = value;
        }
    }
}
