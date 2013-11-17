﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using PhotoServer.Domain;

namespace PhotoServer2.Models
{
    [Obsolete("Used for scaffolding new controllers only")]
    public class PhotoDbContext : DbContext
    {
        private DbSet<Race> _races;
        private DbSet<Event> _events;
        private DbSet<Photo> _photos;
        private DbSet<Distance> _distances;

        public PhotoDbContext() : base("DefaultConnection")
        {
            
        }

        public System.Data.Entity.DbSet<PhotoServer.Domain.Photo> Photos { get; set; }

        public System.Data.Entity.DbSet<PhotoServer.Domain.Race> Races { get; set; }
    }
}