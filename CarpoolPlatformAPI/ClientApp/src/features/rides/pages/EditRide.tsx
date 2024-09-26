import { useState } from "react";
import { Ride, RideUpdateDTO } from "../types";
import { useMutation } from "@tanstack/react-query";
import { updateRide } from "../../../services/ride-service";
import { useLoaderData, useParams } from "react-router";
import { queryClient } from "../../../utils/api-config";
import { useAppDispatch, useAppSelector } from "../../../hooks/store-hooks";
import { Booking } from "../../bookings/types";
import RideForm from "../components/RideForm";
import { ValidationError, ValidationErrorResponse } from "../../../utils/http";
import { errorActions } from "../../../store/error-slice";

const EditRidePage = () => {
  const { ride } = useLoaderData() as {
    ride: Ride;
    bookings: Booking[];
  };
  const userId = useAppSelector((state) => state.auth.userId);
  const [startLocation, setStartLocation] = useState<string>("");
  const [endLocation, setEndLocation] = useState<string>("");
  // eslint-disable-next-line @typescript-eslint/no-unused-vars
  const { id, createdAt, user, ...rideData } = ride;
  const [formData, setFormData] = useState<RideUpdateDTO>({ ...rideData });
  const [validation, setValidation] = useState<ValidationErrorResponse | null>(
    null
  );
  const dispatch = useAppDispatch();
  const params = useParams();

  const { mutate: tryUpdateRide } = useMutation({
    mutationFn: updateRide,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["user-rides", userId] });
    },
    onError: (error) => {
      if (error instanceof ValidationError) {
        setValidation(error.validationErrors);
      } else {
        dispatch(errorActions.setError(error.message));
      }
    },
  });

  const handleInputChange = (
    e: React.ChangeEvent<
      HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement
    >
  ) => {
    const { name, value, type } = e.target;
    const inputValue =
      type === "checkbox" ? (e.target as HTMLInputElement).checked : value;
    setFormData((prevFormData: RideUpdateDTO) => ({
      ...prevFormData,
      [name]: inputValue,
    }));
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    tryUpdateRide({
      id: parseInt(params.id!),
      rideUpdateDTO: { ...formData, startLocation, endLocation },
    });
  };

  return (
    <RideForm
      startLocation={rideData.startLocation}
      setStartLocation={setStartLocation}
      endLocation={rideData.endLocation}
      setEndLocation={setEndLocation}
      departureTime={formData.departureTime}
      pricePerSeat={formData.pricePerSeat}
      rideDescription={formData.rideDescription}
      carInfo={formData.carInfo}
      seatsAvailable={formData.seatsAvailable}
      twoInBackseat={formData.twoInBackseat}
      luggageSize={formData.luggageSize}
      insuranceStatus={formData.insuranceStatus}
      automaticBooking={formData.automaticBooking}
      onSubmit={handleSubmit}
      onChange={handleInputChange}
      formType="Edit"
      validation={validation}
    />
  );
};

export default EditRidePage;
