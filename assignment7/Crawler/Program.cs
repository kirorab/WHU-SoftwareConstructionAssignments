﻿using System.Net;

namespace Crawler
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.IO;
    using System.Text;
    using System.Collections;
    using System.Text.RegularExpressions;
    using System.Threading;

    public class Crawler
    {
        private const int MaxCount = 100;  
        private Hashtable urls = new Hashtable();
        private int count = 0;
        string webRef = @"^https?://.*\.(?:htm|html|aspx|php|jsp)$";
        string relativeRef = @"^/.*$";
        private string limit = @"https?://www.cnblogs.com/dstang2000/.*";
        
        static void Main(string[] args)
        {

            Crawler myCrawler = new Crawler();

            string startUrl = "http://www.cnblogs.com/dstang2000/";
            if (args.Length >= 1) startUrl = args[0];
            myCrawler.urls.Add(startUrl, false);
            new Thread(myCrawler.Crawl).Start();
            //加入初始页面
            //开始爬行
        }

        private void Crawl()
        {
            Console.WriteLine(" 开始爬行了");
            while (true)
            {

                string current = null;
                foreach (string url in urls.Keys)
                {
                    if ((bool)urls[url])
                    {
                        continue;
                    }

                    if (!Regex.IsMatch(url, limit))
                    {
                        continue;
                    }
                    current = url;
                }
                
                if (current == null || (count > MaxCount)) break;
                //找到一个还没有下栽过的链接
                //已经下栽过的， 不再下载
                Console.WriteLine("爬行于" + current + " 页面！");
                string html = DownLoad(current); //下栽
                urls[current] = true;
                count++;
                if (Regex.IsMatch(current, webRef) || count == 1)
                {
                    Parse(html);
                }
            }

            Console.WriteLine("爬行结束");
        }

        public string DownLoad(string url)
        {
            try
            {
                WebClient webClient = new WebClient();
                webClient.Encoding = Encoding.UTF8;
                string html = webClient.DownloadString(url);
                String fileNarne = count.ToString();
                File.WriteAllText(fileNarne, html, Encoding.UTF8);
                return html;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return "";
            }

        }

        public void Parse(string html)
        {
            {
                String strRef = @"(href|HREF)[ ]*=[ ]*[""']([^""'#>]+)[""']";
                MatchCollection matches = new Regex(strRef).Matches(html);

                foreach (Match match in matches)
                {
                    strRef = match.Value.Substring(match.Value.IndexOf('=') + 1).Trim
                        ('"', '\'', '#', ' ', '>', '"');
                    if (Regex.IsMatch(strRef, relativeRef))
                                    {
                                        strRef = "http://www.cnblogs.com" + strRef;
                                    }
                    if (strRef.Length == 0) continue;
                    if (urls[strRef] == null) urls[strRef] = false;
                }
            }
        }
    }
}

