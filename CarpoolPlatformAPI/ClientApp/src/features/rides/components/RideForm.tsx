import { ChangeEvent } from "react";
import Button from "../../../components/Button";
import Input from "../../../components/Input";
import Textarea from "../../../components/Textarea";
import LocationsDropdown from "./LocationsDropdown";
import Dropdown from "../../../components/Dropdown";
import { useNavigation } from "react-router";
import { ValidationErrorResponse } from "../../../utils/http";

interface RideFormProps {
  startLocation?: string;
  setStartLocation: React.Dispatch<React.SetStateAction<string>>;
  endLocation?: string;
  setEndLocation: React.Dispatch<React.SetStateAction<string>>;
  departureTime: string;
  pricePerSeat: number;
  rideDescription?: string;
  carInfo: string;
  seatsAvailable: number;
  twoInBackseat: boolean;
  luggageSize: string;
  insuranceStatus: boolean;
  automaticBooking: boolean;
  onSubmit: (event: React.FormEvent<HTMLFormElement>) => void;
  onChange: (
    event: ChangeEvent<
      HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement
    >
  ) => void;
  formType: "Create" | "Edit";
  validation?: ValidationErrorResponse | null;
}

const RideForm: React.FC<RideFormProps> = ({
  startLocation,
  setStartLocation,
  endLocation,
  setEndLocation,
  departureTime,
  pricePerSeat,
  rideDescription,
  carInfo,
  seatsAvailable,
  twoInBackseat,
  luggageSize,
  insuranceStatus,
  automaticBooking,
  onSubmit,
  onChange,
  formType,
  validation,
}) => {
  const navigation = useNavigation();

  return (
    <form onSubmit={onSubmit}>
      <LocationsDropdown
        label="Starting Location"
        setSelectedLocation={setStartLocation}
        defaultValue={startLocation}
        validationErrorMessage={validation?.errors.StartLocation[0]}
      />
      <LocationsDropdown
        label="End Location"
        setSelectedLocation={setEndLocation}
        defaultValue={endLocation}
        validationErrorMessage={validation?.errors.EndLocation[0]}
      />
      <Input
        label="Departure time"
        id="departure-time"
        name="departureTime"
        type="datetime-local"
        value={departureTime}
        onChange={onChange}
        required
        validationErrorMessage={validation?.errors.DepartureTime[0]}
      />
      <Input
        label="Price per seat"
        id="price-per-seat"
        name="pricePerSeat"
        type="number"
        value={pricePerSeat}
        onChange={onChange}
        required
        validationErrorMessage={validation?.errors.PricePerSeat[0]}
      />
      <Input
        label="Seats available"
        id="seats-available"
        name="seatsAvailable"
        type="number"
        value={seatsAvailable}
        min={1}
        max={4}
        onChange={onChange}
        required
        validationErrorMessage={validation?.errors.SeatsAvailable[0]}
      />
      <Input
        label="Two in Backseat"
        id="two-in-backseat"
        name="twoInBackseat"
        type="checkbox"
        value={twoInBackseat.toString()}
        checked={twoInBackseat}
        onChange={onChange}
        validationErrorMessage={validation?.errors.TwoInBackseat[0]}
      />
      <Textarea
        label="Ride description (optional)"
        id="ride-description"
        name="rideDescription"
        value={rideDescription ?? ""}
        onChange={onChange}
        placeholder="Enter additional information about your ride ..."
        maxLength={500}
        validationErrorMessage={validation?.errors.RideDescription[0]}
      />
      <Textarea
        label="Car information"
        id="car-info"
        name="carInfo"
        value={carInfo}
        onChange={onChange}
        maxLength={250}
        placeholder="Enter information about your car ..."
        required
        validationErrorMessage={validation?.errors.CarInformation[0]}
      />
      <Dropdown
        label="Luggage Size"
        id="luggage-size"
        name="luggageSize"
        value={luggageSize ?? ""}
        onChange={onChange}
        options={[
          { label: "Small", value: "Small" },
          {
            label: "Medium",
            value: "Medium",
          },
          {
            label: "Large",
            value: "Large",
          },
        ]}
        validationErrorMessage={validation?.errors.LuggageSize[0]}
      />
      <Input
        label="Insurance status"
        id="insurance-status"
        name="insuranceStatus"
        type="checkbox"
        value={insuranceStatus.toString()}
        checked={insuranceStatus}
        onChange={onChange}
        validationErrorMessage={validation?.errors.InsuranceStatus[0]}
      />
      <Input
        label="Automatic Booking"
        id="automatic-booking"
        name="automaticBooking"
        type="checkbox"
        value={automaticBooking.toString()}
        checked={automaticBooking}
        onChange={onChange}
        validationErrorMessage={validation?.errors.AutomaticBooking[0]}
      />
      <Button
        type="submit"
        label={
          navigation.state === "submitting"
            ? "Submitting ..."
            : `${formType} ride`
        }
        disabled={navigation.state === "submitting"}
      />
    </form>
  );
};

export default RideForm;
