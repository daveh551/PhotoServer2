using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;

namespace PhotoServer2.App_Start
{
    public static class InitializeMapper
    {
        public static void MapClasses()
        {
            Mapper.CreateMap<PhotoServer.Domain.Photo, PhotoServer2.Models.PhotoData>()
                ;
            Mapper.CreateMap<PhotoServer2.Models.PhotoData, PhotoServer.Domain.Photo>()
                // Members of Photo that are not reflected in PhotoData
                .ForMember(x => x.RaceId, y => y.Ignore())
                .ForMember(x => x.Race, y => y.Ignore())
                .ForMember(x => x.Path, y => y.Ignore())
                .ForMember(x => x.BasedOn, y => y.Ignore())
                .ForMember(x => x.FileSize, y => y.Ignore())
                .ForMember(x => x.LastAccessed, y => y.Ignore())
                .ForMember(x => x.Server, y => y.Ignore())
                ;
            Mapper.CreateMap<PhotoServer2.Models.UpdateablePhotoData, PhotoServer2.Models.PhotoData>()
                .ForMember(x => x.Id, y => y.Ignore())
                .ForMember(x => x.TimeStamp, y => y.Ignore())
                .ForMember(x => x.Hres, y => y.Ignore())
                .ForMember(x => x.Vres, y => y.Ignore())
                .ForMember(x => x.FStop, y=> y.Ignore())
                .ForMember(x => x.FocalLength, y => y.Ignore())
                .ForMember(x => x.ShutterSpeed, y => y.Ignore())
                .ForMember(x => x.ISOSpeed, y => y.Ignore())
                .ForMember(x => x.CreatedBy, y => y.Ignore())
                .ForMember(x => x.CreatedDate, y => y.Ignore())
                ;
            Mapper.CreateMap<PhotoServer2.Models.PhotoData, Models.UpdateablePhotoData>()
                ;
            Mapper.AssertConfigurationIsValid();
        }
    }
}