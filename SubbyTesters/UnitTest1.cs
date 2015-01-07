using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using SubSharpLibrary.Client;
using SubSharpLibrary.Exceptions;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using Windows.Foundation;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;
using System.Threading.Tasks;


namespace SubSharpLibrary.Tests
{
    [TestClass]
    public class SubSharpTests
    {
        /**
            * [0] = user
            * [1] = password
            * [2] = ip
            * [3] = port -- 
            * [4] = version
            * [5] = AppName
         
            */

        private static SubSharp sharp; // for testing supported version

        // for Testing ON Active Server -- Check Daily

        private static int Total_Artist = 377;
        private static int Total_Test_Album = 15;
        private static String Test_Artist_Id_id3 = "721";
        private static String Test_Artist_Id_File = "721";
        private static String Test_Username = "brittany";
        private static int Total_Test_Songs = 4;
        private static String Test_Album_Id = "3341";





        private static SubSharp sharp_beta; // for testing subsonic beta version

        // INIT Subsonic Connections
        [ClassInitialize()]
        public static void setUp(TestContext tx)
        {
            string[] args = { "kj", "soccerg23", "kygre.subsonic.org", "", "1.11.0", "subClient" };
            sharp = new SubSharp(args, false);

            args[4] = "1.11.0";
            sharp_beta = new SubSharp(args, false);
            Debug.WriteLine("Starting Tests");


        }

        // region for SubSharp constructor tests
        #region


        [TestMethod]
        // should fail due to blank argument where it should not be
        public void Fail_init_BadArguments()
        {
            string[] args = { "kj", "soccerg23", "kygre.subsonic.org", "", "", "subClient" };

            try
            {
                var check = new SubSharp(args, false);
                Assert.Fail("Argument Exception not caught!");
            }
            catch (ArgumentException ex)
            {

                StringAssert.Contains(ex.Message, "Failed to Initiliaze SubSharp \nAn argument field is empty");

            }



        }
        #endregion



        // test ping for SubSonic response with html call
        /// <summary>
        /// Uses Global Field sharp to test ping response isOk using HTML
        /// to active subsonic server
        /// </summary>
        [TestMethod]
        public void test_html_ping()
        {

            Debug.WriteLine(sharp == null);

            Assert.IsTrue(sharp.ping().get_isOk);



        }

        // Stream must be testing using WP8.1 application
        // Here for show
        [TestMethod]
        public void test_Stream_No_Work()
        {

            BackgroundDownloader bg = new BackgroundDownloader();


            // Get the local folder.
            StorageFolder local = Windows.Storage.ApplicationData.Current.LocalFolder;

            // Create a new folder name DataFolder.

            String msg = "";

            var File = Create_File(msg, local).Result;



            var Download = sharp.test_Stream("DataFile.txt", 320, "mp3", true, bg, null);



            msg += "\n Download To String " + Download.ToString();
            //msg += "\n File Name" + Download.ResultFile.Name.ToString();


            msg += "\n Method Donwload -- " + Download.Method.ToString();
            msg += "\n Progress" + Download.Progress;


            msg += "\n Properties -- " + File.Properties.ToString();
            msg += "\n -- path " + File.Path;
            msg += "\n -- content type " + File.ContentType;
            msg += "\n -- attributes " + File.Attributes;
            Debug.WriteLine(msg);

            Guid x = Guid.NewGuid();
            Debug.WriteLine(x.ToString());
            // start test Download


        }




        private static async System.Threading.Tasks.Task<StorageFile> Create_File(String msg, StorageFolder local)
        {

            var dataFolder = await local.CreateFolderAsync("DataFolder",
                CreationCollisionOption.OpenIfExists);

            // Create a new file named DataFile.txt.
            var file = await dataFolder.CreateFileAsync("DataFile.txt",
            CreationCollisionOption.ReplaceExisting);

            msg += "\n FIle Test - " + file.DateCreated + " -- " + file.DisplayName;
            return file;
        }



        [TestMethod]
        public void test_getAlbum_html_request()
        {

            string msg = "Using key --" + Test_Album_Id + "--";

            Debug.WriteLine(msg);

            var result = sharp.getAlbum(Test_Album_Id);

            Assert.AreEqual(Total_Test_Songs, result.Count, "Song count not equal");
        }
        [TestMethod]
        public void test_getArtist_html_request()
        {

            string msg = "Using key --" + Test_Artist_Id_id3 + "--";

            Debug.WriteLine(msg);

            var result = sharp.getArtist( Test_Artist_Id_id3);

            Assert.AreEqual(Total_Test_Album, result.Count, "Albums count not equal");

        }

        [TestMethod]
        public void test_getPlaylists_Exceptions()
        {
            try
            {
                var result = sharp.getPlaylists(Test_Username);
                Assert.Fail("Failed to throw SubSharp Exception");
            }
            catch (SubSharpException)
            {

                Assert.IsTrue(true);

            }


        }

        [TestMethod]
        public void test_getVideo()
        {
            var resi = sharp.getVideos();


        }

        [TestMethod]
        public void test_getPlaylists()
        {
            try
            {
                var result = sharp.getPlaylists();
                Assert.IsTrue(result.Count > 0);

                print(result);
            }
            catch (SubSharpException ex)
            {

                Assert.Fail("Threw a SubSharp Exception\n" + ex.Message);
            }


        }

        [TestMethod]
        public void test_getRandomSongs()
        {
            int count = 10;
            var result = sharp.getRandomSongs();


            Assert.AreEqual(count, result.Count);
        }
        /**
        [TestMethod]
        public void test_get_Similiar_Song()
        {
            String keytag = "id";


            var response = sharp.getSimiliarSongs(ref keytag, Test_Artist_Id_id3);

            
            Assert.AreEqual( 50 , response[1].Count);
        }
         * 
         * */

        [TestMethod]
        public void test_getArtistInfo2()
        {
            var response = sharp.getArtistInfo2(ref Test_Artist_Id_id3);

            Assert.AreEqual(20, response[1].Count);

            Assert.IsTrue(response[0].Count > 0);
        }


        [TestMethod]
        public void test_get_Similiar_Song2()
        {
            
            var response = sharp.getSimiliarSongs_2(Test_Artist_Id_id3);


            Assert.AreEqual(50, response.Count);
        }


        [TestMethod]
        public void test_get_Similiar_Song2_NegativeSongs_Exception()
        {


            try
            {
                
                var response = sharp.getSimiliarSongs_2(Test_Artist_Id_id3, -10);
                Assert.Fail();
            }
            catch (SubSharpException ex)
            {

                Assert.AreEqual(ex.Message, "Number of Songs cannot be equal to or lesser than zero");
                Assert.IsTrue(true);

            }


        }

        [TestMethod]
        public void test_getAlbumList2()
        {

            int[] fromTo = null;
            String genre = null;

            var result = sharp.getAlbumList2( SubSharp.Album_List_Type.newest, 10, 0, fromTo, genre);
            Assert.IsTrue(sharp.ping().get_isOk, "Failed to get Album List 2");

        }
        // check if returns xml containing artists
        // test can total count depending on server
        [TestMethod]
        public void test_getArtists()
        {
            var result = sharp.getArtists();
            Assert.IsTrue(sharp.ping().get_isOk, "Failed to getArtists");
            Debug.WriteLine("Artist Count = " + result.Count);

            Assert.AreEqual(Total_Artist, result.Count);
        }


        public void print(LinkedList<Dictionary<String, String>> ldict)
        {
            foreach (var args in ldict)
            {
                foreach (KeyValuePair<string, string> kvp in args)
                {
                    Debug.WriteLine(" -- Value " + kvp.Key.ToString() + " -- " + kvp.Value.ToString());
                }

            }


        }

        // TODO Test Method to check for argument erorr using Subsonic domain

        // TODO Tests for different constructors


    }


    [TestClass]
    public class SubResponseTests
    {




        [TestMethod]
        // [ExpectedException(typeof(System.DivideByZeroException))] -- not supported
        public void test_Constructor_SubSharpException()
        {

            try
            {
                var resp = new SubResponse(null);
                Assert.Fail();
            }
            catch (SubSharpException)
            {
                Assert.IsTrue(true);

            } // fail for other exceptions
            catch (Exception)
            {
                Assert.Fail();
            }

        }
        // tests ping reader without using html call
        [TestMethod]
        public void OkPingResponseTester()
        {

            String xdoc = "<subsonic-response xmlns=\"http://subsonic.org/restapi\" status=\"ok\" version=\"1.10.2\"> </subsonic-response>";

            var response = new SubResponse(xdoc);

            Debug.WriteLine(response.get_isOk);

            Assert.IsTrue(response.get_isOk);
        }

        /// <summary>
        /// Should test null is returned for empty elements or attributes
        /// </summary>
        [TestMethod]
        public void result_to_ICollection_LinkedList()
        {
            var response = "<subsonic-response xmlns=\"http://subsonic.org/restapi\" status=\"ok\" version=\"1.8.0\">\r\n<albumList2>\r\n<album id=\"1768\" name=\"Duets\" coverArt=\"al-1768\" songCount=\"2\" created=\"2002-11-09T15:44:40\" duration=\"514\" artist=\"Nik Kershaw\" artistId=\"829\"/>\r\n<album id=\"2277\" name=\"Hot\" coverArt=\"al-2277\" songCount=\"4\" created=\"2004-11-28T00:06:52\" duration=\"1110\" artist=\"Melanie B\" artistId=\"1242\"/>\r\n</albumList2>\r\n</subsonic-response>";

            var result = new SubResponse(response);

            int AttributeNames = 8;

            var test_dict = new Dictionary<string, string>();
            var linked = new LinkedList<Dictionary<string, string>>();


            result.Results_To_Dicts(out test_dict, out linked);

            Assert.IsTrue(result.get_isOk);
            Assert.AreEqual(2, linked.Count); // element number
            Assert.AreEqual(AttributeNames, linked.First.Value.Count);


        }

        // lazy test print
        [TestMethod]
        public void test_Get_Album()
        {
            String xdoc = "<subsonic-response xmlns=\"http://subsonic.org/restapi\" status=\"ok\" version=\"1.8.0\">\r\n<album id=\"11053\" name=\"High Voltage\" coverArt=\"al-11053\" songCount=\"8\" created=\"2004-11-27T20:23:32\" duration=\"2414\" artist=\"AC/DC\" artistId=\"5432\">\r\n<song id=\"71463\" parent=\"71381\" title=\"The Jack\" album=\"High Voltage\" artist=\"AC/DC\" isDir=\"false\" coverArt=\"71381\" created=\"2004-11-08T23:36:11\" duration=\"352\" bitRate=\"128\" size=\"5624132\" suffix=\"mp3\" contentType=\"audio/mpeg\" isVideo=\"false\" path=\"ACDC/High voltage/ACDC - The Jack.mp3\" albumId=\"11053\" artistId=\"5432\" type=\"music\"/>\r\n<song id=\"71464\" parent=\"71381\" title=\"Tnt\" album=\"High Voltage\" artist=\"AC/DC\" isDir=\"false\" coverArt=\"71381\" created=\"2004-11-08T23:36:11\" duration=\"215\" bitRate=\"128\" size=\"3433798\" suffix=\"mp3\" contentType=\"audio/mpeg\" isVideo=\"false\" path=\"ACDC/High voltage/ACDC - TNT.mp3\" albumId=\"11053\" artistId=\"5432\" type=\"music\"/>\r\n<song id=\"71458\" parent=\"71381\" title=\"It's A Long Way To The Top\" album=\"High Voltage\" artist=\"AC/DC\" isDir=\"false\" coverArt=\"71381\" created=\"2004-11-27T20:23:32\" duration=\"315\" bitRate=\"128\" year=\"1976\" genre=\"Rock\" size=\"5037357\" suffix=\"mp3\" contentType=\"audio/mpeg\" isVideo=\"false\" path=\"ACDC/High voltage/ACDC - It's a long way to the top if you wanna rock 'n 'roll.mp3\" albumId=\"11053\" artistId=\"5432\" type=\"music\"/>\r\n<song id=\"71461\" parent=\"71381\" title=\"Rock 'n' Roll Singer.\" album=\"High Voltage\" artist=\"AC/DC\" isDir=\"false\" coverArt=\"71381\" created=\"2004-11-27T20:23:33\" duration=\"303\" bitRate=\"128\" track=\"2\" year=\"1976\" genre=\"Rock\" size=\"4861680\" suffix=\"mp3\" contentType=\"audio/mpeg\" isVideo=\"false\" path=\"ACDC/High voltage/ACDC - Rock N Roll Singer.mp3\" albumId=\"11053\" artistId=\"5432\" type=\"music\"/>\r\n<song id=\"71460\" parent=\"71381\" title=\"Live Wire\" album=\"High Voltage\" artist=\"AC/DC\" isDir=\"false\" coverArt=\"71381\" created=\"2004-11-27T20:23:33\" duration=\"349\" bitRate=\"128\" track=\"4\" year=\"1976\" genre=\"Rock\" size=\"5600206\" suffix=\"mp3\" contentType=\"audio/mpeg\" isVideo=\"false\" path=\"ACDC/High voltage/ACDC - Live Wire.mp3\" albumId=\"11053\" artistId=\"5432\" type=\"music\"/>\r\n<song id=\"71456\" parent=\"71381\" title=\"Can I sit next to you girl\" album=\"High Voltage\" artist=\"AC/DC\" isDir=\"false\" coverArt=\"71381\" created=\"2004-11-27T20:23:32\" duration=\"251\" bitRate=\"128\" track=\"6\" year=\"1976\" genre=\"Rock\" size=\"4028276\" suffix=\"mp3\" contentType=\"audio/mpeg\" isVideo=\"false\" path=\"ACDC/High voltage/ACDC - Can I Sit Next To You Girl.mp3\" albumId=\"11053\" artistId=\"5432\" type=\"music\"/>\r\n<song id=\"71459\" parent=\"71381\" title=\"Little Lover\" album=\"High Voltage\" artist=\"AC/DC\" isDir=\"false\" coverArt=\"71381\" created=\"2004-11-27T20:23:33\" duration=\"339\" bitRate=\"128\" track=\"7\" year=\"1976\" genre=\"Rock\" size=\"5435119\" suffix=\"mp3\" contentType=\"audio/mpeg\" isVideo=\"false\" path=\"ACDC/High voltage/ACDC - Little Lover.mp3\" albumId=\"11053\" artistId=\"5432\" type=\"music\"/>\r\n<song id=\"71462\" parent=\"71381\" title=\"She's Got Balls\" album=\"High Voltage\" artist=\"AC/DC\" isDir=\"false\" coverArt=\"71381\" created=\"2004-11-27T20:23:34\" duration=\"290\" bitRate=\"128\" track=\"8\" year=\"1976\" genre=\"Rock\" size=\"4651866\" suffix=\"mp3\" contentType=\"audio/mpeg\" isVideo=\"false\" path=\"ACDC/High voltage/ACDC - Shes Got Balls.mp3\" albumId=\"11053\" artistId=\"5432\" type=\"music\"/>\r\n</album>\r\n</subsonic-response>";

            var response = new SubResponse(xdoc);

            Debug.WriteLine(response.get_isOk);

            var lists = response.Result_To_Attribute_Dict("song");

            Assert.AreEqual(8, lists.Count);

        }
        /// <summary>
        /// Test get to get Albums without HTML Call
        /// </summary>
        [TestMethod]
        public void test_GetArtist_Response()
        {
            String albumResponse = "<subsonic-response xmlns=\"http://subsonic.org/restapi\" status=\"ok\" version=\"1.8.0\">\r\n<artist id=\"5432\" name=\"AC/DC\" coverArt=\"ar-5432\" albumCount=\"15\">\r\n<album id=\"11047\" name=\"Back In Black\" coverArt=\"al-11047\" songCount=\"10\" created=\"2004-11-08T23:33:11\" duration=\"2534\" artist=\"AC/DC\" artistId=\"5432\"/>\r\n</artist>\r\n</subsonic-response>";

            var response = new SubResponse(albumResponse);





        }

        // tests get Dictionary without HTML call
        /// <summary>
        /// Iffy dictionary checker
        /// Tried using CollectionAssert for dictionaries but failed miserably
        /// Compares using lists insteads
        /// </summary>
        [TestMethod]
        public void test_Dictionary_Of_Atrribute_Elements()
        {


            String artist = "<subsonic-response xmlns=\"http://subsonic.org/restapi\" status=\"ok\" version=\"1.10.1\">\r\n<artists ignoredArticles=\"The El La Los Las Le Les\">\r\n<index name=\"A\">\r\n<artist id=\"5449\" name=\"A-Ha\" coverArt=\"ar-5449\" albumCount=\"4\"/>\r\n</index>\r\n</artists>\r\n</subsonic-response>";


            var response = new SubResponse(artist);




            // keys are equal

        }




        public void print(LinkedList<Dictionary<String, String>> list)
        {
            Debug.WriteLine("Printing results of dictionaries");

            foreach (Dictionary<String, String> name in list)
            {

                foreach (KeyValuePair<string, string> kvp in name)
                {
                    Debug.WriteLine(" -- Value " + kvp.Key.ToString() + " -- " + kvp.Value.ToString());
                }

            }
        }
        // region for SubResponseTests Error Tests
        #region

        ///
        /// Following Tests are to check SubResponse throws a SubSharpException and sets an ErrorCode enum
        /// The Message is not really checked..

        /// <summary>
        /// Should return a generic error enum 
        /// </summary>
        [TestMethod]
        public void test_Generic_Error_Enum()
        {
            String anoxdox = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><subsonic-response xmlns=\"http://subsonic.org/restapi\" status=\"failed\" version=\"1.1.0\"><error code=\"0\" message=\"Wrong username or password\"/></subsonic-response>";

            try
            {
                var response = new SubResponse(anoxdox);
                Assert.Fail("Failed to throw SubSharpException and throw SubsonicErroCode");
            }
            catch (SubSharpException sx)
            {
                Assert.AreEqual(SubSharpException.ErrorCode.GENERIC, sx.get_SubsonicErrorCode);

            }

        }

        [TestMethod]
        public void test_Not_Generic_Error_Enum()
        {
            String anoxdox = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><subsonic-response xmlns=\"http://subsonic.org/restapi\" status=\"failed\" version=\"1.1.0\"><error code=\"10\" message=\"Wrong username or password\"/></subsonic-response>";

            try
            {
                var response = new SubResponse(anoxdox);
                Assert.Fail("Failed to throw SubSharpException and throw SubsonicErroCode");
            }
            catch (SubSharpException sx)
            {
                Assert.AreNotEqual(SubSharpException.ErrorCode.GENERIC, sx.get_SubsonicErrorCode);

            }

        }


        [TestMethod]
        public void test_Parameters_Missing_Error()
        {
            String anoxdox = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><subsonic-response xmlns=\"http://subsonic.org/restapi\" status=\"failed\" version=\"1.1.0\"><error code=\"10\" message=\"Wrong username or password\"/></subsonic-response>";

            try
            {
                var response = new SubResponse(anoxdox);
                Assert.Fail("Failed to throw SubSharpException and throw SubsonicErroCode");
            }
            catch (SubSharpException sx)
            {
                Assert.AreEqual(SubSharpException.ErrorCode.PARAMETERS_MISSING, sx.get_SubsonicErrorCode);

            }
        }


        [TestMethod]
        public void test_Rest_Client_Error()
        {
            String anoxdox = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><subsonic-response xmlns=\"http://subsonic.org/restapi\" status=\"failed\" version=\"1.1.0\"><error code=\"20\" message=\"Wrong username or password\"/></subsonic-response>";

            try
            {
                var response = new SubResponse(anoxdox);
                Assert.Fail("Failed to throw SubSharpException and throw SubsonicErroCode");
            }
            catch (SubSharpException sx)
            {
                Assert.AreEqual(SubSharpException.ErrorCode.REST_CLIENT_INCOMPATILE, sx.get_SubsonicErrorCode);

            }
        }

        [TestMethod]
        public void test_Rest_Server_Error()
        {
            String anoxdox = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><subsonic-response xmlns=\"http://subsonic.org/restapi\" status=\"failed\" version=\"1.1.0\"><error code=\"30\" message=\"Wrong username or password\"/></subsonic-response>";

            try
            {
                var response = new SubResponse(anoxdox);
                Assert.Fail("Failed to throw SubSharpException and throw SubsonicErroCode");
            }
            catch (SubSharpException sx)
            {
                Assert.AreEqual(SubSharpException.ErrorCode.REST_SERVER_INCOMPATIBLE, sx.get_SubsonicErrorCode);

            }
        }
        [TestMethod]
        public void test_Wrong_USER_Pass_Error()
        {
            String anoxdox = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><subsonic-response xmlns=\"http://subsonic.org/restapi\" status=\"failed\" version=\"1.1.0\"><error code=\"40\" message=\"Wrong username or password\"/></subsonic-response>";

            try
            {
                var response = new SubResponse(anoxdox);
                Assert.Fail("Failed to throw SubSharpException and throw SubsonicErroCode");
            }
            catch (SubSharpException sx)
            {
                Assert.AreEqual(SubSharpException.ErrorCode.WRONG_USER_OR_PASS, sx.get_SubsonicErrorCode);

            }
        }
        [TestMethod]
        public void test_UserNotAuthorizedError()
        {
            String anoxdox = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><subsonic-response xmlns=\"http://subsonic.org/restapi\" status=\"failed\" version=\"1.1.0\"><error code=\"50\" message=\"Wrong username or password\"/></subsonic-response>";

            try
            {
                var response = new SubResponse(anoxdox);
                Assert.Fail("Failed to throw SubSharpException and throw SubsonicErroCode");
            }
            catch (SubSharpException sx)
            {
                Assert.AreEqual(SubSharpException.ErrorCode.USER_NOT_AUTHORIZED, sx.get_SubsonicErrorCode);

            }
        }
        [TestMethod]
        public void test_ServerNotPremiumError()
        {
            String anoxdox = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><subsonic-response xmlns=\"http://subsonic.org/restapi\" status=\"failed\" version=\"1.1.0\"><error code=\"60\" message=\"Wrong username or password\"/></subsonic-response>";

            try
            {
                var response = new SubResponse(anoxdox);
                Assert.Fail("Failed to throw SubSharpException and throw SubsonicErroCode");
            }
            catch (SubSharpException sx)
            {
                Assert.AreEqual(SubSharpException.ErrorCode.SERVER_NO_PREMIUM, sx.get_SubsonicErrorCode);

            }
        }

        [TestMethod]
        public void test_Data_Not_Found_Error()
        {
            String anoxdox = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><subsonic-response xmlns=\"http://subsonic.org/restapi\" status=\"failed\" version=\"1.1.0\"><error code=\"70\" message=\"Wrong username or password\"/></subsonic-response>";

            try
            {
                var response = new SubResponse(anoxdox);
                Assert.Fail("Failed to throw SubSharpException and throw SubsonicErroCode");
            }
            catch (SubSharpException sx)
            {
                Assert.AreEqual(SubSharpException.ErrorCode.DATA_NOT_FOUND, sx.get_SubsonicErrorCode);

            }
        }

        #endregion
    }

}
