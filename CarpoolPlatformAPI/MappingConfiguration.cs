using AutoMapper;
using CarpoolPlatformAPI.Models.Domain;
using CarpoolPlatformAPI.Models.DTO.Auth;
using CarpoolPlatformAPI.Models.DTO.Booking;
using CarpoolPlatformAPI.Models.DTO.Location;
using CarpoolPlatformAPI.Models.DTO.Message;
using CarpoolPlatformAPI.Models.DTO.Notification;
using CarpoolPlatformAPI.Models.DTO.Picture;
using CarpoolPlatformAPI.Models.DTO.Review;
using CarpoolPlatformAPI.Models.DTO.Ride;
using CarpoolPlatformAPI.Models.DTO.User;

namespace CarpoolPlatformAPI
{
    public class MappingConfiguration : Profile
    {
        public MappingConfiguration() 
        {
            CreateMap<User, UserDTO>().ReverseMap();
            CreateMap<User, RegistrationRequestDTO>().ReverseMap();
            CreateMap<User, UserUpdateDTO>().ReverseMap();

            CreateMap<Ride, RideDTO>().ReverseMap();
            CreateMap<Ride, RideCreateDTO>().ReverseMap();
            CreateMap<Ride, RideUpdateDTO>().ReverseMap();

            CreateMap<Booking, BookingDTO>().ReverseMap();
            CreateMap<Booking, BookingCreateDTO>().ReverseMap();
            CreateMap<Booking, BookingUpdateDTO>().ReverseMap();

            CreateMap<Review, ReviewDTO>().ReverseMap();
            CreateMap<Review, ReviewCreateDTO>().ReverseMap();
            CreateMap<Review, ReviewUpdateDTO>().ReverseMap();

            CreateMap<Message, MessageDTO>().ReverseMap();
            CreateMap<Message, MessageCreateDTO>().ReverseMap();
            CreateMap<Message, MessageUpdateDTO>().ReverseMap();

            CreateMap<Picture, PictureDTO>().ReverseMap();
            CreateMap<Picture, PictureCreateDTO>().ReverseMap();

            CreateMap<Notification, NotificationDTO>().ReverseMap();

            CreateMap<Location, LocationDTO>().ReverseMap();
        }
    }
}
