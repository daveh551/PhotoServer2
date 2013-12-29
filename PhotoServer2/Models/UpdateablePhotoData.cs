using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PhotoServer2.Models
{
    public class UpdateablePhotoData
    {
        public virtual string Race { get; set; }
        public virtual string Station { get; set; }
        public virtual string Card { get; set; }
        public virtual int? Sequence { get; set; }
        public virtual string PhotographerInitials { get; set; }
    }
}