using System;
using System.Net;

namespace HTTP_Task1
{
    class Program
    {
        static void Main(string[] args)
        {
            var downloader = new SiteDownloader();
            downloader.AddRestriction("jpeg");
            downloader.LevelOfSearch = 1;
            downloader.Download("https://www.basicwebsiteexample.com/");
        }
    }
}
