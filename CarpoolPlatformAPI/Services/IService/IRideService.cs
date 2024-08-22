﻿using CarpoolPlatformAPI.Models.Domain;
using CarpoolPlatformAPI.Models.DTO.Location;
using CarpoolPlatformAPI.Models.DTO.Ride;
using System.Linq.Expressions;

namespace CarpoolPlatformAPI.Services.IService
{
    public interface IRideService
    {
        Task<List<RideDTO>> GetAllRidesAsync(Expression<Func<Ride, bool>>? filter = null, string? includeProperties = null,
            int pageSize = 0, int pageNumber = 1, bool? notTracked = null);
        Task<RideDTO?> GetRideAsync(Expression<Func<Ride, bool>>? filter = null, string? includeProperties = null,
            bool? notTracked = null);
        Task<RideDTO> CreateRideAsync(RideCreateDTO rideCreateDTO);
        Task<RideDTO?> UpdateRideAsync(int id, RideUpdateDTO rideUpdateDTO);
        Task<RideDTO?> RemoveRideAsync(int id);
        Task ImportLocationsFromExcelAsync(string filePath);
        Task<List<LocationDTO>> GetAllLocationsAsync(Expression<Func<Location, bool>>? filter = null);
    }
}
