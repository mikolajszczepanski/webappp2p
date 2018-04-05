using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace WebAppP2P
{
    public static class UrlUtils
    {
        public static string UrlDecodeWithBase64(string url)
        {
            var decoded = WebUtility.UrlDecode(url);
            return decoded.Replace(' ', '+');
        }
    }
}
