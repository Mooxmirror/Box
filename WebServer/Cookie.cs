using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServer
{
    /// <summary>
    /// Representing cookies. Om nom nom.
    /// </summary>
    public class Cookie
    {
        public string Name { set; get; } = "";
        public string Value { set; get; } = "";
        public int MaxAge { set; get; } = -1;
        public DateTime ExpiryTime { set; get; }
        public string Domain { set; get; } = "";
        public string Path { set; get; } = "";
        public bool Secure { set; get; } = false;
        public bool HttpOnly { set; get; } = false;

        /// <summary>
        /// Creates a new empty cookie.
        /// </summary>
        public Cookie() { }
        /// <summary>
        /// Creates a new cookie with the specified name and value;
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public Cookie(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public string ToHeaderValue()
        {
            string result = $"{Name}={Value}";

            if (MaxAge != -1)
                result += $"; Max-Age={MaxAge}";

            if (ExpiryTime != null)
                result += $"; expires={ExpiryTime.ToUniversalTime().ToString("r")}";

            if (Domain != "")
                result += $"; domain={Domain}";

            if (Path != "")
                result += $"; path={Path}";

            if (Secure)
                result += "; secure";

            if (HttpOnly)
                result += "; HttpOnly";

            return result;
        }
    }
}
