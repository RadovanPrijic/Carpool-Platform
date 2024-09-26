import {
  Location,
  Ride,
  RideCreateDTO,
  RideUpdateDTO,
} from "../features/rides/types";
import { API_ROUTES } from "../utils/api-config";
import { getAuthToken } from "../utils/auth";
import { httpRequest } from "../utils/http";

interface GetFilteredRidesArgs {
  from: string;
  to: string;
  date: string;
  seats: number;
}

export async function getFilteredRides({
  from,
  to,
  date,
  seats,
}: GetFilteredRidesArgs): Promise<Ride[]> {
  return httpRequest<Ride[]>(
    `${API_ROUTES.RIDES}/?from=${encodeURIComponent(
      from
    )}&to=${encodeURIComponent(to)}&date=${encodeURIComponent(
      date
    )}&seats=${seats}`,
    {
      method: "GET",
      headers: {
        "Content-Type": "application/json",
      },
    }
  );
}

export async function getAllRidesForUser(id: string): Promise<Ride[]> {
  return httpRequest<Ride[]>(`${API_ROUTES.RIDES}/all/${id}`, {
    method: "GET",
    headers: {
      Authorization: `Bearer ${getAuthToken()}`,
      "Content-Type": "application/json",
    },
  });
}

export async function getRideById(id: number): Promise<Ride> {
  return httpRequest<Ride>(`${API_ROUTES.RIDES}/${id}`, {
    method: "GET",
    headers: {
      Authorization: `Bearer ${getAuthToken()}`,
      "Content-Type": "application/json",
    },
  });
}

export async function createRide(rideCreateDTO: RideCreateDTO): Promise<Ride> {
  return httpRequest<Ride>(`${API_ROUTES.RIDES}/`, {
    method: "POST",
    headers: {
      Authorization: `Bearer ${getAuthToken()}`,
      "Content-Type": "application/json",
    },
    body: JSON.stringify(rideCreateDTO),
  });
}

interface UpdateRideArgs {
  id: number;
  rideUpdateDTO: RideUpdateDTO;
}

export async function updateRide({
  id,
  rideUpdateDTO,
}: UpdateRideArgs): Promise<Ride> {
  return httpRequest<Ride>(`${API_ROUTES.RIDES}/${id}`, {
    method: "PUT",
    headers: {
      Authorization: `Bearer ${getAuthToken()}`,
      "Content-Type": "application/json",
    },
    body: JSON.stringify(rideUpdateDTO),
  });
}

export async function deleteRide(id: number): Promise<string> {
  return httpRequest<string>(`${API_ROUTES.RIDES}/${id}`, {
    method: "DELETE",
    headers: {
      Authorization: `Bearer ${getAuthToken()}`,
    },
  });
}

export async function getLocations(): Promise<Location[]> {
  return httpRequest<Location[]>(`${API_ROUTES.RIDES}/locations`, {
    method: "GET",
    headers: {
      "Content-Type": "application/json",
    },
  });
}
