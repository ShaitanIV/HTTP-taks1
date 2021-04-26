using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Linq;
using System.Net.Http;
using HtmlAgilityPack;
using System.IO;

namespace HTTP_Task1
{
    class SiteDownloader
    {
        private List<string> Restrictions;
        private List<string> VisitedResourses;
        public uint LevelOfSearch { get; set; }
        public string RootDirectory { get; set; }
        public bool StayInSameDomain { get; set; }
        public bool NotHigherThanStartingPoint { get; set; }
        private int counter { get; set; }

        public SiteDownloader(uint level = 0, string rootDirectory = @"C:\test", bool stayInSameDomain = false, bool notHigherThanStartingPoint=false)
        {
            Restrictions = new List<string>();
            VisitedResourses = new List<string>();
            LevelOfSearch = level;
            RootDirectory = rootDirectory;
            StayInSameDomain = stayInSameDomain;
            NotHigherThanStartingPoint = notHigherThanStartingPoint;
            counter = 0;
        }

        public void Download(string startingPoint)
        {
            VisitedResourses.Clear();

            using (var webClient = new WebClient())
            {
                //Get host
                var uri = new Uri(startingPoint);
                var host = uri.Host;
                AnalyzeUri(uri.ToString(), uri.ToString(), 0);
            }
        }

        private void AnalyzeUri(string startingPoint, string uri, uint level)
        {
            if (uri == null)
            {
                throw new NullReferenceException();
            }

            uri = RemoveParameters(uri);

            if (LevelOfSearch < level || VisitedResourses.Contains(uri))
            {
                return;
            }

            VisitedResourses.Add(uri);
            level++;

            if (IsAcceptable(uri))
            {
                using (var webclient = new WebClient())
                {
                    var resourse = webclient.DownloadString(uri);
                    webclient.DownloadFile(uri, Path.Combine(RootDirectory , $"{counter++}.file"));
                    var html = new HtmlAgilityPack.HtmlDocument();
                    html.LoadHtml(resourse);

                    foreach (var linkAsNode in html.DocumentNode.SelectNodes("//a[@href]"))
                    {
                        var link = linkAsNode.Attributes["href"].Value;

                        if (NotHigherThanStartingPoint)
                        {
                            if (RemoveParameters(link.ToString()).StartsWith(startingPoint) && IsAcceptable(link))
                            {
                                try
                                {
                                    Console.WriteLine($"Downloading file {link}");
                                    webclient.DownloadFile(uri, Path.Combine(RootDirectory, "${counter++}", ".file"));
                                    var newLevel = level++;
                                    AnalyzeUri(startingPoint, link, level);
                                }
                                catch
                                {
                                    Console.WriteLine($"Unable to download file {link}");
                                }
                            }
                            else
                            {
                                return;
                            }
                        }
                        else if (StayInSameDomain)
                        {
                            var startingUri = new Uri(startingPoint);
                            var currentUri = new Uri(link);

                            if (currentUri.Host == startingUri.Host && IsAcceptable(link))
                            {
                                try
                                {
                                    Console.WriteLine($"Downloading file {link}");
                                    webclient.DownloadFile(uri, Path.Combine(RootDirectory, "${counter++}", ".file"));
                                    var newLevel = level++;
                                    AnalyzeUri(startingPoint, link, level);
                                }
                                catch
                                {
                                    Console.WriteLine($"Unable to download file {link}");
                                }
                            }
                            else
                            {
                                return;
                            }
                        }
                        else
                        {
                            if (IsAcceptable(link))
                            {
                                try
                                {
                                    Console.WriteLine($"Downloading file {link}");
                                    webclient.DownloadFile(link.ToString(), Path.Combine(RootDirectory, uri));
                                    var newLevel = level++;
                                    AnalyzeUri(startingPoint, link, level);
                                }
                                catch
                                {
                                    Console.WriteLine($"Unable to download file {link}");
                                }
                            }
                        }
                    }
                }
            }
        }

        public void AddRestriction(string restriction)
        {
            if (restriction == null)
            {
                throw new NullReferenceException();
            }

            Restrictions.Add(restriction);
        }

        public void RemoveRestriction(string restriction)
        {
            if (restriction == null)
            {
                throw new NullReferenceException();
            }

            var indexOfRestriction = Restrictions.IndexOf(restriction);

            if (indexOfRestriction == -1)
            {
                Console.WriteLine("There is no such restriction in list");
            }
            else
            {
                Restrictions.RemoveAt(indexOfRestriction);
                Console.WriteLine($"Restriction {restriction} successfully removed");
            }
        }

        private string RemoveParameters(string urlWithParameters)
        {
            return urlWithParameters.Split('?').First();
        }

        private bool IsAcceptable(string resourseUrl)
        {
            return Restrictions.Contains(RemoveParameters(resourseUrl).Split('.').Last()) ? false : true;
        }
    }
}
