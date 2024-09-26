import { User } from "../users/types";
import { Ride } from "../rides/types";

export interface Review {
  id: number;
  rating: number;
  comment: string;
  createdAt: string;
  updatedAt?: string;
  reviewer: User;
  revieweeId: string;
  ride: Ride;
}

export interface ReviewCreateDTO {
  rating: number;
  comment: string;
  reviewerId: string;
  revieweeId: string;
  rideId: number;
  bookingId: number;
}

export interface ReviewUpdateDTO {
  rating: number;
  comment: string;
}
