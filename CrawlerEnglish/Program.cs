using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;


namespace CrawlerEnglish
{
    class Program
    {
        private const string ApiUrl = "http://httpvod.ct-edu.com.cn:8080/index.php?app=api&mod=Public&act=GetVideo&id={0}";
        private const string SavePath = "e:\\{0}.mp4";
        private static object lockobj = new object();
        private static int threadCounter = 0;

        private static void DownFile(int index)
        {
            var url = string.Format(ApiUrl, index);
            var request = (HttpWebRequest)WebRequest.CreateHttp(url);
            request.Method = "GET";
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            using (var response = request.GetResponseAsync().Result)
            using (var stream = response.GetResponseStream())
            using (var reader = new StreamReader(stream))
            {
                var content = reader.ReadToEnd();
                var videoResponse = JsonConvert.DeserializeObject<VideoResponse>(content);
                if (videoResponse != null)
                {
                    Console.WriteLine(videoResponse.Status);
                    var video = JsonConvert.DeserializeObject<Video>(videoResponse.Content);
                    if (video != null && !String.IsNullOrEmpty(video.Url))
                    {

                        Console.WriteLine("title:{0}, url:{1}", video.Title, video.Url);
                        if (video.Title.Contains("英"))
                        {
                            while (true)
                            {
                                if (threadCounter <= 10)
                                {
                                    ThreadPool.QueueUserWorkItem((o) => DownFile(video.Url, video.Title));
                                    lock (lockobj)
                                    {
                                        threadCounter++;
                                    }
                                    break;
                                }
                                else
                                {
                                    Thread.Sleep(30 * 1000);
                                    Console.WriteLine($"current thread counter is {threadCounter}");
                                }
                            }

                        }
                    }
                }
            }
        }

        private static void DownFile(string url, string name)
        {
            try
            {
                var request = HttpWebRequest.Create(url);
                var response = request.GetResponseAsync().Result;
                var filepath = string.Format(SavePath, name);
                var filestream = new FileStream(filepath, FileMode.Create);
                var httpStream = response.GetResponseStream();
                byte[] bArr = new byte[1024];
                int size = httpStream.Read(bArr, 0, (int)bArr.Length);
                while (size > 0)
                {
                    filestream.Write(bArr, 0, size);
                    size = httpStream.Read(bArr, 0, (int)bArr.Length);
                }

                filestream.Dispose();
                httpStream.Dispose();

                Console.WriteLine($"download this file is ok, filename:{filepath}");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"this {name} file is error, info:{ex.ToString()}");
            }
            finally
            {
                lock (lockobj)
                {
                    threadCounter--;
                }
            }

        }

        static void Main(string[] args)
        {
            if (args.Length != 2)
                Console.WriteLine("write start number and end index");

            var start = Convert.ToInt32(args[0]);
            var end = Convert.ToInt32(args[1]);

            //var start = 201170;
            //var end = 10;

            for (int i = 0; i < end; i++)
            {
                DownFile(start + i);
            }

            while (true)
            {
                if (threadCounter <= 0)
                {
                    Console.WriteLine("not have down task");
                    break;
                }
                else
                {
                    Console.WriteLine($"now has {threadCounter} tasks");
                    Thread.Sleep(10 * 1000);
                }
            }

            Console.Read();
        }
    }
}