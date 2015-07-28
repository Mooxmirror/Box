using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Emit;
using System.Text;

namespace WebServer
{
    public class Router
    {
        private static Router _DefaultInstance;

        private Dictionary<RequestMethod, List<KeyValuePair<string, Route>>> HandledRoutes;
        public delegate bool Route(Request req, Response res);

        /// <summary>
        /// Initializes a new empty router.
        /// </summary>
        public Router()
        {
            HandledRoutes = new Dictionary<RequestMethod, List<KeyValuePair<string, Route>>>()
            {
                { RequestMethod.GET, new List<KeyValuePair<string, Route>>() }
            };
        }

        /// <summary>
        /// Adds a handle to the router.
        /// </summary>
        /// <param name="method"></param>
        /// <param name="pattern"></param>
        /// <param name="del"></param>
        public void AddHandle(RequestMethod method, string pattern, Route del)
        {
            HandledRoutes[method].Add(new KeyValuePair<string, Route>(pattern, del));
        }

        /// <summary>
        /// Checks if the path matches the pattern.
        /// </summary>
        /// <param name="pattern"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        private bool Matches(string pattern, string path)
        {
            // TODO: make this better
            return true;
        }
        
        /// <summary>
        /// Handles the given request and routes it to the fitting handler.
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public Response Handle(Request req)
        {
            Response res = new Response();
            // Checks if the method is implemented (routed)
            if (!HandledRoutes.ContainsKey(req.Method))
            {
                res.Header.StatusCode = "501 Not Implemented";
                res.Header.ContentType = "text/plain; encoding=utf-8";
                res.Write("501 Not Implemented\n");
                return res;
            }

            List<KeyValuePair<string, Route>> possibleRoutes = HandledRoutes[req.Method];
            foreach (KeyValuePair<string, Route> r in possibleRoutes)
            {
                if (Matches(r.Key, req.Path))
                {
                    try
                    {
                        if (r.Value(req, res))
                            return res;
                    }
                    catch (Exception e)
                    {
                        res.Header.StatusCode = "500 Internal Server Error";
                        res.Header.ContentType = "text/plain; encoding=utf-8";
                        res.ClearContent();
                        res.Write("500 Internal Server Error\n");
                        res.Write($"Route {r.Key} threw error: {e.Message}\n");
                        return res;
                    }
                }
            }
            return res;
        }

        /// <summary>
        /// Represents a default router.
        /// </summary>
        public static Router Default
        {
            get
            {
                if (_DefaultInstance == null)
                {
                    _DefaultInstance = new Router();
                    Route defaultRoute = new Route((req, res) =>
                    {
                        res.Header.ContentType = "text/html";
                        res.Write("<html><body><h1>it works!</h1><p>BOX/0.1</p></body></html>");
                        return true;
                    });
                    _DefaultInstance.AddHandle(RequestMethod.GET, "/", defaultRoute);
                }
                return _DefaultInstance;
            }
        }
    }
}
