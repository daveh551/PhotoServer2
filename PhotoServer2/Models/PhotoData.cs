using System;

namespace PhotoServer2.Models
{
    // This class represents the members of the PhotoData Domain model that are transferred back to the user
    public class PhotoData
    {
        public Guid Id { get; set; }
        public virtual string Race { get; set; }
        public virtual string Station { get; set; }
        public virtual string Card { get; set; }
        public virtual int? Sequence { get; set; }
        public virtual DateTime? TimeStamp { get; set; }
        public virtual int? Hres { get; set; }
        public virtual int? Vres { get; set; }
        public virtual string FStop { get; set; }
        public virtual string ShutterSpeed { get; set; }
        public virtual short? ISOSpeed { get; set; }
        public virtual short? FocalLength { get; set; }
        public virtual string PhotographerInitials { get; set; }
        public virtual string CreatedBy { get; set; }
        public virtual DateTime CreatedDate { get; set; }
    }
}