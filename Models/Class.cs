using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cumulative1.Models
{
    /// <summary>
    /// Represents a class in the school system.
    /// </summary>
    public class Class
    {
        // Properties of the Class entity
        public int ClassId;
        public string ClassCode;
        public int TeacherId;
        public DateTime StartDate;
        public DateTime FinishDate;
        public string ClassName;
    }
}