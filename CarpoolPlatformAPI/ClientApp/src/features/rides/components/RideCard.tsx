import { useNavigate } from "react-router";
import { Ride } from "../types";
import Button from "../../../components/Button";

interface RideCardProps {
  ride: Ride;
}

const RideCard: React.FC<RideCardProps> = ({ ride }) => {
  const navigate = useNavigate();

  const handleNavigation = (id: number) => {
    navigate(`/rides/${id}`);
  };

  return (
    <li>
      {ride.id} | {ride.departureTime} | {ride.pricePerSeat} RSD{" "}
      <Button label="Go to ride" onClick={() => handleNavigation(ride.id)} />
    </li>
  );
};

export default RideCard;
