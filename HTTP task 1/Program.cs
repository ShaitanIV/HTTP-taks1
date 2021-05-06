using CsQuery;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace HTTP_task_1
{
    class Program
    {
        static void Main(string[] args)
        {
            var siteDownloader = new SiteDownloader();
            siteDownloader.DownloadSite(1, "https://playeternalreturn.com/");

        }
    }
}
