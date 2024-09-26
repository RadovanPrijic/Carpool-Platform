import { useLoaderData, useNavigate, useParams } from "react-router";
import { deleteRide } from "../../../services/ride-service";
import { useMutation } from "@tanstack/react-query";
import { useAppDispatch, useAppSelector } from "../../../hooks/store-hooks";
import { queryClient } from "../../../utils/api-config";
import { createBooking } from "../../../services/booking-service";
import { useRef, useState } from "react";
import Modal, { ModalHandle } from "../../../components/Modal";
import { Ride } from "../types";
import { Booking } from "../../bookings/types";
import Button from "../../../components/Button";
import Input from "../../../components/Input";
import { errorActions } from "../../../store/error-slice";

const SingleRidePage = () => {
  const isAuthenticated = useAppSelector((state) => state.auth.isAuthenticated);
  const userId = useAppSelector((state) => state.auth.userId);
  const { ride, bookings } = useLoaderData() as {
    ride: Ride;
    bookings: Booking[];
  };
  const isRideCreator: boolean = userId === ride.user.id;
  const [numberOfSeats, setNumberOfSeats] = useState<number>(1);
  const bookingmodalRef = useRef<ModalHandle>(null);
  const deleteModalRef = useRef<ModalHandle>(null);
  const params = useParams();
  const navigate = useNavigate();
  const dispatch = useAppDispatch();

  const { mutate: tryDeleteRide } = useMutation({
    mutationFn: deleteRide,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["user-rides", userId] });
      navigate("/rides");
    },
    onError: (error) => {
      dispatch(errorActions.setError(error.message));
    },
  });

  const { mutate: tryCreateBooking } = useMutation({
    mutationFn: createBooking,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["ride-bookings", params.id] });
    },
    onError: (error) => {
      dispatch(errorActions.setError(error.message));
    },
  });

  const handleInputChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setNumberOfSeats(parseInt(event.target.value));
  };

  const handleNavigation = () => {
    navigate(`/rides/edit/${params.id}`);
  };

  const handleConfirmDelete = () => {
    tryDeleteRide(parseInt(params.id!));
    deleteModalRef.current!.close();
  };

  const handleConfirmBooking = (
    totalPrice: number,
    userId: string,
    rideId: number
  ) => {
    tryCreateBooking({
      seatsBooked: numberOfSeats,
      totalPrice,
      userId,
      rideId,
    });
    bookingmodalRef.current!.close();
  };

  return (
    <div>
      <header>
        <h2>Ride {ride.id}</h2>
      </header>
      {isRideCreator && (
        <>
          <Button label="Edit ride" onClick={() => handleNavigation()} />
          <Button
            label="Delete ride"
            onClick={() => deleteModalRef.current!.open()}
          />
          <Modal
            title="Ride deletion"
            ref={deleteModalRef}
            onCancel={() => deleteModalRef.current!.close()}
            onConfirm={handleConfirmDelete}
          >
            <p>Are you sure you want to delete this ride?</p>
          </Modal>
        </>
      )}
      {isAuthenticated &&
        !isRideCreator &&
        ride.seatsAvailable >
          bookings.filter((booking) => booking.bookingStatus == "accepted")
            .length && (
          <>
            <Button
              label="Book ride"
              onClick={() => bookingmodalRef.current!.open()}
            />
            <Modal
              title="Ride booking"
              ref={bookingmodalRef}
              onCancel={() => bookingmodalRef.current!.close()}
              onConfirm={() =>
                handleConfirmBooking(
                  ride.pricePerSeat * numberOfSeats,
                  userId,
                  ride.id
                )
              }
            >
              <Input
                label="Number of seats"
                id="seats"
                name="seats"
                type="number"
                value={numberOfSeats}
                min={1}
                max={4}
                onChange={handleInputChange}
                required
              />
            </Modal>
          </>
        )}
    </div>
  );
};

export default SingleRidePage;
