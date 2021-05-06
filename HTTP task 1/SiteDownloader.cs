using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CsQuery;

namespace HTTP_task_1
{
    class SiteDownloader
    {
        public uint MaxDepthLevel { get; set; }
        private string RootDirectory { get; set; }

        public SiteDownloader(uint maxDepthLevel = 1, string rootDitectory = "D:/temp")
        {
            MaxDepthLevel = maxDepthLevel;
            RootDirectory = rootDitectory;
            if (!Directory.Exists(RootDirectory))
            {
                Directory.CreateDirectory(RootDirectory);
            }
        }

        public void DownloadSite(uint level, string url)
        {
            if (level>MaxDepthLevel)
            {
                return;
            }

            using (var client = new HttpClient())
            {
                var tempUri = new Uri(url);
                var hostname = tempUri.Host;
                var currentDirectory =RootDirectory + "/" + hostname;

                if (!Directory.Exists(currentDirectory))
                {
                    Directory.CreateDirectory(currentDirectory);
                    var content = client.GetStringAsync(url).Result;

                    var html = CQ.Create(content);
                    var linkList = new List<string>();
                    var sourceList = new List<string>();

                    foreach (var link in html.Find("a"))
                    {
                        linkList.Add(link.GetAttribute("href"));
                    }

                    foreach (var img in html.Find("img"))
                    {
                        sourceList.Add(img.GetAttribute("src"));
                    }

                    foreach (var style in html.Find("link"))
                    {
                        if (style.GetAttribute("href").EndsWith(".css"))
                        {
                            sourceList.Add(style.GetAttribute("href"));
                        }
                    }

                    foreach (var script in html.Find("script"))
                    {
                        sourceList.Add(script.GetAttribute("src"));
                    }

                    foreach (var source in sourceList)
                    {
                        if (source != null)
                        {
                            var path = source;

                            if (source.StartsWith("/"))
                            {
                                path = url + source;
                            }

                            try
                            {
                                using (var response = client.GetAsync(path).Result)
                                {
                                    if (response.StatusCode != HttpStatusCode.OK)
                                    {
                                        return;
                                    }

                                    using (var streamToReadFrom = response.Content.ReadAsStreamAsync().Result)
                                    {
                                        var filePath = currentDirectory + "/" + path.Split('/').Last();

                                        using (var streamToWriteTo = File.Open(filePath, FileMode.Create))
                                        {
                                            streamToReadFrom.CopyTo(streamToWriteTo);
                                        }

                                        response.Content = null;

                                        content = content.Replace(source, source.Split('/').Last());
                                    }
                                }
                            }
                            catch
                            {

                            }
                        }
                    }

                    var htmlPath = currentDirectory + "/site.html";

                    using (var writer = new StreamWriter(htmlPath, true))
                    {
                        writer.Write(content);
                    }
                }
            }
        }

    }
}
