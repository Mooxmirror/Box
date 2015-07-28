using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServer
{
    public class Request
    {
        private static Dictionary<string, RequestMethod> RequestMethods { get; } = new Dictionary<string, RequestMethod>()
        {
            { "GET", RequestMethod.GET },
            { "POST", RequestMethod.POST },
            { "PUT", RequestMethod.PUT },
            { "DELETE", RequestMethod.DELETE }
        };

        public Dictionary<string, string> Header { get; protected set; } = new Dictionary<string, string>();
        public string HTTPVersion { get; protected set; } = "HTTP/1.1";
        public string Path { get; protected set; } = "/";
        public RequestMethod Method { get; protected set; } = RequestMethod.GET;
        public string Content { get; protected set; } = "";

        /// <summary>
        /// Parses a request to data.
        /// </summary>
        /// <param name="request"></param>
        public void Parse(string request)
        {
            string[] lines = request.Split('\n');
            string[] requestInformation = lines[0].Trim().Split(' ');

            string methodName = requestInformation[0].ToUpper();
            if (!RequestMethods.ContainsKey(methodName))
                Method = RequestMethod.UNKNOWN;
            else
                Method = RequestMethods[methodName];

            Path = requestInformation[1];
            HTTPVersion = requestInformation[2];

            int lineIndex;

            for (lineIndex = 1; lineIndex < lines.Length && lines[lineIndex] != ""; lineIndex++)
            {
                string current = lines[lineIndex];
                int seperatorIndex = current.IndexOf(':');
                string key = current.Substring(0, seperatorIndex).Trim();
                string value = current.Substring(seperatorIndex + 1).Trim();

                Header[key] = value;
            }
        }
        
        /// <summary>
        /// Generates a HTTP request string from the given data.
        /// </summary>
        /// <returns></returns>
        public string ToHTTPString()
        {
            string ind = $"{Enum.GetName(typeof(RequestMethod), Method)} {Path} {HTTPVersion}\n";
            foreach (KeyValuePair<string, string> field in Header)
                ind += $"{field.Key}: {field.Value}\n";
            ind += Content;
            return ind;
        }
    }

    public enum RequestMethod
    {
        GET, POST, PUT, DELETE, HEAD, TRACE, OPTIONS, CONNECT, PATCH, UNKNOWN
    }
}
