import { useQuery } from "@tanstack/react-query";
import { getAllRidesForUser } from "../../../services/ride-service";
import { useAppSelector } from "../../../hooks/store-hooks";
import RideCard from "../components/RideCard";

const UserRidesPage = () => {
  const userId = useAppSelector((state) => state.auth.userId);

  const {
    data: rides,
    isLoading,
    error,
  } = useQuery({
    queryKey: ["user-rides", userId],
    queryFn: () => getAllRidesForUser(userId),
  });

  let content;
  if (isLoading) {
    content = <p>Loading ...</p>;
  } else if (error) {
    content = <p>Error!</p>;
  } else if (rides) {
    content = (
      <ul>
        {rides.map((ride) => (
          <RideCard key={ride.id} ride={ride} />
        ))}
      </ul>
    );
  }

  return (
    <div>
      <header>
        <h2>Rides</h2>
      </header>
      {content}
    </div>
  );
};

export default UserRidesPage;
