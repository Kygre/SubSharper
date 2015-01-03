using System;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SubSharpLibrary.Exceptions
{



    public class SubSharpException : Exception
    {

        private ErrorCode code;
        private String message;

        public ErrorCode get_SubsonicErrorCode
        {

            get { return code; }
        }

        public enum ErrorCode
        {
            GENERIC = 0,
            PARAMETERS_MISSING = 10,
            REST_CLIENT_INCOMPATILE = 20,
            REST_SERVER_INCOMPATIBLE = 30,
            WRONG_USER_OR_PASS = 40,
            USER_NOT_AUTHORIZED = 50,
            SERVER_NO_PREMIUM = 60,
            DATA_NOT_FOUND = 70

        }
        public SubSharpException()
        {

        }

        public SubSharpException(String message, XAttribute attrcode, XAttribute attrmsg)
            : base(message)
        {



            if (!Enum.TryParse(attrcode.Value.ToString(), out code))
            {

                throw new Exception("Unable to Parse Error Code in SubSharpException", this);
            }




        }

        public SubSharpException(string message, Exception inner)
            : base(message, inner)
        {


        }





    }






}


