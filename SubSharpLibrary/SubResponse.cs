using SubSharpLibrary.Exceptions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using Windows.UI.Xaml.Media.Imaging;

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
        
        private bool ok = false;
        private XDocument Xresult;
        private const String restApi = "{http://subsonic.org/restapi}subsonic-response";
        

        // for Querys that do not return data
        public SubResponse(string result)
        {
            // TODO: Complete member initialization
            if (String.IsNullOrEmpty(result))
            {
                throw new SubSharpException("Subsonic response text is empty!" , new ArgumentException());
            }

            this.Xresult = XDocument.Parse(result);

            this.ok = isOk(Xresult);
            Debug.WriteLine(Xresult.Document.ToString());
            if (!ok)
            {

                err_Msg();

            }
                        
        }

        // for saving BitmapImage
        public SubResponse(BitmapImage url)
        {

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
        public LinkedList<Dictionary<String, String>> Result_To_Attribute_Dict(String XmlElementTarget)
        {

            var list = new LinkedList<Dictionary<String, String>>();

            XmlElementTarget = "{http://subsonic.org/restapi}" + XmlElementTarget;

            var descendants = Xresult.Document.Descendants(XmlElementTarget);

            if (descendants.Count() > 0)
            {
                foreach (XElement xe in descendants)
                {

                    //Debug.WriteLine("Found Element - " + xe.Name);
                    if (xe.HasAttributes)
                    {
                        var temp_dict = new Dictionary<String, String>();


                        foreach (XAttribute attribtue in xe.Attributes())
                        {
                            temp_dict.Add(attribtue.Name.ToString(), attribtue.Value.ToString());
                        }


                        list.AddLast(temp_dict);
                        

                    }
                    else
                    {
                        // Get any elements that have text

                        
                    }
                }
            }
                /*
            else
            {
                throw new SubSharpException("No Attribute Name for keyTag found in " + XmlElementTarget);
            }
            
            */
            
            if (list.Count == 0)
            {
                Debug.WriteLine("No Keys found for dict using XElementTarget = " + XmlElementTarget);
                
            }
             
            

            return list;
        }

        // FIXME /// Values of elements may contain newlines or return carrage
        /// <summary>
        /// Return an array of length 2
        /// [0] = Dictionary of Element Name and Text
        /// [1] = Linked List of Dictionary of Attribute Name and Attribute Value -- for each Similiar Song
        /// 
        /// If null, indicates is empty
        /// </summary>
        /// <param name="Xresult"></param>
        /// <param name="XmlElementTarget"></param>
        /// <param name="attrKey"></param>
        /// <returns>out values with data or null</returns>
        public ICollection[] Results_To_Dicts()
        {

            
            var first = this.Xresult.Document.Root;

            var elemtne = first.Elements();


            var elements = new Dictionary<string, string>();
            var attributes = new LinkedList<Dictionary<string, string>>();


            foreach (var xelem in elemtne.Elements())
            {

                Debug.WriteLine("Getting Element - " + xelem.Name.ToString());

                if (xelem.HasAttributes)
                {
                    // add attribute values
                    var attrs = new Dictionary<string, string>();

                    foreach (XAttribute attribute in xelem.Attributes())
                    {
                        //Debug.WriteLine("Got Attribute - " + attribute.Name.ToString());
                        attrs.Add(attribute.Name.ToString(), attribute.Value.ToString());
                    }
                    attributes.AddLast(attrs);
                }
                else
                {
                    elements.Add(xelem.Name.LocalName.ToString(), xelem.Value.ToString());
                }

            }



            if (elements.Count == 0)
            {
                Debug.WriteLine("Elements is empty");
                
            }
            else
            {
                Debug.WriteLine("Printing Dictionary");

                foreach(KeyValuePair<String,String> kvp in elements){
                    Debug.WriteLine(kvp.Key + " - " + kvp.Value.ToString());
                }
            }


            if (attributes.Count == 0)
            {
                Debug.WriteLine("Attributes is empty - ");
                
            }
            else
            {
                Debug.WriteLine("Printing Linked List Dicationaries");
                foreach (var v in attributes)
                {
                    foreach (var kvp in v)
                    {
                        Debug.WriteLine(kvp.Key.ToString() + " - " + kvp.Value.ToString());
                    }
                }

            }

            ICollection[] collect = { elements, attributes };
            return collect;
        }

        /// <summary>
        /// Three dictionaries that can be emtpy
        /// For seperating getStarred into dictionaries of their types
        /// [0] = Artists :: [1] = Albums :: [2] = Songs
        /// </summary>
        /// <returns></returns>
        public LinkedList<Dictionary<String, String>>[] result_get_all_Descendants( )
        {

            var first = this.Xresult.Document.Root;

            var elemtne = first.Elements();

            LinkedList<Dictionary<String, String>>[] dicts = { new LinkedList<Dictionary<String, String>>(), new LinkedList<Dictionary<String, String>>(), new LinkedList<Dictionary<String, String>>() };

            int dict_index = 0;
            foreach (var xelem in elemtne.Elements())
            {

                Debug.WriteLine("Getting Element - " + xelem.Name.ToString());

                if (xelem.Name.ToString().Equals("artist"))
                {
                    dict_index = 0;
                }
                if (xelem.Name.ToString().Equals("album"))
                {
                    dict_index = 1;
                }
                if (xelem.Name.ToString().Equals("song"))
                {
                    dict_index = 2;
                }


                if (xelem.HasAttributes)
                {
                    // add attribute values
                    var attrs = new Dictionary<string, string>();

                    foreach (XAttribute attribute in xelem.Attributes())
                    {
                        //Debug.WriteLine("Got Attribute - " + attribute.Name.ToString());
                        attrs.Add(attribute.Name.ToString(), attribute.Value.ToString());
                    }

                   dicts[dict_index].AddLast(attrs);
                }
               

            }

            return dicts;
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
