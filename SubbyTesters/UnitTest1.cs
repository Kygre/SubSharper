﻿using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using SubSharpLibrary.Client;
using SubSharpLibrary.Exceptions;
using System;
using System.Diagnostics;
using System.Collections.Generic;

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

        private static int Total_Artist = 326;

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

        // check if returns xml containing artists
        // test can total count depending on server
        
        [TestMethod]
        public void test_getArtists()
        {
            var result = sharp.getArtists("id");
            Assert.IsTrue(sharp.ping().get_isOk , "Failed to ping");
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
            public void test_Constructor_ArgumentException()
            {

                try
                {
                    var resp = new SubResponse(null);
                    Assert.Fail();
                }
                catch (ArgumentException)
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

            // tests get Dictionary withouth HTML call
            /// <summary>
            /// Iffy dictionary checker
            /// Tried using CollectionAssert for dictionaries but failed miserably
            /// Compares using lists insteads
            /// </summary>
            [TestMethod]
            public void test_Dictionary_Of_Atrribute_Elements()
            {
                String xdoc = "<subsonic-response xmlns=\"http://subsonic.org/restapi\" status=\"ok\" version=\"1.10.2\"> </subsonic-response>";

                String artist = "<subsonic-response xmlns=\"http://subsonic.org/restapi\" status=\"ok\" version=\"1.10.1\">\r\n<artists ignoredArticles=\"The El La Los Las Le Les\">\r\n<index name=\"A\">\r\n<artist id=\"5449\" name=\"A-Ha\" coverArt=\"ar-5449\" albumCount=\"4\"/>\r\n</index>\r\n</artists>\r\n</subsonic-response>";


                var response = new SubResponse(artist);

                var test_dict = new Dictionary<String, Dictionary<String, String>>();


                var test_inner_dict = new Dictionary<String, String>();

                test_inner_dict.Add("id", "5449");
                test_inner_dict.Add("name", "A-Ha");
                test_inner_dict.Add("coverArt", "ar-5449");
                test_inner_dict.Add("albumCount", "4");


                test_dict.Add("5449", test_inner_dict);

                Assert.IsTrue(response.get_isOk);

                //Assert.AreEqual(test_dict, response.Result_To_Attribute_Dict("artists", "id"));


                Debug.WriteLine("printing dict");

                var dict = response.Result_To_Attribute_Dict("artist", "id");
                Debug.WriteLine("Printing Actual Dict\n");
                print(dict);
                Debug.WriteLine("\nPrinting Test Dic");
                print(test_dict);



                List<string> keyList = new List<string>(dict.Keys);
                List<string> test_keyList = new List<string>(test_dict.Keys);



                // convert values to List of Dictionaries

                var valList = new List<Dictionary<string, string>>();
                var inner_dict_keys = new List<string>();
                var inner_dict_values = new List<string>();
                var test_inner_values = new List<string>();
                var test_inner_keys = new List<string>();


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


                CollectionAssert.AreEqual(test_keyList, keyList, "KEYS NOT EQUAL");


                CollectionAssert.AreEqual(test_inner_keys, inner_dict_keys, "inner Dict keys not equal");
                CollectionAssert.AreEqual(test_inner_values, inner_dict_values, " inner value not equal");


                // keys are equal

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
            public void test_UserNotAuthorizedErro()
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