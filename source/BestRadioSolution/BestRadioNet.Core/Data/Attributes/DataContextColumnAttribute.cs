using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BestRadioNet.Core.Data.Attributes
{
    /// <summary>
    /// DataContext Column Attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class DataContextColumnAttribute:Attribute
    {
        /// <summary>
        /// Map Key
        /// </summary>
        public string MapKey { get; set; }

        /// <summary>
        /// Column Name
        /// </summary>
        public string ColumnName { get; set; }

        /// <summary>
        /// Property Name
        /// </summary>
        public string ProperyName { get; set; }

        /// <summary>
        /// Property Type
        /// </summary>
        public Type PropertyType { get; set; }
    }
}
