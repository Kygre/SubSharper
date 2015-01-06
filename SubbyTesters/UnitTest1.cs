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

        private static int Total_Artist = 341;
        private static int Total_Test_Album = 15;
        private static String Test_Artist_Id = "721";

        private static int Total_Test_Songs = 4;
        private static String Test_Album_Id = "3341";

        
        


        private static SubSharp sharp_beta; // for testing subsonic beta version

        // INIT Subsonic Connections
        [ClassInitialize()]
        public static void setUp(TestContext tx)
        {
            string[] args = { "kj", "soccerg23", "kygre.subsonic.org", "", "1.10.2", "subClient" };
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

            

            var Download = sharp.test_Stream( "DataFile.txt", 320, "mp3", true, bg, null);

            
            
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


       

        private static async System.Threading.Tasks.Task<StorageFile> Create_File(String msg, StorageFolder local )
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

            var result = sharp.getAlbum("id", Test_Album_Id);

            Assert.AreEqual(Total_Test_Songs, result.Count, "Song count not equal");
        }
        [TestMethod]
        public void test_getArtist_html_request()
        {
            
            string msg = "Using key --" + Test_Artist_Id + "--";

            Debug.WriteLine(msg);

            var result = sharp.getArtist("id", Test_Artist_Id);

            Assert.AreEqual( Total_Test_Album , result.Count, "Albums count not equal");

        }


        [TestMethod]
        public void test_get_Similiar_Song2()
        {
            String keytag = "id";
            
            
            var response = sharp.getSimiliarSongs_2(ref keytag, Test_Artist_Id);

            
            Assert.AreEqual( 50 , response.Count);
        }


        [TestMethod]
        public void test_get_Similiar_Song2_NegativeSongs_Exception()
        {
            
            
            try
            {
                String id = "id";
                var response = sharp.getSimiliarSongs_2( ref id, Test_Artist_Id, -10);
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

            var result = sharp.getAlbumList2("id", SubSharp.Album_List_Type.newest, 10, 0, fromTo, genre);
            Assert.IsTrue(sharp.ping().get_isOk, "Failed to get Album List 2");

        }
        // check if returns xml containing artists
        // test can total count depending on server
        [TestMethod]
        public void test_getArtists()
        {
            var result = sharp.getArtists("id");
            Assert.IsTrue(sharp.ping().get_isOk , "Failed to getArtists");
            Debug.WriteLine("Artist Count = " + result.Keys.Count);
             
            Assert.AreEqual(Total_Artist, result.Keys.Count);
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

            // lazy test print
            [TestMethod]
            public void test_Get_Album()
            {
                String xdoc = "<subsonic-response xmlns=\"http://subsonic.org/restapi\" status=\"ok\" version=\"1.8.0\">\r\n<album id=\"11053\" name=\"High Voltage\" coverArt=\"al-11053\" songCount=\"8\" created=\"2004-11-27T20:23:32\" duration=\"2414\" artist=\"AC/DC\" artistId=\"5432\">\r\n<song id=\"71463\" parent=\"71381\" title=\"The Jack\" album=\"High Voltage\" artist=\"AC/DC\" isDir=\"false\" coverArt=\"71381\" created=\"2004-11-08T23:36:11\" duration=\"352\" bitRate=\"128\" size=\"5624132\" suffix=\"mp3\" contentType=\"audio/mpeg\" isVideo=\"false\" path=\"ACDC/High voltage/ACDC - The Jack.mp3\" albumId=\"11053\" artistId=\"5432\" type=\"music\"/>\r\n<song id=\"71464\" parent=\"71381\" title=\"Tnt\" album=\"High Voltage\" artist=\"AC/DC\" isDir=\"false\" coverArt=\"71381\" created=\"2004-11-08T23:36:11\" duration=\"215\" bitRate=\"128\" size=\"3433798\" suffix=\"mp3\" contentType=\"audio/mpeg\" isVideo=\"false\" path=\"ACDC/High voltage/ACDC - TNT.mp3\" albumId=\"11053\" artistId=\"5432\" type=\"music\"/>\r\n<song id=\"71458\" parent=\"71381\" title=\"It's A Long Way To The Top\" album=\"High Voltage\" artist=\"AC/DC\" isDir=\"false\" coverArt=\"71381\" created=\"2004-11-27T20:23:32\" duration=\"315\" bitRate=\"128\" year=\"1976\" genre=\"Rock\" size=\"5037357\" suffix=\"mp3\" contentType=\"audio/mpeg\" isVideo=\"false\" path=\"ACDC/High voltage/ACDC - It's a long way to the top if you wanna rock 'n 'roll.mp3\" albumId=\"11053\" artistId=\"5432\" type=\"music\"/>\r\n<song id=\"71461\" parent=\"71381\" title=\"Rock 'n' Roll Singer.\" album=\"High Voltage\" artist=\"AC/DC\" isDir=\"false\" coverArt=\"71381\" created=\"2004-11-27T20:23:33\" duration=\"303\" bitRate=\"128\" track=\"2\" year=\"1976\" genre=\"Rock\" size=\"4861680\" suffix=\"mp3\" contentType=\"audio/mpeg\" isVideo=\"false\" path=\"ACDC/High voltage/ACDC - Rock N Roll Singer.mp3\" albumId=\"11053\" artistId=\"5432\" type=\"music\"/>\r\n<song id=\"71460\" parent=\"71381\" title=\"Live Wire\" album=\"High Voltage\" artist=\"AC/DC\" isDir=\"false\" coverArt=\"71381\" created=\"2004-11-27T20:23:33\" duration=\"349\" bitRate=\"128\" track=\"4\" year=\"1976\" genre=\"Rock\" size=\"5600206\" suffix=\"mp3\" contentType=\"audio/mpeg\" isVideo=\"false\" path=\"ACDC/High voltage/ACDC - Live Wire.mp3\" albumId=\"11053\" artistId=\"5432\" type=\"music\"/>\r\n<song id=\"71456\" parent=\"71381\" title=\"Can I sit next to you girl\" album=\"High Voltage\" artist=\"AC/DC\" isDir=\"false\" coverArt=\"71381\" created=\"2004-11-27T20:23:32\" duration=\"251\" bitRate=\"128\" track=\"6\" year=\"1976\" genre=\"Rock\" size=\"4028276\" suffix=\"mp3\" contentType=\"audio/mpeg\" isVideo=\"false\" path=\"ACDC/High voltage/ACDC - Can I Sit Next To You Girl.mp3\" albumId=\"11053\" artistId=\"5432\" type=\"music\"/>\r\n<song id=\"71459\" parent=\"71381\" title=\"Little Lover\" album=\"High Voltage\" artist=\"AC/DC\" isDir=\"false\" coverArt=\"71381\" created=\"2004-11-27T20:23:33\" duration=\"339\" bitRate=\"128\" track=\"7\" year=\"1976\" genre=\"Rock\" size=\"5435119\" suffix=\"mp3\" contentType=\"audio/mpeg\" isVideo=\"false\" path=\"ACDC/High voltage/ACDC - Little Lover.mp3\" albumId=\"11053\" artistId=\"5432\" type=\"music\"/>\r\n<song id=\"71462\" parent=\"71381\" title=\"She's Got Balls\" album=\"High Voltage\" artist=\"AC/DC\" isDir=\"false\" coverArt=\"71381\" created=\"2004-11-27T20:23:34\" duration=\"290\" bitRate=\"128\" track=\"8\" year=\"1976\" genre=\"Rock\" size=\"4651866\" suffix=\"mp3\" contentType=\"audio/mpeg\" isVideo=\"false\" path=\"ACDC/High voltage/ACDC - Shes Got Balls.mp3\" albumId=\"11053\" artistId=\"5432\" type=\"music\"/>\r\n</album>\r\n</subsonic-response>";

                var response = new SubResponse(xdoc);

                Debug.WriteLine(response.get_isOk);
                print(response.Result_To_Attribute_Dict("song", "id"));
                Assert.IsTrue(response.get_isOk);
            }
            /// <summary>
            /// Test get to get Albums without HTML Call
            /// </summary>
            [TestMethod]
            public void test_GetArtist_Response()
            {
                String albumResponse = "<subsonic-response xmlns=\"http://subsonic.org/restapi\" status=\"ok\" version=\"1.8.0\">\r\n<artist id=\"5432\" name=\"AC/DC\" coverArt=\"ar-5432\" albumCount=\"15\">\r\n<album id=\"11047\" name=\"Back In Black\" coverArt=\"al-11047\" songCount=\"10\" created=\"2004-11-08T23:33:11\" duration=\"2534\" artist=\"AC/DC\" artistId=\"5432\"/>\r\n</artist>\r\n</subsonic-response>";

                var response = new SubResponse(albumResponse);

                var test_dict = new Dictionary<String, Dictionary<String, String>>();


                

                var test_inner_dict = new Dictionary<String, String>();

                test_inner_dict.Add("id", "11047");
                test_inner_dict.Add("name" , "Back In Black");
                test_inner_dict.Add("coverArt", "al-11047");
                test_inner_dict.Add("songCount", "10");
                test_inner_dict.Add("created", "2004-11-08T23:33:11");
                test_inner_dict.Add("duration", "2534");
                test_inner_dict.Add("artist", "AC/DC");
                test_inner_dict.Add("artistId", "5432");

                test_dict.Add("11047", test_inner_dict);

                // test Dicts

                Assert.IsTrue(response.get_isOk);

                #region // for debugging
                Debug.WriteLine("printing dict");

                Debug.WriteLine("Printing Actual Dict\n");


                // Call To Get Dict
                var dict = response.Result_To_Attribute_Dict("album", "id");
                print(dict);


                Debug.WriteLine("\nPrinting Test Dic");

                print(test_dict);

                #endregion


                List<string> keyList;
                List<string> test_keyList;
                List<string> inner_dict_keys;
                List<string> inner_dict_values;
                List<string> test_inner_values;
                List<string> test_inner_keys;


                NestedDictionary_To_Lists(test_dict, dict, out keyList, out test_keyList, out inner_dict_keys, out inner_dict_values, out test_inner_values, out test_inner_keys);


                
                CollectionAssert.AreEqual(test_keyList, keyList, "KEYS NOT EQUAL");

                CollectionAssert.AreEqual(test_inner_keys, inner_dict_keys, "inner Dict keys not equal");
                CollectionAssert.AreEqual(test_inner_values, inner_dict_values, " inner value not equal");


                // keys are equal




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
                
                var test_dict = new Dictionary<String, Dictionary<String, String>>();


                var test_inner_dict = new Dictionary<String, String>();

                test_inner_dict.Add("id", "5449");
                test_inner_dict.Add("name", "A-Ha");
                test_inner_dict.Add("coverArt", "ar-5449");
                test_inner_dict.Add("albumCount", "4");


                test_dict.Add("5449", test_inner_dict);

                // test Dicts

                Assert.IsTrue(response.get_isOk);

                #region // for debugging
                Debug.WriteLine("printing dict");

                var dict = response.Result_To_Attribute_Dict("artist", "id");


                Debug.WriteLine("Printing Actual Dict\n");
                print(dict);

                Debug.WriteLine("\nPrinting Test Dic");
                print(test_dict);

                #endregion


                List<string> keyList;
                List<string> test_keyList;
                List<string> inner_dict_keys;
                List<string> inner_dict_values;
                List<string> test_inner_values;
                List<string> test_inner_keys;


                NestedDictionary_To_Lists(test_dict, dict, out keyList, out test_keyList, out inner_dict_keys, out inner_dict_values, out test_inner_values, out test_inner_keys);



                CollectionAssert.AreEqual(test_keyList, keyList, "KEYS NOT EQUAL");


                CollectionAssert.AreEqual(test_inner_keys, inner_dict_keys, "inner Dict keys not equal");
                CollectionAssert.AreEqual(test_inner_values, inner_dict_values, " inner value not equal");


                // keys are equal

            }

            private static void NestedDictionary_To_Lists(Dictionary<string, Dictionary<string, string>> test_dict, Dictionary<string, Dictionary<string, string>> dict, out List<string> keyList, out List<string> test_keyList, out List<string> inner_dict_keys, out List<string> inner_dict_values, out List<string> test_inner_values, out List<string> test_inner_keys)
            {


                keyList = new List<string>(dict.Keys);
                test_keyList = new List<string>(test_dict.Keys);



                // convert values to List of Dictionaries

                var valList = new List<Dictionary<string, string>>();





                inner_dict_keys = new List<string>();
                inner_dict_values = new List<string>();

                test_inner_values = new List<string>();
                test_inner_keys = new List<string>();


                foreach (KeyValuePair<string, Dictionary<string, string>> kvp in dict)
                {
                    foreach (var inner in kvp.Value)
                    {
                        inner_dict_keys.Add(inner.Key);
                        inner_dict_values.Add(inner.Value);
                    }
                }


                foreach (KeyValuePair<string, Dictionary<string, string>> kvp in dict)
                {
                    foreach (var inner in kvp.Value)
                    {
                        test_inner_keys.Add(inner.Key);
                        test_inner_values.Add(inner.Value);
                    }
                }

               
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

}