using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Net;
using System.Net.Http;

namespace URLSelect
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            try
            {
                List<string> backupData = new List<string>();
                Console.WriteLine("URL検索を行いたいファイルのパスを指定して下さい");

                //ファイルパスの入力待ち
                string filepath = Console.ReadLine().Trim();

                //ファイルの中身を一行ずつ読み込んでいく
                string line;
                using(StreamReader sr = new StreamReader(filepath,Encoding.Default))
                {
                    while((line = sr.ReadLine()) != null)
                    {
                        backupData.Add(line);
                    }
                }

                Console.WriteLine("URLの抽出開始");
                List<string> rireki = new List<string>();
                List<string> badlink = new List<string>();
                rireki.Add("");

                foreach(string linedata in backupData)
                {
                    if(linedata.Contains(Defs.TITLE))
                    {
						Console.WriteLine("=============");
						badlink.Add("=============");
                        Console.WriteLine(linedata);
                        badlink.Add(linedata);
                    }

                    if(linedata.Contains(Defs.HTTP))
                    {
                        string[] split = linedata.Split('"');

                        for (int i = 0; i < split.Length; i++)
                        {
                            string url = split[i].Replace(Defs.IMAGE, "").Trim();
                            bool flg = true;

                            foreach (string tmp in rireki)
							{
                                if (tmp.Equals(url))
                                {
                                    flg = false;
                                    break;
                                }
							}

                            if (((url.StartsWith("http://", StringComparison.OrdinalIgnoreCase)) || (url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))) && flg)
                            {
                                try
                                {
                                    using (var client = new HttpClient())
                                    {
                                        var response = client.GetAsync(url).Result;

                                        if(response.StatusCode.ToString().Equals("OK"))
                                        {
                                            rireki.Add(url);
                                        }
                                        else
										{
                                            Console.WriteLine(url);
											Console.WriteLine(response.StatusCode);
                                            badlink.Add(url);
                                            badlink.Add(response.StatusCode.ToString());
                                        }
                                    }
                                    
                                }
                                catch(Exception ex)
                                {
                                    Console.WriteLine(url);
                                    Console.WriteLine(ex.Message);
                                    badlink.Add(url);
                                    badlink.Add(ex.Message);
                                }
                            }
                        }
                    }
                }

                string GetPath = Directory.GetCurrentDirectory() + "/" + Defs.OUTPUT;
				string writepath = GetPath + "/" + DateTime.Now.Year.ToString() +
														DateTime.Now.Day.ToString() +
														DateTime.Now.Hour.ToString() +
														DateTime.Now.Minute.ToString() +
														DateTime.Now.Second.ToString() + ".txt";
                using (StreamWriter sw = File.CreateText(writepath))
                {
                    foreach (string oline in badlink)
                    {
                        sw.WriteLine(oline);
                    }
                }
                

                Console.WriteLine("終了");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
