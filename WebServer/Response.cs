using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServer
{
    /// <summary>
    /// Represents a response from the server.
    /// </summary>
    public class Response
    {

        /// <summary>
        /// The header fields of the response.
        /// </summary>
        public ResponseHeader Header { get; protected set; }
        /// <summary>
        /// The content of the response stored in bytes.
        /// </summary>
        private byte[] Content;
        /// <summary>
        /// Appends the text to the content of the response:
        /// </summary>
        /// <param name="text"></param>
        public void Write(string text)
        {
            Encoding encoding = new UTF8Encoding(true, true);
            byte[] bytes = encoding.GetBytes(text);
            Write(bytes);
        }
        /// <summary>
        /// Appends the bytes to the content of the response.
        /// </summary>
        /// <param name="bytes"></param>
        public void Write(byte[] bytes)
        {
            int startIndex = Content.Length;
            int arrayLength = startIndex + bytes.Length;

            Array.Resize(ref Content, arrayLength);

            // copy the content into the array
            for (int i = startIndex; i < arrayLength; i++)
                Content[i] = bytes[i - startIndex];

            Header.ContentLength = arrayLength;
        }

        public void ClearContent()
        {
            Content = new byte[0];
            Header.ContentLength = 0;
        }

        public byte[] GetContentBytes() => Content;
        public byte[] GetHeaderBytes()
        {
            string header = Header.ToHTTPHeader() + "\n";
            Encoding encoding = new UTF8Encoding(true, true);
            return encoding.GetBytes(header);
        }

        /// <summary>
        /// Generates a byte array from the header and the content.
        /// </summary>
        /// <returns></returns>
        public byte[] GetBytes()
        {
            byte[] contentBytes = GetContentBytes();
            byte[] headerBytes = GetHeaderBytes();
            byte[] responseBytes = new byte[headerBytes.Length + contentBytes.Length];

            // copy them into the array
            Buffer.BlockCopy(headerBytes, 0, responseBytes, 0, headerBytes.Length);
            Buffer.BlockCopy(contentBytes, 0, responseBytes, headerBytes.Length, contentBytes.Length);

            return responseBytes;
        }

        /// <summary>
        /// Returns the response as a UTF-8 string.
        /// </summary>
        /// <returns></returns>
        public string GetText()
        {
            Encoding encoding = new UTF8Encoding(true, true);
            return encoding.GetString(GetBytes());
        }

        public Response()
        {
            Content = new byte[0];
            Header = new ResponseHeader();
        }
    }
    /// <summary>
    /// The header of a server response.
    /// </summary>
    public class ResponseHeader
    {
        // a collection of standard http status codes
        private static Dictionary<int, string> HTTPStatusCodes { get; } = new Dictionary<int, string>()
        {
            { 100, "Continue" },
            { 101, "Swiching Protocols" },
            { 102, "Processing" },
            { 200, "OK" },
            { 201, "Created" },
            { 202, "Accepted" },
            { 203, "Non-Authoritative Information" },
            { 204, "No Content" },
            { 205, "Reset Content" },
            { 206, "Partial Content" },
            { 207, "Multi-Status" },
            { 208, "Already Reported" },
            { 226, "IM Used" },
            { 300, "Multiple Choices" },
            { 301, "Moved Permanently" },
            { 302, "Found" },
            { 303, "See Other" },
            { 304, "Not Modified" },
            { 305, "Use Proxy" },
            { 306, "Switch Proxy" },
            { 307, "Temporary Redirect" },
            { 308, "Permanent Redirect" },
            { 400, "Bad Request" },
            { 401, "Unauthorized" },
            { 402, "Payment Required" },
            { 403, "Forbidden" },
            { 404, "Not Found" },
            { 405, "Method Not Allowed" },
            { 406, "Not Acceptable" },
            { 407, "Proxy Authentication Required" },
            { 408, "Request Timeout" },
            { 409, "Conflict" },
            { 410, "Gone" },
            { 411, "Length Required" },
            { 412, "Precondition Failed" },
            { 413, "Payload Too Large" },
            { 414, "Request-URI Too Long" },
            { 415, "Unsupported Media Type" },
            { 416, "Requested Range Not Satisfiable" },
            { 417, "Expectation Failed" },
            { 418, "I'm a teapot" },
            { 419, "Authentication Timeout" },
            { 421, "Misdirected Request" },
            { 422, "Unprocessable Entity" },
            { 423, "Locked" },
            { 424, "Failed Dependecy" },
            { 426, "Upgrade Required" },
            { 428, "Precondition Required" },
            { 429, "Too Many Requests" },
            { 431, "Request Header Fields Too Large" },
            { 500, "Internal Server Error" },
            { 501, "Not Implemented" },
            { 502, "Bad Gateway" },
            { 503, "Service Unavailable" },
            { 504, "Gateway Timeout" },
            { 505, "HTTP Version Not Supported" },
            { 506, "Variant Also Negotiates" },
            { 507, "Insufficient Storage" },
            { 508, "Loop Detected" },
            { 509, "Bandwith Limit Exceeded" },
            { 510, "Not Extended" },
            { 511, "Network Authentication Required" },
            { 520, "Unknown Error" }
        };
        public Dictionary<string, string> Fields { get; protected set; }

        /// <summary>
        /// The date and time the message was sent.
        /// </summary>
        public DateTime Date
        {
            set
            {
                Fields["Date"] = value.ToUniversalTime().ToString("r");
            }
            get
            {
                return DateTime.Parse(Fields["Date"]);
            }
        }

        /// <summary>
        /// The date and time the resource expires.
        /// </summary>
        public DateTime ExpiryDate
        {
            set
            {
                Fields["Expires"] = value.ToUniversalTime().ToString("r");
            }
            get
            {
                return DateTime.Parse(Fields["Expires"]);
            }
        }


        /// <summary>
        /// The MIME-type of the content.
        /// </summary>
        public string ContentType
        {
            set
            {
                Fields["Content-Type"] = value;
            }
            get
            {
                return Fields["Content-Type"];
            }
        }

        /// <summary>
        /// The length of the content measured in bytes.
        /// </summary>
        public int ContentLength
        {
            set
            {
                Fields["Content-Length"] = value.ToString();
            }
            get
            {
                return int.Parse(Fields["Content-Length"]);
            }
        }
        
        /// <summary>
        /// The status code of the response.
        /// </summary>
        public string StatusCode
        {
            set
            {
                Fields["Status"] = value;
            }
            get
            {
                return Fields["Status"];
            }
        }

        /// <summary>
        /// The location of the redirect or newly created resource.
        /// </summary>
        public string Location
        {
            set
            {
                Fields["Location"] = value;
            }
            get
            {
                return Fields["Location"];
            }
        }

        /// <summary>
        /// Sets the connection status to close or keep-alive.
        /// </summary>
        public string Connection
        {
            set
            {
                Fields["Connection"] = value;
            }
            get
            {
                return Fields["Connection"];
            }
        }

        /// <summary>
        /// The cookies set by this header.
        /// </summary>
        public List<Cookie> Cookies { get; set; } = new List<Cookie>();

        public ResponseHeader()
        {
            Fields = new Dictionary<string, string>();
            Fields["Server"] = "BOX/0.1";
            SetStatus(200);
        }

        /// <summary>
        /// Sends a refresh header.
        /// </summary>
        /// <param name="seconds">Time waiting before refresh</param>
        /// <param name="url">Target url</param>
        public void SetRefresh(int seconds, string url) => Fields["Refresh"] = $"{seconds}; url={url}";

        /// <summary>
        /// Marks the response as a file download.
        /// </summary>
        /// <param name="filename">The specified filename</param>
        public void MarkAsDownload(string filename) => Fields["Content-Disposition"] = $"attachement; filename={filename}";

        public void SetStatus(int code) => Fields["Status"] = GetStatusMessage(code);
        public string GetStatusMessage(int code) => $"{code} {HTTPStatusCodes[code]}";

        /// <summary>
        /// Generates a HTTP header string from the given data.
        /// </summary>
        /// <returns></returns>
        public string ToHTTPHeader()
        {
            string response = $"HTTP/1.1 {StatusCode}\n";

            // append header fields
            foreach (KeyValuePair<string, string> entry in Fields)
                response += $"{entry.Key}: {entry.Value}\n";

            // append set cookie stuff ^^
            foreach (Cookie cookie in Cookies)
            {
                response += $"Set-Cookie: {cookie.ToHeaderValue()}\n";
            }

            return response;
        }
    }
}
