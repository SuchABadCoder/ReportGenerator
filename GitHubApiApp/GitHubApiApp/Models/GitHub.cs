using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace GitHubApiApp.Models
{
    class LinkFields
    {
        public string self;
    }

    class FileInfo
    {
        public string name;
        public string type;
        public string download_url;
        public LinkFields _links;
        public string extention;
    }

    public class FileData
    {
        public string name;
        public string contents;
        public string url;

        public override string ToString()
        {
            return name;
        }
    }
    public class Directory
    {
        public string name;
        public List<Directory> subDirs;
        public List<FileData> files;


        public override string ToString()
        {
            return name;
        }
    }

    //Github classes
    public class GitHub
    {
        //Get all files from a repo
        public static async Task<Directory> GetRepo(string owner, string name, string access_token)
        {
            Directory root = new Directory();
            using(HttpClient client=new HttpClient())
            {
                root = await readDirectory("root", client, String.Format("https://api.github.com/repos/{0}/{1}/contents/", owner, name), access_token);
            }
            return root;
        }

        public static async Task<Directory> GetRepo2(string link, string access_token)
        {
            Directory root = new Directory();

            string repos = "";
            string username = "";
            string str = link;
            int index = str.Length - 1;
            while (true)
            {
                repos += str[index];
                if (str[index - 1] == '/')
                    break;
                index--;
            }

            repos = new string(repos.Reverse().ToArray());

            str = str.Remove(str.Length - repos.Length - 1);
            index = str.Length - 1;
            while (true)
            {
                username += str[index];
                if (str[index - 1] == '/')
                    break;
                index--;
            }

            username = new string(username.Reverse().ToArray());

            using (HttpClient client = new HttpClient())
            {
                root = await readDirectory("root", client, String.Format("https://api.github.com/repos/{0}/{1}/contents/", username, repos), access_token);
            }
            return root;

        }

        //recursively get the contents of all files and subdirectories within a directory 
        private static async Task<Directory> readDirectory(string name, HttpClient client, string uri, string access_token)
        {
            //get the directory contents
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, uri);
            request.Headers.Add("Authorization",
                "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(String.Format("{0}:{1}", access_token, "x-oauth-basic"))));
            request.Headers.Add("User-Agent", "lk-github-client");

            //parse result
            HttpResponseMessage response = await client.SendAsync(request);
            String jsonStr = await response.Content.ReadAsStringAsync(); ;
            response.Dispose();
            //FileInfo[] dirContents = JsonConvert.DeserializeObject<FileInfo[]>(jsonStr);
            List<FileInfo> dirContents = JsonConvert.DeserializeObject<List<FileInfo>>(jsonStr);
            //read in data
            Directory result=new Directory();
            result.name = name;
            result.subDirs = new List<Directory>();
            result.files = new List<FileData>();
            foreach (FileInfo file in dirContents)
            {
                if (file.type == "dir")
                { //read in the subdirectory
                    Directory sub = await readDirectory(file.name, client, file._links.self, access_token);
                    result.subDirs.Add(sub);
                }
                else
                { //get the file contents;
                    Console.WriteLine(file.extention);
                    HttpRequestMessage downLoadUrl = new HttpRequestMessage(HttpMethod.Get, file.download_url);

                    downLoadUrl.Headers.Add("Authorization",
                        "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(String.Format("{0}:{1}", access_token, "x-oauth-basic"))));
                    request.Headers.Add("User-Agent", "lk-github-client");

                    HttpResponseMessage contentResponse = await client.SendAsync(downLoadUrl);
                    string content = await contentResponse.Content.ReadAsStringAsync();
                    contentResponse.Dispose();

                    FileData data = new FileData() 
                    { 
                        name = file.name,
                        contents = content,
                        url = dirContents.Where(t => t.name.Contains(file.name)).Select(t => t.download_url).FirstOrDefault()
                    };
                    result.files.Add(data);
                }
            }
            return result;
        }
    }
}
