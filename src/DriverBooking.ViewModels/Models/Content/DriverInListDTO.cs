using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DriverBooking.Core.Domain.Entities;
using DriverBooking.Core.Domain.Identity;
using NetTopologySuite.Geometries;

namespace DriverBooking.Core.Models.Content
{
    public class DriverInListDTO
    {
        public int Id { get; set; }
        public required string PhoneNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public string? ProfileAvatarUrl { get; set; }
        public int TripCount { get; set; }
        public int RatingCount { get; set; }
        public Point? CurrentLocation { get; set; }
        public Vehicle Vehicle { get; set; }

        public class AutoMapperProfiles : Profile {
            
            public AutoMapperProfiles() {
                CreateMap<Driver, DriverInListDTO>()
                    .ForMember(dest => dest.Vehicle, opt => opt.MapFrom(src => src.Vehicle))
                    .ForMember(dest => dest.CurrentLocation, opt => opt.MapFrom(src => src.CurrentLocation));
            }

        }
    }
}
