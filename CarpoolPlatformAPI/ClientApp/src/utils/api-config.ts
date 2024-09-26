import { QueryClient } from "@tanstack/react-query";

export const BASE_URL = "https://localhost:7153/api";

export const API_ROUTES = {
  USERS: `${BASE_URL}/Users`,
  RIDES: `${BASE_URL}/Rides`,
  BOOKINGS: `${BASE_URL}/Bookings`,
  REVIEWS: `${BASE_URL}/Reviews`,
  MESSAGES: `${BASE_URL}/Messages`,
};

export const queryClient = new QueryClient();
