using SubSharpLibrary.Exceptions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.Web.Http;

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


        public enum Album_List_Type
        {
            random,
            newest,
            frequent,
            highest,
            recent,
            starred,
            alphabeticalByName,
            alphabeticalByArtist,
            byYear,
            byGenre
        }

        /**
             * [0] = user
             * [1] = password
             * [2] = ip 
             * [3] = port -- 
             * [4] = version
             * [5] = AppName
             */

        public SubSharp(string[] connect, bool https)
        {

            // TODO verify args


            checkArgs(ref connect);

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




        /// <summary>
        /// <exception cref="ArgumentException">Ip can only be empty arg</exception>
        /// </summary>
        /// <param name="connect"></param>
        private static void checkArgs(ref string[] connect)
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


            Add_Parameters(parameters, out saveParam);


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

        private async Task<String> get_HTML(string http)
        {

            Uri uri = null;
            try
            {
                uri = new Uri(http);
            }
            catch (Exception ex)
            {

                throw new SubSharpException("Imporperly formed URL" + http, ex);
            }


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

        private void Add_Parameters(string[] parameters, out String saveParam)
        {
            

            if (parameters != null)
            {

                int count = parameters.Count();

                if (count == 0)
                {
                    saveParam = "";
                }
                else if (count % 2 == 0)
                {
                    saveParam = this.parameters;

                    for (int i = 0; i < count - 1; i += 2)
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
            else
            {
                saveParam = "";
            }

        }

  

        // TODO Allow Async Download Operation Creation
        /// <summary>
        /// Stream a given media file
        /// Returns a Download Operation for Background Data Transferance
        /// Check should be done to comnfirm stream is writing
        /// 
        /// All collections return with a collection that may be empty
        /// </summary>
        /// <param name="downloadFile"></param>
        /// <param name="id"></param>
        /// <param name="maxBitRate"></param>
        /// <param name="format"></param>
        /// <param name="estimate_Content_Length"></param>
        /// <param name="bg"></param>
        /// <returns>A Download Operation</returns>
        public DownloadOperation test_Stream(String id, int maxBitRate, String format, bool estimate_Content_Length, BackgroundDownloader bg, IStorageFile file)
        {




            string[] paramerters = { "&", id, "maxBitRate", maxBitRate.ToString(), "format", format, "estimateContentLength", estimate_Content_Length.ToString() };

            String stream_Params = "";
            this.Add_Parameters(paramerters, out stream_Params);

            action = "stream.view";
            String http = this.url + action + this.parameters;
            Uri uri = null;

            try
            {
                uri = new Uri(http);
            }
            catch (Exception ex)
            {

                throw new SubSharpException("Imporperly formed URL" + http, ex);
            }
            StorageFolder local = Windows.Storage.ApplicationData.Current.LocalFolder;
            /**
            Uri downloadUri = new Uri("/shared/transfers" + downloadFile, UriKind.RelativeOrAbsolute);
            
            // Create Download Location if doesnt Exist
            

            StorageFolder local = Windows.Storage.ApplicationData.Current.LocalFolder;
            Debug.WriteLine("Local Storage FOlder - " + local.Name);
            // Create a new folder name DataFolder.
            try
            {
                var dataFolder = await local.CreateFolderAsync("/shared/transfers",
                        CreationCollisionOption.OpenIfExists);

                // Create a new file and replace if existing
                try
                {
                    var file = await dataFolder.CreateFileAsync(downloadFile,
                            CreationCollisionOption.ReplaceExisting);
                }
                catch (Exception)
                {
                    Debug.WriteLine("Cant create file");
                    throw;
                }
            }
            catch (Exception)
            {
                Debug.WriteLine("Can't Create folder");
                throw;
            }
            
           

            */
            // Always catch network exceptions for async methods.
            Debug.WriteLine("url - " + uri.ToString());

            try
            {
                /**
                var http_message = await httpClient.SendRequestAsync(new HttpRequestMessage(HttpMethod.Get, uri));
                String headers = "";
                foreach(var head in http_message.Headers.Values){
                    headers += head + "\n";
                    
                }

                Debug.WriteLine("headers\n" + headers);

                
                var text =  http_message.Content.GetType();
                Debug.WriteLine("Content type " + text);
                var response = await http_message.Content.ReadAsStringAsync();
                
                if (response != null)
                {
                    Debug.WriteLine("Response -- " + response);
                   // var resposnive = new SubResponse(response);
                   // Debug.WriteLine("Ok -- " + resposnive.get_isOk);
                }
                else
                {
                    Debug.WriteLine("No response!");
                }
                
                

                
                var stream = await http_message.Content.ReadAsInputStreamAsync();
                
                var in_stream = stream.AsStreamForRead();
                Debug.WriteLine("CAn read - " + in_stream.CanRead);
                Debug.WriteLine("CAn write - " + in_stream.CanWrite);
                Debug.WriteLine("CAn seek - " + in_stream.CanSeek);
                Debug.WriteLine("CAn timeout - " + in_stream.CanTimeout);

                 * */
                return bg.CreateDownload(uri, file);


            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                throw ex;
            }





        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <exception cref="SubSharpException">Will throw exception if reuqesting user is not admin</exception>       
        /// <returns></returns>
        public LinkedList<Dictionary<string, string>> getPlaylists(string username = "")
        {

            LinkedList<string> my_Params = new LinkedList<string>();

            if (!String.IsNullOrEmpty(username))
            {
                my_Params.AddLast("username");
                my_Params.AddLast(username);
            }


            action = "getPlaylists.view";

            return gen_SubResponse(action, my_Params.ToArray()).Result_To_Attribute_Dict("playlist");
           

            
        }

        /// <summary>
        /// Gets songs by genre
        /// </summary>
        /// <param name="genre"></param>
        /// <param name="count">Optional, must be greater than zero</param>
        /// <param name="offset">Optional for offsetting pages</param>
        /// <returns>A linked list of dictionaries for each song element</returns>
        public LinkedList<Dictionary<String, String>> getSongsByGenre( String genre, int count = 10, int offset = 0)
        {

            if (String.IsNullOrEmpty(genre))
            {
                throw new SubSharpException("Genre is required and is empty");
            }

            if (count <= 0)
            {
                throw new SubSharpException("Count Requested is less than or equal to zero");
            }

            
            if (offset <= 0)
            {
                throw new SubSharpException("Offset Requested is less than or equal to zero");
            }

            if (count > 500)
            {
                count = 500;
            }


            LinkedList<string> my_Params = new LinkedList<string>();

            String[] paramas = { "genre", genre, "count", count.ToString(), "offset", offset.ToString() };

            action = "getSongByGenre.view";

            return gen_SubResponse(action, my_Params.ToArray()).Result_To_Attribute_Dict("song");
            
        }


        // TODO Cast hange to (LinkedList<Dictionary<String,String>>)
        /// <summary>
        /// Return a linked list with a dictionary foreach random song element
        /// </summary>
        /// <param name="size"></param>
        /// <param name="genre"> From getGenres </param>
        /// <param name="fromYear"></param>
        /// <param name="toYear"></param>
        /// <param name="musicFolderId"> From get Music Folders</param>
        /// <exception cref="SubSharpException"> Will throw SubSharpException for bad args</exception>
        /// <returns>A linked list of dictionaries</returns>
        public  LinkedList<Dictionary<String,String>> getRandomSongs(int size = 10, String genre = "", int fromYear = 0, int toYear = 0, int musicFolderId = -1)
        {
            LinkedList<string> my_Params = new LinkedList<string>();

            if (size <= 0 )
            {
                throw new SubSharpException("Requesting number of songs is less than or equal to zero");
            }
            else if (size > 500)
            {
                size = 500;
            }

            my_Params.AddLast("size");
            my_Params.AddLast(size.ToString());

            if (!String.IsNullOrEmpty(genre))
            {
                my_Params.AddLast("genre");
                my_Params.AddLast(genre);
            }

            if (fromYear < 0)
            {
                throw new SubSharpException("From year parameter is negative");
            }
            else if (fromYear > 0)
            {
                my_Params.AddLast("fromYear");
                my_Params.AddLast(fromYear.ToString());
            }

            if (toYear < 0)
            {
                throw new SubSharpException("From year parameter is negative");
            }
            else if (toYear > 0)
            {
                my_Params.AddLast("toYear");
                my_Params.AddLast(toYear.ToString());
            }

            if (musicFolderId > 0)
            {
                my_Params.AddLast("musicFolderId");
                my_Params.AddLast(musicFolderId.ToString());
            }

            action = "getRandomSongs.view";

            return gen_SubResponse(action, my_Params.ToArray()).Result_To_Attribute_Dict("song");
        }

        

        /// <summary>
        /// Get Similiar Songs based on folder structure
        /// </summary>
        /// <param name="keyType"></param>
        /// <param name="song_id"> The song id based on Folders</param>
        /// <param name="count"></param>
        /// <exception cref="SubSharpException"> Will throw SubSharpException for bad args</exception>
        /// <returns>A linked list of Dictionaries for each song element</returns>
        public LinkedList<Dictionary<String, String>> getSimiliarSongs( String song_id, int count = 10)
        {
            if (String.IsNullOrEmpty(song_id))
            {
                throw new SubSharpException("< Get similiar songs requested Song id is null or empty! >");
            }


            if (count <= 0)
            {
                throw new SubSharpException("Number of Songs cannot be equal to or lesser than zero");
            }


            action = "getSimilarSongs.view";


            string[] paramas = { "id", song_id, "count", count.ToString() };
            
            return gen_SubResponse(action, paramas).Result_To_Attribute_Dict("song");
        }


        /// <summary>
        /// Returns a Linked List with a dictionary foreach song element
        /// </summary>
        /// <param name="song_id">Song id based on id3 tags</param>
        /// <param name="count"></param>
        /// /// <exception cref="SubSharpException"> Will throw SubSharpException for bad args</exception>
        /// <returns>A linked list of Dictionaries</returns>
        public LinkedList<Dictionary<String,String>> getSimiliarSongs_2( String song_id, int count = 50)
        {
            if (String.IsNullOrEmpty(song_id))
            {
                throw new SubSharpException("< Get similiar songs 2 requested Song id is null or empty! >");
            }


            if (count <= 0)
            {
                throw new SubSharpException("Number of Songs cannot be equal to or lesser than zero");
            }


            action = "getSimilarSongs2.view";


            string[] paramas = { "id", song_id, "count", count.ToString() };


            return gen_SubResponse(action, paramas).Result_To_Attribute_Dict("song");
        }

        /// <summary>
        /// Returns data on Artist as ICollection
        /// Max count is only a suggestion, it is not guaranteed
        /// [0] = Text Data  ::
        /// [1] = Linked List of Dictionary Data for each similiar artist
        /// </summary>
        /// <typeparamref name="artist_id"/>
        /// <param name="count"></param>
        /// <param name="includeNotPresent">Include artist not present in the library</param>
        /// <exception cref="SubSharpException"> Will throw SubSharpException for bad args</exception>
        /// <returns></returns>
        public ICollection[] getArtistInfo2(ref String artist_id, int count = 20, bool includeNotPresent = false)
        {

            if (count <= 0)
            {
                throw new SubSharpException("Requested count less equal to or less than zero");
            }

            action = "getArtistInfo2.view";


            string[] paramas = { "id", artist_id, "count", count.ToString(), "includeNotPresent", includeNotPresent.ToString() };

            var dict = new Dictionary<string, string>();
            var linked = new LinkedList<Dictionary<string, string>>();


            return gen_SubResponse(action, paramas).Results_To_Dicts(out dict, out linked);
        }

        public LinkedList<Dictionary<String, String>> getMusicFolders()
        {
            action = "getMusicFolders.view";

            return  gen_SubResponse(action, null).Result_To_Attribute_Dict("musicFolder");
            
        }
        /// <summary>
        /// Get genres from library
        /// </summary>
        /// <param name="keyType"></param>
        /// <param name="id"></param>
        /// <returns>A linked list with a dictionary for each genre element</returns>
        public LinkedList<Dictionary<String, String>> getGenres()
        {

            action = "getGenres.view";



            var dict = new Dictionary<string, string>();
            var linked = new LinkedList<Dictionary<string, string>>();


            return gen_SubResponse(action, null).Result_To_Attribute_Dict("genre");
        }

        public LinkedList<Dictionary<string, string>> getVideos()
        {

            action = "getVideos.view";



            var dict = new Dictionary<string, string>();
            var linked = new LinkedList<Dictionary<string, string>>();


            return gen_SubResponse(action, null).Result_To_Attribute_Dict("videos");
        }



        /// <summary>
        /// Get album list 2, which is according to ID3 tags
        /// </summary>
        /// <param name="keyType"></param>
        /// <param name="album_id"></param>
        /// <exception cref="SubSharpException"> Will throw SubSharpException for bad args</exception>
        /// <returns>A linked list with a dictionary for each album list element</returns>
        public LinkedList<Dictionary<string, string>> getAlbumList2( Album_List_Type type, int size, int offset, int[] fromToYears, String genre)
        {

            action = "getAlbumList2.view";

            if (size < 0)
            {
                throw new SubSharpException("Size requested is negative", new ArgumentException());
            }
            else if (size > 500)
            {

                size = 500;
            }

            LinkedList<string> my_Params = new LinkedList<string>();

            my_Params.AddLast("type");
            my_Params.AddLast(type.ToString());


            my_Params.AddLast("size");
            my_Params.AddLast(size.ToString());
            my_Params.AddLast("offset");
            my_Params.AddLast(offset.ToString());



            switch (type)
            {
                case Album_List_Type.byYear:

                    if (fromToYears != null)
                    {
                        my_Params.AddLast("fromYear");
                        my_Params.AddLast(fromToYears[0].ToString());

                        my_Params.AddLast("ToYear");
                        my_Params.AddLast(fromToYears[1].ToString());

                    }
                    break;

                case Album_List_Type.byGenre:

                    my_Params.AddLast("genre");
                    my_Params.AddLast(genre);

                    break;


            }

            return gen_SubResponse(action, my_Params.ToArray()).Result_To_Attribute_Dict("song");
        }

        /// <summary>
        /// Return a dictioanry of keyType and attributes
        /// KeyType is the attribute field name
        /// </summary>
        /// <param name="keyType"></param>
        /// <param name="id"></param>
        /// <returns>A dictioanry with dictionary values</returns>
        public LinkedList<Dictionary<String, String>> getAlbum(String id)
        {

            action = "getAlbum.view";

            string[] parameters = { "id", id };
            return gen_SubResponse(action, parameters).Result_To_Attribute_Dict("song");
        }


        /// <summary>
        /// A dictionary of album elements 
        /// Key = KeyType of attribute.value in album element tag
        /// Value =  Dict< Attribute.Name , Attribute.Value >
        /// </summary>
        /// <param name="keyType"></param>
        /// <param name="id"></param>
        /// <returns> a dictionary of nested albums keyed by KeyType</returns>
        public LinkedList<Dictionary<String, String>> getArtist( String id)
        {

            action = "getArtist.view";

            string[] parameters = { "id", id };
            return gen_SubResponse(action, parameters).Result_To_Attribute_Dict("album");
        }

        /// <summary>
        /// A dictionary of artist elements
        /// With key = Atrribute.Value of keyType in artist element tag
        /// </summary>
        /// <returns>a SubResponse containing a Dictionary response</returns>
        public LinkedList<Dictionary<String, String>> getArtists()
        {
            action = "getArtists.view";

            return gen_SubResponse(action, null).Result_To_Attribute_Dict("artist");


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

      
    }

}
