import { useRef, useState } from "react";
import { Booking } from "../types";
import Modal, { ModalHandle } from "../../../components/Modal";
import { useNavigate } from "react-router";
import { useMutation } from "@tanstack/react-query";
import { sendMessage } from "../../../services/message-service";
import { queryClient } from "../../../utils/api-config";
import { useAppDispatch, useAppSelector } from "../../../hooks/store-hooks";
import {
  cancelBooking,
  updateBooking,
} from "../../../services/booking-service";
import Button from "../../../components/Button";
import Textarea from "../../../components/Textarea";
import { errorActions } from "../../../store/error-slice";
import { ValidationError, ValidationErrorResponse } from "../../../utils/http";

interface BookingComponentProps {
  booking: Booking;
  filter: string;
}

const BookingComponent: React.FC<BookingComponentProps> = ({
  booking,
  filter,
}) => {
  const userId = useAppSelector((state) => state.auth.userId);
  const [messageContent, setMessageContent] = useState<string>("");
  const [validation, setValidation] = useState<ValidationErrorResponse | null>(
    null
  );
  const messageModalRef = useRef<ModalHandle>(null);
  const dispatch = useAppDispatch();
  const navigate = useNavigate();

  const { mutate: trySendMessage } = useMutation({
    mutationFn: sendMessage,
    onSuccess: (sentMessage) => {
      queryClient.invalidateQueries({
        queryKey: ["inbox", userId],
      });
      queryClient.invalidateQueries({
        queryKey: ["conversation-messages", userId, sentMessage.receiver.id],
      });
    },
    onError: (error) => {
      if (error instanceof ValidationError) {
        setValidation(error.validationErrors);
      } else {
        dispatch(errorActions.setError(error.message));
      }
    },
  });

  const { mutate: tryUpdateBooking } = useMutation({
    mutationFn: updateBooking,
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: ["bookings", userId, filter],
      });
    },
    onError: (error) => {
      dispatch(errorActions.setError(error.message));
    },
  });

  const { mutate: tryCancelBooking } = useMutation({
    mutationFn: cancelBooking,
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: ["bookings", userId, filter],
      });
    },
    onError: (error) => {
      dispatch(errorActions.setError(error.message));
    },
  });

  const handleInputChange = (event: React.ChangeEvent<HTMLTextAreaElement>) => {
    setMessageContent(event.target.value);
  };

  const handleConfirm = (senderId: string, receiverId: string) => {
    trySendMessage({ content: messageContent, senderId, receiverId });
    if (!validation) {
      messageModalRef.current!.close();
    }
  };

  const handleNavigation = (id: number) => {
    navigate(`/rides/review/${id}`);
  };

  return (
    <li>
      {booking.id}: {booking.bookingStatus} {booking.ride.id}
      {filter === "for-user-requested" && (
        <>
          <Button
            label="Accept"
            onClick={() =>
              tryUpdateBooking({
                id: booking.id,
                bookingUpdateDTO: { bookingStatus: "accepted" },
              })
            }
          />
          <Button
            label="Reject"
            onClick={() =>
              tryUpdateBooking({
                id: booking.id,
                bookingUpdateDTO: { bookingStatus: "rejected" },
              })
            }
          />
        </>
      )}
      {filter === "for-user-accepted" && (
        <Button
          label="Cancel"
          onClick={() =>
            tryUpdateBooking({
              id: booking.id,
              bookingUpdateDTO: { bookingStatus: "cancelled" },
            })
          }
        />
      )}
      {((filter === "by-user-accepted" &&
        new Date(booking.ride.departureTime) > new Date()) ||
        filter === "by-user-requested") && (
        <Button label="Cancel" onClick={() => tryCancelBooking(booking.id)} />
      )}
      {filter === "by-user-accepted" &&
        new Date(booking.ride.departureTime) < new Date() &&
        booking.review === null && (
          <Button label="Review" onClick={() => handleNavigation(booking.id)} />
        )}
      <Button label="Message" onClick={() => messageModalRef.current!.open()} />
      <Modal
        title={`Message ${
          filter.includes("by-user")
            ? booking.ride.user.firstName + " " + booking.ride.user.lastName
            : booking.user.firstName + " " + booking.user.lastName
        }`}
        ref={messageModalRef}
        onCancel={() => messageModalRef.current!.close()}
        onConfirm={() =>
          handleConfirm(
            userId,
            filter.includes("by-user") ? booking.ride.user.id : booking.user.id
          )
        }
      >
        <Textarea
          label="Your message"
          id="message"
          name="message"
          value={messageContent}
          onChange={handleInputChange}
          placeholder="Enter your message ..."
          required
          validationErrorMessage={validation?.errors.Content[0]}
        />
      </Modal>
    </li>
  );
};

export default BookingComponent;
