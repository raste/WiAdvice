//#define WORKING

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UserInterface
{
    /// <summary>
    /// Summary description for $codebehindclassname$
    /// </summary>
    public class GetImage : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
#if WORKING
            if (context != null)
            {
                HttpRequest request = context.Request;

                if (request != null)
                {
                    // Query string is expected to be like
                    // type=site|company|product|advertisement&id=1
                }
            }
#else
            context.Response.ContentType = "text/plain";
            context.Response.Write("Hello World");
#endif
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}
