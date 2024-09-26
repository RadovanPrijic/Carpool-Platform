import Input from "../../../components/Input";

interface ReviewFormProps {
  filter: string;
  onChange: (event: React.ChangeEvent<HTMLInputElement>) => void;
}

const BookingsFilter: React.FC<ReviewFormProps> = ({ filter, onChange }) => {
  return (
    <div>
      <Input
        label="Your requested bookings"
        id="by-user-requested"
        type="radio"
        value="by-user-requested"
        checked={filter === "by-user-requested"}
        onChange={onChange}
      />
      <Input
        label="Your accepted bookings"
        id="by-user-accepted"
        type="radio"
        value="by-user-accepted"
        checked={filter === "by-user-accepted"}
        onChange={onChange}
      />
      <Input
        label="Requested bookings for your ride"
        id="for-user-requested"
        type="radio"
        value="for-user-requested"
        checked={filter === "for-user-requested"}
        onChange={onChange}
      />
      <Input
        label="Accepted bookings for your ride"
        id="for-user-accepted"
        type="radio"
        value="for-user-accepted"
        checked={filter === "for-user-accepted"}
        onChange={onChange}
      />
    </div>
  );
};

export default BookingsFilter;
