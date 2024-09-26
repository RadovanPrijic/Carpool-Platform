import { useQuery } from "@tanstack/react-query";
import { useState } from "react";
import { useAppSelector } from "../../../hooks/store-hooks";
import { getFilteredBookings } from "../../../services/booking-service";
import BookingsFilter from "../components/BookingsFilter";
import BookingComponent from "../components/Booking";
import { Booking } from "../types";

const BookingsPage = () => {
  const [filter, setFilter] = useState<string>("by-user-requested");
  const userId = useAppSelector((state) => state.auth.userId);

  const {
    data: bookings,
    isLoading,
    error,
  } = useQuery({
    queryKey: ["bookings", userId, filter],
    queryFn: () => getFilteredBookings(userId, filter),
  });

  const handleFilterChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setFilter(event.target.value);
  };

  let content;
  if (isLoading) {
    content = <p>Loading ...</p>;
  } else if (error) {
    content = <p>Error!</p>;
  } else if (bookings) {
    content = (
      <ul>
        {bookings.map((booking: Booking) => (
          <BookingComponent
            key={booking.id}
            booking={booking}
            filter={filter}
          />
        ))}
      </ul>
    );
  }

  return (
    <div>
      <header>
        <h2>Bookings</h2>
      </header>
      <BookingsFilter filter={filter} onChange={handleFilterChange} />
      {content}
    </div>
  );
};

export default BookingsPage;
