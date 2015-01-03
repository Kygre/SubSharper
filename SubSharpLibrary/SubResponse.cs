using SubSharpLibrary.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;

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
        private Dictionary<String, Dictionary<String, String>> data;
        private const String restApi = "{http://subsonic.org/restapi}subsonic-response";

        // for Querys that do not return data
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

                err_Msg();

            }
                        
        }


        /// <summary>
        /// Creates Error Message and throw SubSharpException
        /// </summary>
        private void err_Msg()
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


        /// TODO assert AttrKey is defined in xml element
        /// <summary>
        /// A dictionary of key being the attrKey value and a dictionary of attributes < Name, Value >
        /// Strictly for Artists, Albums, and Songs
        /// </summary>
        /// <param name="XmlElementTarget"></param>
        /// <returns></returns>
        public Dictionary<String, Dictionary<String, String>> Result_To_Attribute_Dict(String XmlElementTarget, String attrKey)
        {

            var dict = new Dictionary<String, Dictionary<String, String>>();

            XmlElementTarget = "{http://subsonic.org/restapi}" + XmlElementTarget;


            foreach (XElement xe in Xresult.Document.Descendants(XmlElementTarget))
            {
                
                //Debug.WriteLine("Found Element - " + xe.Name);
                if (xe.HasAttributes)
                {
                    var temp_dict = new Dictionary<String, String>();


                    foreach (XAttribute attribtue in xe.Attributes())
                    {
                        temp_dict.Add(attribtue.Name.ToString(), attribtue.Value.ToString());
                    }

                    String attr;
                    temp_dict.TryGetValue(attrKey, out attr);

                    dict.Add(attr, temp_dict);

                }
                else
                {
                    // throw SubShar


                }
            }


            if (dict.Keys.Count == 0)
            {
                Debug.WriteLine("No Keys found for dict using XElementTarget = " + XmlElementTarget);
            }
            return dict;
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
