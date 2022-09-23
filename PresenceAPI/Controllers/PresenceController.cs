using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http;
using System;
using System.IO;

namespace PresenceAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PresenceController : ControllerBase
    {
        [HttpGet(Name = "GetPresence")]

        public List<KeyValuePair> Get()
        {
            List<KeyValuePair> result = new List<KeyValuePair>();
            string filepath = "C:\\Projects\\PresenceAPI\\PresenceAPI\\DataStore.txt";
            using (StreamReader sr = new StreamReader(filepath))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    string[] parts = line.Split(':');
                    if (parts.Length == 2)
                    {
                        KeyValuePair keyValuePair = new KeyValuePair
                        {
                            Key = parts[0],
                            Value = parts[1]
                        };
                        result.Add(keyValuePair);
                    }
                }
            }
            return result;
        }

        [HttpGet("{emailId}")]
        public string Get(string emailId)
        {
            string ans = string.Empty;
            string ap = "'";
            string filepath = "C:\\Projects\\PresenceAPI\\PresenceAPI\\DataStore.txt";
            using (StreamReader sr = new StreamReader(filepath))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    string[] parts = line.Split(':');
                    if (parts.Length == 2 && parts[0] == ap + emailId + ap)
                    {
                        ans = parts[1];
                    }
                }
            }
            return (string.IsNullOrEmpty(ans)) ? $"{emailId} has not updated presence." : ans;
        }

        [HttpPost("{obj}")]
        public string Post(KeyValuePair obj)
        {
            string ap = "'";
            string emailID = ap + obj.Key + ap;
            string duplicate = string.Empty;
            KeyValuePair prevData = new KeyValuePair();
            bool isPresent = false;
            try
            {
                string filepath = "C:\\Projects\\PresenceAPI\\PresenceAPI\\DataStore.txt";
                string toAdd = ap + obj.Key + ap + ":" + ap + obj.Value + ap;

                using (StreamReader sr = new StreamReader(filepath))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        string[] parts = line.Split(':');
                        if (parts.Length == 2 && parts[0] == emailID)
                        {
                            duplicate = line;
                            isPresent = true;
                            prevData.Key = parts[0];
                            prevData.Value = parts[1];
                        }
                    }
                }
                if (isPresent)
                {
                    string text = System.IO.File.ReadAllText(filepath);
                    text = text.Replace(duplicate, toAdd);
                    System.IO.File.WriteAllText(filepath, text);
                    return prevData.Key + ", your presence has been updated from " + prevData.Value + " to " + obj.Value + ".";
                }
                System.IO.File.AppendAllText(filepath, toAdd + Environment.NewLine);

                return duplicate + ": Added " + toAdd + " successfully";
            }
            catch (Exception)
            {
                return "You messed up!";
            }
        }
    }
}