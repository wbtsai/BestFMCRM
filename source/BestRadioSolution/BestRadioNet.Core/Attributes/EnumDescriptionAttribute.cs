using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace BestRadioNet.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class EnumDescriptionAttribute:Attribute
    {
        private string _Description;

        public EnumDescriptionAttribute(string description)
        {
            _Description = description;
        }

        public string Description
        {
            get { return _Description; }
        }
    }
}
