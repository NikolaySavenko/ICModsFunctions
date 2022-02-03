using ApplicationCore.Model.InnerCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace ApplicationCore
{
    public class Mineprogramming
    {
        public static readonly string API_URL = "https://icmods.mineprogramming.org/api/";

        public static List<InnerCoreModData> GetExistingMods()
        {
            var mineprogrammningModList = MakeRequest("list?start=0&count=10000&horizon");
            return JsonConvert.DeserializeObject<List<InnerCoreModData>>(mineprogrammningModList);
        }

        public static InnerCoreModDescription GetDescription(int id)
        {
            var response = MakeRequest($"description?id={id}");
            return JsonConvert.DeserializeObject<InnerCoreModDescription>(response);
        }

        public static int GetDownloads(int id) => GetDescription(id).Downloads;

        private static string MakeRequest(string apiTarget)
        {
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
