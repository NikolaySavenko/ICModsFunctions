using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace ICModsFunctions
{
    class Mineprogramming
    {
        public static readonly string API_URL = "https://icmods.mineprogramming.org/api/";

        public static string GetDescription(int id) => MakeRequest($"description?id={id}");

        public static int GetDownloads(int id) {
            dynamic description = JsonConvert.DeserializeObject(GetDescription(id));
            return (int) description.downloads;
        }

        public static string GetDownloads(string id) => (string) GetDownloads((string) id);

        private static string MakeRequest(string apiTarget) {
            var requestURI = API_URL + apiTarget;
            var request = WebRequest.Create(requestURI);
            request.Credentials = CredentialCache.DefaultCredentials;

            using WebResponse response = request.GetResponse();
            using Stream dataStream = response.GetResponseStream();
            using StreamReader reader = new StreamReader(dataStream);

            return reader.ReadToEnd();
        }
    }
}
