import { LoaderFunctionArgs } from "react-router";
import { Ride } from "./types";
import { Booking } from "../bookings/types";
import { queryClient } from "../../utils/api-config";
import { getRideById } from "../../services/ride-service";
import { getAllBookingsForRide } from "../../services/booking-service";

export async function loader({ params }: LoaderFunctionArgs) {
  if (!params.id) {
    throw new Error("Ride ID parameter is required.");
  }
  const [ride, bookings] = await Promise.all([
    queryClient.fetchQuery<Ride>({
      queryKey: ["ride", params.id],
      queryFn: () => getRideById(parseInt(params.id!)),
    }),
    queryClient.fetchQuery<Booking[]>({
      queryKey: ["ride-bookings", params.id],
      queryFn: () => getAllBookingsForRide(parseInt(params.id!)),
    }),
  ]);
  return { ride, bookings };
}
