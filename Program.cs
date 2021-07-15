using System;
using System.Net;
using System.IO;
using System.Collections.Specialized;
using System.Configuration;
using System.Threading;
using Tweetinvi;
using Tweetinvi.Models;
using Tweetinvi.Models.Entities;

namespace get_yeji
{
    class Program
    {
        static string client_key = ConfigurationManager.AppSettings.Get("client_key");
        static string client_key_secret = ConfigurationManager.AppSettings.Get("client_key_secret");
        static string bearer_key = ConfigurationManager.AppSettings.Get("bearer_key");
        static string downloadPath = ConfigurationManager.AppSettings.Get("downloadPath");
        static int interval = int.Parse(ConfigurationManager.AppSettings.Get("interval"));
        static string scope = ConfigurationManager.AppSettings.Get("scope");

        static TwitterClient client = new TwitterClient(client_key, client_key_secret, bearer_key);
        static void Main(string[] args)
        {
            string arguments = string.Concat(args);
            if (config.check() == false || arguments.Contains("config"))
            {
                config.setup();
            }

            while (true)
            {
                string[] targets = scope.Split(',');

                foreach (string target in targets)
                {
                    string[] info = target.Split('|');
                    Get(info[0], info[1], info[2]);
                }
                Console.WriteLine("Cycle complete, next cycle in {0} miliseconds", interval);
                Thread.Sleep(interval);
            }
        }
        static void Get(string mode, string target, string targetDir)
        {
            switch (mode)
            {
                case "st":
                var task = client.Search.SearchTweetsAsync(target);
                task.Wait();
                var tweets = task.Result;
                
                Console.WriteLine("found tweets for {0}", target);
                foreach (var tweet in tweets)
                {
                    foreach (IMediaEntity media in tweet.Entities.Medias)
                    {
                        switch (media.MediaType)
                        {
                            case "photo":
                            Console.Write("downloading {0}\t", media.MediaURL);
                            try
                            {
                                download.image(media.MediaURL, dirPath.procedure(downloadPath,targetDir));
                                Console.WriteLine("Success");
                            }
                            catch (Exception)
                            {
                                Console.WriteLine("Fail");
                                throw;
                            }
                            break;

                            case "video":

                            string dirtyUrl = tweet.Entities.Medias[0].VideoDetails.Variants[0].URL;
                            string vUrl = dirtyUrl.Remove(dirtyUrl.LastIndexOf("?"));

                            Console.Write("downloading {0}\t", vUrl);

                            try
                            {
                                download.video(vUrl, dirPath.procedure(downloadPath,targetDir));
                                Console.WriteLine("Success");
                            }
                            catch (Exception)
                            {
                                Console.WriteLine("Fail");
                                throw;
                            }
                            break;
                        }
                    }
                }
                break;
                case "p":
                var getTweets = client.Timelines.GetUserTimelineAsync(target);
                getTweets.Wait();
                var profileTweets = getTweets.Result;
                
                Console.WriteLine("found tweets for {0}", target);
                foreach (var tweet in profileTweets)
                {
                    foreach (IMediaEntity media in tweet.Entities.Medias)
                    {
                        switch (media.MediaType)
                        {
                            case "photo":
                            Console.Write("downloading {0}\t", media.MediaURL);

                            try
                            {
                                download.image(media.MediaURL, dirPath.procedure(downloadPath,targetDir));
                                Console.WriteLine("Success");
                            }
                            catch (Exception)
                            {
                                Console.WriteLine("Fail");
                                throw;
                            }
                            break;

                            case "video":

                            string dirtyUrl = tweet.Entities.Medias[0].VideoDetails.Variants[0].URL;
                            string vUrl = dirtyUrl.Remove(dirtyUrl.LastIndexOf("?"));

                            Console.Write("downloading {0}\t", vUrl);

                            try
                            {
                                download.video(vUrl, dirPath.procedure(downloadPath,targetDir));
                                Console.WriteLine("Success");
                            }
                            catch (Exception)
                            {
                                Console.WriteLine("Fail");
                                throw;
                            }
                            break;
                        }
                    }
                }
                break;
            }
        }
        public static class download
        {
            public static void image(string url, string path)
            {
                string name = url.Trim();
                name = name.Remove(0, name.LastIndexOf("/") + 1);

                string file = path + name;

                using (WebClient wclient = new WebClient())
                {
                    wclient.DownloadFile(new Uri(url), file);
                }
            }
            public static void video(string url, string path)
            {
                string name = url.Trim();
                name = name.Remove(0, name.LastIndexOf("/") + 1);

                string file = path + name;

                using (WebClient wclient = new WebClient())
                {
                    wclient.DownloadFile(new Uri(url), file);
                }
            }
        }
        public static class config
        {
            public static bool check()
            {
                if(ConfigurationManager.AppSettings.Get("client_key") == "" || ConfigurationManager.AppSettings.Get("client_key_secret") == "" || ConfigurationManager.AppSettings.Get("bearer_key") == "" || ConfigurationManager.AppSettings.Get("downloadPath") == "" || ConfigurationManager.AppSettings.Get("interval") == "" || ConfigurationManager.AppSettings.Get("scope") == "")
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            public static void setup()
            {
                string[] strings = {"client_key","client_key_secret","bearer_key","downloadPath","interval","scope"};
                Console.WriteLine("config service mode, please enter data for the new config");
                foreach(string type in strings)
                {
                    Console.WriteLine("Old {0}: {1}",type, ConfigurationManager.AppSettings.Get(type));
                    Console.Write("New {0}: ",type);
                    
                    string input = Console.ReadLine();
                    if(input == "")
                    {
                        input = ConfigurationManager.AppSettings.Get(type);
                        Console.WriteLine("kept old values (no new entered)");
                    }
                    config.set(type,input);
                }
                Console.WriteLine("new config generated");
                Environment.Exit(0);
            }
            public static void set(string key, string value)
            {
                try
                {
                    var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                    var settings = configFile.AppSettings.Settings;
                    if (settings[key] == null)
                    {
                        settings.Add(key, value);
                    }
                    else
                    {
                        settings[key].Value = value;
                    }
                    configFile.Save(ConfigurationSaveMode.Modified);
                    ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
                }
                catch (ConfigurationErrorsException)
                {
                    Console.WriteLine("Error writing app settings");
                }
            }
        }
        public static class dirPath
        {
            public static string procedure(string masterPath, string subDir)
            {
                masterPath = masterPath.Trim();
                subDir = subDir.Trim();

                if(!masterPath.EndsWith('\\'))
                {
                    masterPath = masterPath + @"\";
                }
                if (subDir.StartsWith('\\'))
                {
                    subDir = subDir.Remove(0,subDir.IndexOf('\\')+1);
                }
                if(!subDir.EndsWith('\\'))
                {
                    subDir = subDir + "\\";
                }

                return masterPath+subDir;
            }
        }
    }
}