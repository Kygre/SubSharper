using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;
using System.Text;
using SubSharpLibrary.Exceptions;

namespace SubSharpLibrary.Client
{
    /// <summary>
    /// A class wrapper for subsonic response xml
    /// Returns any data recieved in Dictionary of SubItems
    /// 
    /// Example -- Result
    /// <subsonic-response xmlns="http://subsonic.org/restapi" status="failed" version="1.10.2">
    ///       <error code="10" message="Required parameter is missing."/>
    /// </subsonic-response>
    /// 
    /// 
    /// </summary>
    public class SubResponse
    {
        private string result;
        private bool ok;
        private XDocument Xresult;


        public SubResponse(string result)
        {
            // TODO: Complete member initialization
            if (String.IsNullOrEmpty(result))
            {
                throw new ArgumentException("Subsonic response text is empty!");
            }


            this.Xresult = XDocument.Parse(result);

            this.ok = isOk(Xresult);

            if (!ok)
            {
                var error = this.Xresult.Document.Descendants("{http://subsonic.org/restapi}error").Attributes();


                String codeString = error.ElementAt(0).ToString();
                String message = error.ElementAt(1).ToString();


                if (String.IsNullOrEmpty(codeString) || String.IsNullOrEmpty(message))
                {
                    throw new SubSharpException("< Subsonic response attributes for Code or Message are empty! >", error.ElementAt(0), error.ElementAt(1));
                }

                String exceptionMsg = "Subsonic returned error with " + codeString + "\n" + message;

                throw new SubSharpException(exceptionMsg, error.ElementAt(0), error.ElementAt(1));

            }
        }

        /// <summary>
        /// 
        /// Checks from all descendants if the subsonic response has returned ok
        /// With no error codes
        /// </summary>
        /// <param name="descendants" > </param>
        /// <returns>boolean</returns>
        public static Boolean isOk(XDocument x)
        {
            // String xXmlSubRoot = "{http://subsonic.org/restapi}subsonic-response";

            return x.Root.Attributes("status").FirstOrDefault().Value.Equals("ok");


        }

        public bool get_isOk
        {
            get
            {
                return ok;
            }
        }
    }
}
