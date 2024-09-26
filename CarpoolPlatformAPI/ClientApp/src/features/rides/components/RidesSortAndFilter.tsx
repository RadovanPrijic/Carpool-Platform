import React, { useState, useEffect, useCallback } from "react";
import { Ride } from "../types";
import { useAppDispatch } from "../../../hooks/store-hooks";
import { ridesActions } from "../rides-slice";
import Input from "../../../components/Input";
import Dropdown from "../../../components/Dropdown";

const timeRanges = [
  { label: "Midnight to 6 AM", start: "00:00", end: "06:00" },
  { label: "6:01 AM to 12 PM", start: "06:01", end: "12:00" },
  { label: "12:01 PM to 6 PM", start: "12:01", end: "18:00" },
  { label: "6:01 PM to Midnight", start: "18:01", end: "23:59" },
];

const isTimeInRange = (
  timeString: string,
  start: string,
  end: string
): boolean => {
  const [timeHours, timeMinutes] = timeString.split(":").map(Number);
  const [startHours, startMinutes] = start.split(":").map(Number);
  const [endHours, endMinutes] = end.split(":").map(Number);

  const time = timeHours * 60 + timeMinutes;
  const startTime = startHours * 60 + startMinutes;
  const endTime = endHours * 60 + endMinutes;

  return time >= startTime && time <= endTime;
};

interface RidesSortAndFilterProps {
  rides: Ride[];
}

const RidesSortAndFilter: React.FC<RidesSortAndFilterProps> = ({ rides }) => {
  const [sortOption, setSortOption] = useState<string>("departureTime");
  const [departureTimeFilter, setDepartureTimeFilter] = useState<string[]>([]);
  const dispatch = useAppDispatch();

  const applyFiltersAndSort = useCallback(() => {
    let filteredRides = rides;

    if (departureTimeFilter.length > 0) {
      filteredRides = filteredRides.filter((ride) => {
        const rideTime = ride.departureTime.split("T")[1].slice(0, 5);

        return departureTimeFilter.some((filter) =>
          timeRanges.some(
            (range) =>
              range.label === filter &&
              isTimeInRange(rideTime, range.start, range.end)
          )
        );
      });
    }

    const sortedRides = [...filteredRides].sort((a, b) => {
      if (sortOption === "pricePerSeat") {
        return a.pricePerSeat - b.pricePerSeat;
      } else if (sortOption === "departureTime") {
        return (
          new Date(a.departureTime).getTime() -
          new Date(b.departureTime).getTime()
        );
      }
      return 0;
    });

    dispatch(ridesActions.setFilteredRides(sortedRides));
  }, [rides, departureTimeFilter, sortOption, dispatch]);

  useEffect(() => {
    applyFiltersAndSort();
  }, [sortOption, departureTimeFilter, applyFiltersAndSort]);

  const handleSortChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
    setSortOption(e.target.value);
  };

  const handleDepartureTimeFilter = (label: string) => {
    const newFilter = departureTimeFilter.includes(label)
      ? departureTimeFilter.filter((t) => t !== label)
      : [...departureTimeFilter, label];
    setDepartureTimeFilter(newFilter);
  };

  return (
    <div>
      <Dropdown
        label="Sort by: "
        id="sorting-choice"
        name="sorting-choice"
        value={sortOption}
        onChange={handleSortChange}
        options={[
          { label: "Departure Time", value: "departureTime" },
          {
            label: "Price (lowest first)",
            value: "pricePerSeat",
          },
        ]}
      />
      {timeRanges.map((range) => (
        <Input
          key={range.label}
          label={range.label}
          id="time-range"
          name="time-range"
          type="checkbox"
          value={range.label}
          onChange={() => handleDepartureTimeFilter(range.label)}
          checked={departureTimeFilter.includes(range.label)}
        />
      ))}
    </div>
  );
};

export default RidesSortAndFilter;
