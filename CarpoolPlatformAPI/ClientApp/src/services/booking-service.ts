import {
  Booking,
  BookingCreateDTO,
  BookingUpdateDTO,
} from "../features/bookings/types";
import { API_ROUTES } from "../utils/api-config";
import { getAuthToken } from "../utils/auth";
import { httpRequest } from "../utils/http";

export async function getFilteredBookings(
  id: string,
  filter: string
): Promise<Booking[]> {
  return httpRequest<Booking[]>(
    `${API_ROUTES.BOOKINGS}/filtered/${id}?filter=${filter}`,
    {
      method: "GET",
      headers: {
        Authorization: `Bearer ${getAuthToken()}`,
        "Content-Type": "application/json",
      },
    }
  );
}

export async function getAllBookingsForRide(id: number): Promise<Booking[]> {
  return httpRequest<Booking[]>(`${API_ROUTES.BOOKINGS}/for-ride/${id}`, {
    method: "GET",
    headers: {
      Authorization: `Bearer ${getAuthToken()}`,
      "Content-Type": "application/json",
    },
  });
}

export async function getBookingById(id: number): Promise<Booking> {
  return httpRequest<Booking>(`${API_ROUTES.BOOKINGS}/${id}`, {
    method: "GET",
    headers: {
      Authorization: `Bearer ${getAuthToken()}`,
      "Content-Type": "application/json",
    },
  });
}

export async function createBooking(
  bookingCreateDTO: BookingCreateDTO
): Promise<Booking> {
  return httpRequest<Booking>(`${API_ROUTES.BOOKINGS}/`, {
    method: "POST",
    headers: {
      Authorization: `Bearer ${getAuthToken()}`,
      "Content-Type": "application/json",
    },
    body: JSON.stringify(bookingCreateDTO),
  });
}

interface UpdateBookingArgs {
  id: number;
  bookingUpdateDTO: BookingUpdateDTO;
}

export async function updateBooking({
  id,
  bookingUpdateDTO,
}: UpdateBookingArgs): Promise<Booking> {
  return httpRequest<Booking>(`${API_ROUTES.BOOKINGS}/${id}`, {
    method: "PUT",
    headers: {
      Authorization: `Bearer ${getAuthToken()}`,
      "Content-Type": "application/json",
    },
    body: JSON.stringify(bookingUpdateDTO),
  });
}

export async function cancelBooking(id: number): Promise<Booking> {
  return httpRequest<Booking>(`${API_ROUTES.BOOKINGS}/${id}`, {
    method: "DELETE",
    headers: {
      Authorization: `Bearer ${getAuthToken()}`,
      "Content-Type": "application/json",
    },
  });
}
