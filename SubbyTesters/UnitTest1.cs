using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using SubSharpLibrary.Client;
using SubSharpLibrary.Exceptions;
using System;
using System.Diagnostics;

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
         * 
         * 
         * For Before Build use -- 
         *  string[] args = { "kj" , "soccerg23", "kygre.subsonic.org", "", "1.10", "subClient"};

            var check = new SubSharp(args, false);
         * Testing Exception use
         *  [ExpectedException(typeof(ArgumentException)[, string msg])]
            */

        private static SubSharp sharp;
        private static SubSharp sharps; // for testing https possibly

        // INIT Subsonic Connections
        [ClassInitialize()]
        public static void setUp(TestContext tx)
        {
            string[] args = { "kj", "soccerg23", "kygre.subsonic.org", "", "1.10", "subClient" };
            sharp = new SubSharp(args, false);
            var check = new SubSharp(args, false);
            Debug.WriteLine("Starting Tests");


        }


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

            } // fail for other exception
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


        // TODO Test Error Exceptions

        [TestMethod]
        public void test_SubharpException_Thrown()
        {

        }

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
    }
}
