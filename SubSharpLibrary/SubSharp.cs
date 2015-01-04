using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.Web.Http;
using SubSharpLibrary.Exceptions;

namespace SubSharpLibrary.Client
{
    /// <summary>
    /// A class for handling request to Subsonic API
    /// </summary>
    public class SubSharp
    {

        string url, action, parameters;

        
        HttpClient httpClient;
        private string[] ag;


       
        /**
             * [0] = user
             * [1] = password
             * [2] = ip 
             * [3] = port -- 
             * [4] = version
             * [5] = AppName
             */

        public SubSharp(string[] connect, bool https){

            // TODO verify args


            checkArgs( ref connect);

            httpClient = new HttpClient();


            string url = "http://";
            if (https)
            {
                url = "https://";
            }

            
            

            this.url = url + connect[2] + "/rest/";
            parameters = "?u=" + connect[0] + "&p=enc:" + StringToHex(connect[1]) + "&v=" + connect[4] + "&c=" + connect[5];


            
        }


        /// <summary>
        /// Helper to convert a String to Hex encoding
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string StringToHex(String data)
        {
            var bytes = Encoding.UTF8.GetBytes(data);

            string hex = BitConverter.ToString(bytes);
            return hex.Replace("-", "");
        }



        // FIXME Better Exception Handling
        /// <summary>
        /// Return the Task containg the string result of the http get url
        /// May return unhandled exceptions
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task<String> get_HTML(String url) 
        {
            var uri = new Uri(url);
            

            // Always catch network exceptions for async methods.
            
            try
            {
             
                var result = await httpClient.GetStringAsync(uri);
                return result;
          
            }
                // TODO Catch Network Exceptions
            catch (Exception ex)
            {

                Debug.WriteLine(ex.ToString());
                throw ex;
            }
          
          
                
       }

        /// <summary>
        /// <exception cref="ArgumentException">Ip can only be empty arg</exception>
        /// </summary>
        /// <param name="connect"></param>
        private static void checkArgs( ref string[] connect)
        {
            if (connect[3].Length == 0)
            {
                int skip = 3;

                for (int i = 0; i < connect.Length; i++)
                {
                    if (i != skip)
                    {
                        if (connect[i].Length == 0)
                        {
                            throw new ArgumentException("Failed to Initiliaze SubSharp \nAn argument field is empty");
                        }
                    }
                }
            }
        }


        public SubResponse ping()
        {

            action = "ping.view";
            return gen_SubResponse(action, null);
        }

        private SubResponse gen_SubResponse(string action, string[] parameters)
        {
            String saveParam = null;

            if (parameters != null)
            {
                if (parameters.Count() > 1 && parameters.Count() % 2 == 0)
                {
                    saveParam = this.parameters;

                    for (int i = 0; i < parameters.Count() - 1; i += 2)
                    {
                        this.parameters += "&" + parameters[i] + "=" + parameters[i + 1];
                    }

                    
                }
                else
                {
                    // throw SubSharp Exception
                    throw new SubSharpException("Parameters are not even or greater than one", SubSharpException.ErrorCode.GENERIC_SUBSHARP_EXCEPTION);
                }
               
            }


            String http = this.url + action + this.parameters;

            Debug.WriteLine("URL = " + http);
            var get = get_HTML(http).Result;

            // dispose of actions and paremeters
            action = "";
            if (!String.IsNullOrEmpty(saveParam))
            {
                this.parameters = saveParam;
            }
            return new SubResponse(get);

        }

        /// <summary>
        /// Helper to add params to new string of Parameters
        /// </summary>
        /// <param name="parameters"></param>
        private String get_added_Parameters(string[] parameters){

            return null;
        }

        /// <summary>
        /// A dictionary of album elements 
        /// Key = KeyType of attribute.value in album element tag
        /// Value =  Dict< Attribute.Name , Attribute.Value >
        /// </summary>
        /// <param name="keyType"></param>
        /// <param name="id"></param>
        /// <returns> a dictionary of nested albums keyed by KeyType</returns>
        public Dictionary<string, Dictionary<string, string>> getArtist(String keyType, String id)
        {

            action = "getArtist.view";
            
            string[] parameters = { "id", id };
            return  gen_SubResponse(action, parameters).Result_To_Attribute_Dict("album", keyType);
        }

        /// <summary>
        /// A dictionary of artist elements
        /// With key = Atrribute.Value of keyType in artist element tag
        /// </summary>
        /// <returns>a SubResponse containing a Dictionary response</returns>
        public Dictionary<String, Dictionary<String, String>> getArtists(String keyType)
        {
            action = "getArtists.view";
            
            return gen_SubResponse(action, null).Result_To_Attribute_Dict("artist", keyType);
            
            
        }

        /// <summary>
        /// Helper to generate a url with no parameters
        /// </summary>
        /// <returns>a string url</returns>
        private string gen_Url(string action)
        {
            
            throw new NotImplementedException();
        }

        public static string ToHexString(string str)
        {
            var sb = new StringBuilder();

            var bytes = Encoding.Unicode.GetBytes(str);
            foreach (var t in bytes)
            {
                sb.Append(t.ToString("X2"));
            }

            return sb.ToString(); // returns: "48656C6C6F20776F726C64" for "Hello world"
        }

        public void print(Dictionary<String, Dictionary<String, String>> dict)
        {
            foreach (String name in dict.Keys)
            {
                Debug.WriteLine("Key - " + name);

                Dictionary<String, String> args;

                if (dict.TryGetValue(name, out args))
                {
                    foreach (KeyValuePair<string, string> kvp in args)
                    {
                        Debug.WriteLine(" -- Value " + kvp.Key.ToString() + " -- " + kvp.Value.ToString());
                    }

                }


            }
        }
    }

}
