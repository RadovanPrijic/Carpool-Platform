using System.ComponentModel.DataAnnotations;

namespace CarpoolPlatformAPI.Models.DTO.Ride
{
    public class RideCreateDTO
    {
        [Required(ErrorMessage = "You have not chosen a starting location.")]
        public string StartLocation { get; set; }

        [Required(ErrorMessage = "You have not chosen a destination.")]
        public string EndLocation { get; set; }

        [Required(ErrorMessage = "You have not entered the time of departure.")]
        [DataType(DataType.DateTime, ErrorMessage = "The entered departure time is not a DateTime type.")]
        public DateTime DepartureTime { get; set; }

        [Required(ErrorMessage = "You have not entered a price per seat.")]
        public double PricePerSeat { get; set; }

        [StringLength(500, ErrorMessage = "The car information should be up to 500 characters long.")]
        public string? RideDescription { get; set; }

        [Required(ErrorMessage = "You have not entered any information about the car.")]
        [StringLength(250, ErrorMessage = "The car information should be up to 250 characters long.")]
        public string CarInfo { get; set; }

        [Required(ErrorMessage = "You have not chosen the number of available seats.")]
        [Range(1, 4, ErrorMessage = "The number of available seats must be between 1 and 4.")]
        public int SeatsAvailable { get; set; }

        [Required(ErrorMessage = "You have not chosen if there can be only two people in the backseat.")]
        public bool TwoInBackseat { get; set; }

        [Required(ErrorMessage = "You have not entered any information about the size of your luggage space.")]
        public string LuggageSize { get; set; }

        [Required(ErrorMessage = "You have not chosen the insurance status.")]
        public bool InsuranceStatus { get; set; }

        [Required(ErrorMessage = "You have not chosen if bookings should be automatic or not.")]
        public bool AutomaticBooking { get; set; }

        [Required(ErrorMessage = "You have not provided a user ID with your ride.")]
        public string UserId { get; set; }
    }
}
