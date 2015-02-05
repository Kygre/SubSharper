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
using Windows.UI.Xaml.Media.Imaging;
using Windows.Web.Http;


namespace SubSharpLibrary.Client
{
    /// <summary>
    /// A class wrapper for Subsonic API methods
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

        public SubSharp(String username, String password, String ip, String port, String version, String appName = "SubClient", bool https = false)
        {

            String[] connect = { username, password, ip, port, version, appName };
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
        public SubSharp(string[] connect, bool https)
        {

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

        public void Dispose()
        {
            this.Dispose();
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
            if (connect.Count() != 6)
            {
                String dbg = "";

                foreach (String s in connect)
                {
                    dbg += s + "\n";
                }

                Debug.WriteLine(dbg);

                throw new SubSharpException("Connect settings not complete");
            }

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
                Debug.WriteLine(ex.Message);
                Debug.WriteLine("HResult = " + ex.HResult);
                throw new SubSharpException("Http Client threw an Exception", ex, SubSharpException.ErrorCode.GENERIC_SUBSHARP_EXCEPTION);
            }

        }

        
        private void Add_Parameters(Object[] parameters, out String saveParam)
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

           /// <summary>
        /// Ping to server 
        /// </summary>
        /// <exception cref="SubSharpException">Will throw exception if reuqesting user is not admin</exception>       
        /// <returns>A subresponse</returns>
        public Task<SubResponse> ping()
        {

            action = "ping.view";
            return gen_SubResponse(action, null);
        }


        /// FIXME Prevent deadlocking of result change to configure await false
        /// <summary>
        /// Return a SubResponse
        /// </summary>
        /// <param name="action"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private async Task<SubResponse> gen_SubResponse(string action, string[] parameters)
        {
            String saveParam = null;


            Add_Parameters(parameters, out saveParam);


            String http = this.url + action + this.parameters;

            Debug.WriteLine("URL = " + http);
            var get =  await (get_HTML(http)).ConfigureAwait(false);

            

            // dispose of actions and paremeters
            action = "";
            if (!String.IsNullOrEmpty(saveParam))
            {
                this.parameters = saveParam;
            }
            return new SubResponse(get);

        }

        private async Task<LinkedList<Dictionary<String,String>>> gen_SubResponse_Data_LinkedList(string action, string[] parameters, string AttributeTag = "")
        {
            String saveParam = null;


            Add_Parameters(parameters, out saveParam);


            String http = this.url + action + this.parameters;

            Debug.WriteLine("URL = " + http);
            var get = await get_HTML(http).ConfigureAwait(false);



            
            action = "";
            if (!String.IsNullOrEmpty(saveParam))
            {
                this.parameters = saveParam;
            }

            if (String.IsNullOrEmpty(AttributeTag))
            {
                // return dictionary
                return null;
            }
            else
            {
                return new SubResponse(get).Result_To_Attribute_Dict(AttributeTag);
            }

        }

        private async Task<ICollection[]> gen_SubResponse_Data_Collection_Array(string action, string[] parameters)
        {
            String saveParam = null;


            Add_Parameters(parameters, out saveParam);


            String http = this.url + action + this.parameters;

            Debug.WriteLine("URL = " + http);
            var get = await get_HTML(http).ConfigureAwait(false);




            action = "";
            if (!String.IsNullOrEmpty(saveParam))
            {
                this.parameters = saveParam;
            }

            return new SubResponse(get).Results_To_Dicts();

        }

        private async Task<BitmapImage> gen_SubResponse_For_Image(string action, string[] parameters)
        {
            String saveParam = null;


            Add_Parameters(parameters, out saveParam);


            String http = this.url + action + this.parameters;


            // dispose of actions and paremeters
            action = "";
            if (!String.IsNullOrEmpty(saveParam))
            {
                this.parameters = saveParam;
            }

            Debug.WriteLine("URL = " + http);

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


                var result = await httpClient.SendRequestAsync(new HttpRequestMessage(HttpMethod.Get, (uri)));

                var byto = await result.Content.ReadAsInputStreamAsync();

                BitmapImage bn = new BitmapImage();

                await bn.SetSourceAsync(byto.AsStreamForRead().AsRandomAccessStream());

                return bn;

            }
            // TODO Catch Network Exceptions
            catch (Exception ex)
            {

                Debug.WriteLine(ex.ToString());
                throw new SubSharpException("Failed to read image stream", ex, SubSharpException.ErrorCode.GENERIC_SUBSHARP_EXCEPTION);
            }



            

        }

        

        /// <summary>
        /// Will create a playlist of the given name
        /// Update is called for ad
        /// Is only used for creating playlist not for updating!
        /// </summary>
        /// <returns></returns>
        public Task<SubResponse> createPlaylist(String name = "")
        {

            if (String.IsNullOrEmpty(name))
            {
                throw new SubSharpException("Name cannot bet empty when creating a playlist");
            }
            action = "createPlaylist.view";

            String[] paramsas = { "name", name };

            
            return gen_SubResponse(action, paramsas);
        }

        public Task<SubResponse> deletePlaylist(String id = "")
        {

            if (String.IsNullOrEmpty(id))
            {
                throw new SubSharpException("Id cannot bet empty when deleting a playlist");
            }
            action = "deletePlaylist.view";

            String[] paramsas = { "id", id };
            return gen_SubResponse(action, paramsas);
        }



        /// FIXME Email Api developer for current usage of mulitple parameters
        /// <summary>
        /// Update an existing playlist according to params
        /// Null or empty collections are ignored
        /// </summary>
        /// <param name="song_ids_add"></param>
        /// <param name="song_ids_remove"></param>
        /// <param name="playist_id"> Get from GetPlaylists</param>
        /// <param name="name"></param>
        /// <param name="comment"></param>
        /// <param name="isPublic"> Will default to false</param>
        /// <returns>A SubResponse</returns>
        public Task<SubResponse> updatePlaylist( IEnumerable<String> song_ids_add, IEnumerable<String> song_ids_remove, String playist_id = "", String name = "", String comment = "", bool isPublic = false)
        {
            String song_adding = "", song_removing = "";


            if (String.IsNullOrEmpty(playist_id))
            {
                throw new SubSharpException("Name cannot bet empty when creating a playlist");
            }


            if (song_ids_add != null)
            {
                if (song_ids_add.Count() > 0)
                {
                    foreach (String song in song_ids_add)
                    {
                        song_adding += song + ",";
                    }

                    song_adding = song_adding.Substring(0, song_adding.LastIndexOf(","));
                }
            }

            if (song_ids_remove != null)
            {
                if (song_ids_remove.Count() > 0)
                {
                    foreach (String song in song_ids_remove)
                    {
                        song_removing += song + ",";
                    }

                    song_removing = song_removing.Substring(0, song_removing.LastIndexOf(","));
                }

            }
           
            

            action = "updatePlaylist.view";
            var paramas = new LinkedList<String>();

            paramas.AddLast("playlistId");
            paramas.AddLast(playist_id);


            if (!String.IsNullOrEmpty(name))
            {
                paramas.AddLast("name");
                paramas.AddLast(name);
            }

            if (!String.IsNullOrEmpty(comment))
            {
                paramas.AddLast("comment");
                paramas.AddLast(comment);
            }

            paramas.AddLast("public");
            paramas.AddLast(isPublic.ToString());

            if (!String.IsNullOrEmpty(song_adding))
            {
                paramas.AddLast("songIdToAdd");
                paramas.AddLast(song_adding);
            }

            if (!String.IsNullOrEmpty(song_removing))
            {
                paramas.AddLast("songIndexToRemove");
                paramas.AddLast(song_removing);
            }



            
            return gen_SubResponse(action, paramas.ToArray());
        }


        /// OPTIONAL Return as stream
        /// <summary>
        /// Return a task to get a bitmap image async'ly
        /// </summary>
        /// <param name="id"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public Task<BitmapImage> getCoverArt( String id , int size = 0)
        {

            if(String.IsNullOrEmpty(id)){
                throw new SubSharpException("Id for getting covert art cannot be empty");
            }

            if (size < 0)
            {
                throw new SubSharpException("Size for image cannot be less than zero");
            }

            var pars = new LinkedList<String>();

            pars.AddLast("id");
            pars.AddLast(id);

            if (size > 0)
            {
                pars.AddLast("size");
                pars.AddLast(size.ToString());

            }



            return gen_SubResponse_For_Image( action, pars.ToArray());
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
        public async Task<DownloadOperation> Stream(String id, int maxBitRate, String format, bool estimate_Content_Length, BackgroundDownloader bg, IStorageFile file)
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

            return null;
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

                 
                return bg.CreateDownload(uri, file);


            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                throw ex;
            }


            */


        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <exception cref="SubSharpException">Will throw exception if reuqesting user is not admin</exception>       
        /// <returns>A linked list with a dictionary for each playlist element</returns>
        public async Task<LinkedList<Dictionary<String, String>>> getPlaylists(string username = "")
        {

            LinkedList<string> my_Params = new LinkedList<string>();

            if (!String.IsNullOrEmpty(username))
            {
                my_Params.AddLast("username");
                my_Params.AddLast(username);
            }


            action = "getPlaylists.view";

            var result = await gen_SubResponse(action, my_Params.ToArray());

            return result.Result_To_Attribute_Dict("playlist");
                     

            
        }

        /// <summary>
        /// Gets songs by genre
        /// </summary>
        /// <param name="genre"></param>
        /// <param name="count">Optional, must be greater than zero</param>
        /// <param name="offset">Optional for offsetting pages</param>
        /// <returns>A linked list of dictionaries for each song element</returns>
        public async Task<LinkedList<Dictionary<String, String>>> getSongsByGenre( String genre, int count = 10, int offset = 0)
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

            var result = await gen_SubResponse(action, my_Params.ToArray());
            
            return result.Result_To_Attribute_Dict("song");
            
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
        public async  Task<LinkedList<Dictionary<String,String>>> getRandomSongs(int size = 10, String genre = "", int fromYear = 0, int toYear = 0, int musicFolderId = -1)
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


            var result = await gen_SubResponse(action, my_Params.ToArray());

            return result.Result_To_Attribute_Dict("song");
            
        }

        

        /// <summary>
        /// Get Similiar Songs based on folder structure
        /// </summary>
        /// <param name="keyType"></param>
        /// <param name="song_id"> The song id based on Folders</param>
        /// <param name="count"></param>
        /// <exception cref="SubSharpException"> Will throw SubSharpException for bad args</exception>
        /// <returns>A linked list of Dictionaries for each song element</returns>
        public async Task<LinkedList<Dictionary<String, String>>> getSimiliarSongs( String song_id, int count = 10)
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


            string[] my_Params = { "id", song_id, "count", count.ToString() };


            var result = await gen_SubResponse(action, my_Params);

            return result.Result_To_Attribute_Dict("song");
            
        }


        /// <summary>
        /// Returns a Linked List with a dictionary foreach song element
        /// </summary>
        /// <param name="song_id">Song id based on id3 tags</param>
        /// <param name="count"></param>
        /// /// <exception cref="SubSharpException"> Will throw SubSharpException for bad args</exception>
        /// <returns>A linked list of Dictionaries</returns>
        public async Task<LinkedList<Dictionary<String, String>>> getSimiliarSongs_2(String song_id, int count = 50)
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


            string[] my_Params = { "id", song_id, "count", count.ToString() };


            var result = await gen_SubResponse(action, my_Params);

            return result.Result_To_Attribute_Dict("song");
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
        public Task<ICollection[]> getArtistInfo2(ref String artist_id, int count = 20, bool includeNotPresent = false)
        {

            if (count <= 0)
            {
                throw new SubSharpException("Requested count less equal to or less than zero");
            }

            action = "getArtistInfo2.view";


            string[] paramas = { "id", artist_id, "count", count.ToString(), "includeNotPresent", includeNotPresent.ToString() };


            return gen_SubResponse_Data_Collection_Array(action, paramas);
            
        }

        /// <summary>
        /// Return a dictionary and a linked list of dictionaries for each entry element in the playlist
        /// </summary>
        /// <param name="playlist_id"></param>
        /// <returns></returns>
        public Task<ICollection[]> getPlaylist( String playlist_id = "" )
        {

            if (String.IsNullOrEmpty(playlist_id))
            {
                throw new SubSharpException("Requested count less equal to or less than zero");
            }

            action = "getPlaylist.view";


            string[] paramas = { "id", playlist_id};

            return gen_SubResponse_Data_Collection_Array(action, paramas);
        }


        public async Task<LinkedList<Dictionary<String, String>>> getMusicFolders()
        {
            action = "getMusicFolders.view";

            var result = await gen_SubResponse(action, null);

            return result.Result_To_Attribute_Dict("musicFolder");
        }
        /// <summary>
        /// Get genres from library
        /// </summary>
        /// <param name="keyType"></param>
        /// <param name="id"></param>
        /// <returns>A linked list with a dictionary for each genre element</returns>
        public Task<LinkedList<Dictionary<String, String>>> getGenres()
        {

            action = "getGenres.view";

            return gen_SubResponse_Data_LinkedList(action, null, "genre");
        }

        public Task<LinkedList<Dictionary<string, string>>> getVideos()
        {
            action = "getVideos.view";

            return gen_SubResponse_Data_LinkedList(action, null, "videos");
        }

        /// <summary>
        /// Get the songs that are now playing
        /// Can return empty
        /// </summary>
        /// <returns>A linked list with a dictionary for each entry element</returns>
        public Task<LinkedList<Dictionary<string, string>>> getNowPlaying()
        {

            action = "getNowPlaying.view";

            return gen_SubResponse_Data_LinkedList(action, null, "entry");
            
        }

        public async Task<LinkedList<Dictionary<String, String>>[]> getStarred()
        {
            action = "getStarred.view";

            var sub = await gen_SubResponse(action, null).ConfigureAwait(false);
            return sub.result_get_all_Descendants();
        }

        /// <summary>
        /// Returns an Linked List of dictionaries for each type of return
        /// [0] = Artists
        /// [1] = Albums
        /// [2] = Songs
        /// Each has a dictionary of all attribute xml tags
        /// </summary>
        /// <returns></returns>
        public async Task<LinkedList<Dictionary<String, String>>[]> getStarred_2()
        {

            action = "getStarred2.view";


            var sub = await gen_SubResponse(action, null).ConfigureAwait(false);
            return sub.result_get_all_Descendants();
        }
        /// <summary>
        /// Get album list 2, which is according to ID3 tags
        /// </summary>
        /// <param name="keyType"></param>
        /// <param name="album_id"></param>
        /// <exception cref="SubSharpException"> Will throw SubSharpException for bad args</exception>
        /// <returns>A linked list with a dictionary for each album list element</returns>
        public Task<LinkedList<Dictionary<string, string>>> getAlbumList(Album_List_Type type, int size, int offset, int[] fromToYears, String genre)
        {

            action = "getAlbumList.view";

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

            return gen_SubResponse_Data_LinkedList(action, my_Params.ToArray(), "song");
            
        }

        

        /// <summary>
        /// Get album list 2, which is according to ID3 tags
        /// </summary>
        /// <param name="keyType"></param>
        /// <param name="album_id"></param>
        /// <exception cref="SubSharpException"> Will throw SubSharpException for bad args</exception>
        /// <returns>A linked list with a dictionary for each album list element</returns>
        public Task<LinkedList<Dictionary<string, string>>> getAlbumList2( Album_List_Type type, int size, int offset, int[] fromToYears, String genre)
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

            return gen_SubResponse_Data_LinkedList(action, my_Params.ToArray(), "song");
            
        }

        /// <summary>
        /// Return a dictioanry of keyType and attributes
        /// KeyType is the attribute field name
        /// </summary>
        /// <param name="keyType"></param>
        /// <param name="id"></param>
        /// <returns>A dictioanry with dictionary values</returns>
        public Task<LinkedList<Dictionary<String, String>>> getAlbum(String id)
        {

            action = "getAlbum.view";

            string[] parameters = { "id", id };
            return gen_SubResponse_Data_LinkedList(action, parameters, "song");
            
        }


        /// <summary>
        /// A dictionary of album elements 
        /// Key = KeyType of attribute.value in album element tag
        /// Value =  Dict< Attribute.Name , Attribute.Value >
        /// </summary>
        /// <param name="keyType"></param>
        /// <param name="id"></param>
        /// <returns> a dictionary of nested albums keyed by KeyType</returns>
        public Task<LinkedList<Dictionary<String, String>>> getArtist( String id)
        {

            action = "getArtist.view";

            string[] parameters = { "id", id };
            return gen_SubResponse_Data_LinkedList(action, parameters, "album");
        }

        /// <summary>
        /// A dictionary of artist elements
        /// With key = Atrribute.Value of keyType in artist element tag
        /// </summary>
        /// <returns>a SubResponse containing a Dictionary response</returns>
        public Task<LinkedList<Dictionary<String, String>>> getArtists()
        {
            action = "getArtists.view";
            return gen_SubResponse_Data_LinkedList(action, null, "artist");
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

        public string Url   
        {
            get
            {
                return url;
            }
        }

        
    }

}
