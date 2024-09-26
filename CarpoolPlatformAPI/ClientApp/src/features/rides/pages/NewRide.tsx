import { useState } from "react";
import { useAppDispatch, useAppSelector } from "../../../hooks/store-hooks";
import { RideCreateDTO } from "../types";
import { useMutation } from "@tanstack/react-query";
import { createRide } from "../../../services/ride-service";
import { queryClient } from "../../../utils/api-config";
import RideForm from "../components/RideForm";
import { ValidationError, ValidationErrorResponse } from "../../../utils/http";
import { errorActions } from "../../../store/error-slice";

const NewRidePage = () => {
  const userId = useAppSelector((state) => state.auth.userId);
  const [startLocation, setStartLocation] = useState<string>("");
  const [endLocation, setEndLocation] = useState<string>("");
  const initialFormData: RideCreateDTO = {
    startLocation,
    endLocation,
    departureTime: "",
    pricePerSeat: 0,
    rideDescription: "",
    carInfo: "",
    seatsAvailable: 3,
    twoInBackseat: true,
    luggageSize: "",
    insuranceStatus: false,
    automaticBooking: false,
    userId: userId,
  };
  const [formData, setFormData] = useState<RideCreateDTO>(initialFormData);
  const [validation, setValidation] = useState<ValidationErrorResponse | null>(
    null
  );
  const dispatch = useAppDispatch();

  const { mutate: tryCreateRide } = useMutation({
    mutationFn: createRide,
    onSuccess: () => {
      setFormData(initialFormData);
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
    setFormData((prevFormData: RideCreateDTO) => ({
      ...prevFormData,
      [name]: inputValue,
    }));
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    tryCreateRide({ ...formData, startLocation, endLocation });
  };

  return (
    <RideForm
      setStartLocation={setStartLocation}
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
      formType="Create"
      validation={validation}
    />
  );
};

export default NewRidePage;
