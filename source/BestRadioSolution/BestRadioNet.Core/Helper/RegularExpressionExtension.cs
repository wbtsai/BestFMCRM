using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace BestRadioNet.Core.Helper
{
    public static class RegularExpressionExtension
    {
        public static bool IsMatchEmail(this string email)
        {
            //Email Pattern
            string patternStrict = @"^(([^<>()[\]\\.,;:\s@\""]+(\.[^<>()[\]\\.,;:\s@\""]+)*)|(\"".+\""))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$";
            
            Regex reStrict = new Regex(patternStrict);

            return reStrict.IsMatch(email);
        }
    }
}
