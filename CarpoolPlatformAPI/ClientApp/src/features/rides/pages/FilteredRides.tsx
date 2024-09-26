import { useSearchParams } from "react-router-dom";
import { getFilteredRides } from "../../../services/ride-service";
import { useEffect } from "react";
import { useMutation } from "@tanstack/react-query";
import RidesSortAndFilter from "../components/RidesSortAndFilter";
import { useAppDispatch, useAppSelector } from "../../../hooks/store-hooks";
import { ridesActions } from "../rides-slice";
import RideCard from "../components/RideCard";
import { errorActions } from "../../../store/error-slice";

const FilteredRidesPage = () => {
  const rides = useAppSelector((state) => state.rides.rides);
  const filteredRides = useAppSelector((state) => state.rides.filteredRides);
  const [searchParams] = useSearchParams();
  const from = searchParams.get("from")!;
  const to = searchParams.get("to")!;
  const date = searchParams.get("date")!;
  const seats = parseInt(searchParams.get("seats")!);
  const dispatch = useAppDispatch();

  const {
    mutate: tryGetFilteredRides,
    isPending,
    error,
  } = useMutation({
    mutationFn: getFilteredRides,
    onSuccess: (responseRides) => {
      dispatch(ridesActions.setRides(responseRides));
    },
    onError: (error) => {
      dispatch(errorActions.setError(error.message));
    },
  });

  useEffect(() => {
    tryGetFilteredRides({ from, to, date, seats });
  }, [from, to, date, seats, tryGetFilteredRides]);

  let content;
  if (isPending) {
    content = <p>Loading ...</p>;
  } else if (error) {
    content = <p>Error!</p>;
  } else if (rides) {
    content = (
      <>
        <RidesSortAndFilter rides={rides} />
        <ul>
          {filteredRides!.map((ride) => (
            <RideCard key={ride.id} ride={ride} />
          ))}
        </ul>
      </>
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

export default FilteredRidesPage;
