import { useEffect, useState } from "react";
import { Location } from "../types";
import { debounce } from "lodash";
import { useQuery } from "@tanstack/react-query";
import { getLocations } from "../../../services/ride-service";
import Input from "../../../components/Input";

interface LocationDropdownProps {
  label: string;
  setSelectedLocation: React.Dispatch<React.SetStateAction<string>>;
  defaultValue?: string;
  validationErrorMessage?: string;
}

const LocationsDropdown: React.FC<LocationDropdownProps> = ({
  label,
  setSelectedLocation,
  defaultValue,
  validationErrorMessage,
}) => {
  const [filteredLocations, setFilteredLocations] = useState<Location[]>([]);
  const [searchTerm, setSearchTerm] = useState<string>(defaultValue ?? "");

  useEffect(() => {
    if (defaultValue) {
      setSelectedLocation(defaultValue);
    }
  }, [defaultValue, setSelectedLocation]);

  const {
    data: locations,
    isLoading,
    error,
  } = useQuery({
    queryKey: ["locations"],
    queryFn: () => getLocations(),
    staleTime: Infinity,
  });

  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const input = e.target.value;
    setSearchTerm(input);
    filterLocations(input);
  };

  const filterLocations = debounce((input: string) => {
    const filtered = locations!.filter((location) =>
      `${location.city}, ${location.country}`
        .toLowerCase()
        .includes(input.toLowerCase())
    );
    setFilteredLocations(filtered);
  }, 300);

  const handleSelectLocation = (location: Location) => {
    setSelectedLocation(`${location.city}, ${location.country}`);
    setSearchTerm(`${location.city}, ${location.country}`);
    setFilteredLocations([]);
  };

  let content;
  if (isLoading) {
    content = <p>Loading ...</p>;
  } else if (error) {
    content = <p>Error!</p>;
  } else if (locations) {
    content = (
      <div className="location-dropdown">
        <Input
          label={label}
          id="location"
          name="location"
          type="text"
          value={searchTerm}
          onChange={handleInputChange}
          placeholder={`Search for ${label.toLowerCase()}...`}
          autoComplete="off"
          required
          validationErrorMessage={validationErrorMessage}
        />
        {filteredLocations.length > 0 && (
          <ul className="dropdown-menu">
            {filteredLocations.map((location) => (
              <li
                key={location.id}
                onClick={() => handleSelectLocation(location)}
              >
                {location.city}, {location.country}
              </li>
            ))}
          </ul>
        )}
      </div>
    );
  }

  return <>{content}</>;
};

export default LocationsDropdown;
