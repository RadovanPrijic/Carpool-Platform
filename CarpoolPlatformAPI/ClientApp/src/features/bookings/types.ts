import { Review } from "../reviews/types";
import { Ride } from "../rides/types";
import { User } from "../users/types";

export interface Booking {
  id: number;
  bookingStatus: string;
  seatsBooked: number;
  totalPrice: number;
  createdAt: string;
  updatedAt?: string;
  user: User;
  ride: Ride;
  review?: Review;
}

export interface BookingCreateDTO {
  seatsBooked: number;
  totalPrice: number;
  userId: string;
  rideId: number;
}

export interface BookingUpdateDTO {
  bookingStatus: string;
}
