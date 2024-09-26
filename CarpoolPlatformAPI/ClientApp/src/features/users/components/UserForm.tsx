import { ChangeEvent } from "react";
import Button from "../../../components/Button";
import Dropdown from "../../../components/Dropdown";
import Input from "../../../components/Input";
import Textarea from "../../../components/Textarea";
import { useNavigation } from "react-router";
import { ValidationErrorResponse } from "../../../utils/http";

interface UserFormProps {
  email?: string;
  password?: string;
  firstName: string;
  lastName: string;
  phoneNumber: string;
  birthDate: string;
  profileBio?: string;
  chattinessPrefs?: string;
  musicPrefs?: string;
  smokingPrefs?: string;
  petsPrefs?: string;
  onSubmit: (event: React.FormEvent<HTMLFormElement>) => void;
  onChange: (
    event: ChangeEvent<
      HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement
    >
  ) => void;
  validation?: ValidationErrorResponse | null;
}

const UserForm: React.FC<UserFormProps> = ({
  email,
  password,
  firstName,
  lastName,
  phoneNumber,
  birthDate,
  profileBio,
  chattinessPrefs,
  musicPrefs,
  smokingPrefs,
  petsPrefs,
  onSubmit,
  onChange,
  validation,
}) => {
  const navigation = useNavigation();

  return (
    <form onSubmit={onSubmit}>
      {email !== undefined && (
        <Input
          label="Email address"
          id="email"
          name="email"
          type="email"
          value={email}
          onChange={onChange}
          placeholder="Enter your email address ..."
          required
          validationErrorMessage={validation?.errors.Email[0]}
        />
      )}
      {password !== undefined && (
        <Input
          label="Password"
          id="password"
          name="password"
          type="password"
          value={password}
          minLength={8}
          maxLength={30}
          onChange={onChange}
          placeholder="Enter your password ..."
          required
          validationErrorMessage={validation?.errors.Password[0]}
        />
      )}
      <Input
        label="First name"
        id="first-name"
        name="firstName"
        type="text"
        value={firstName}
        onChange={onChange}
        placeholder="Enter your first name ..."
        required
        validationErrorMessage={validation?.errors.FirstName[0]}
      />
      <Input
        label="Last name"
        id="last-name"
        name="lastName"
        type="text"
        value={lastName}
        onChange={onChange}
        placeholder="Enter your last name ..."
        required
        validationErrorMessage={validation?.errors.LastName[0]}
      />
      <Input
        label="Phone number"
        id="phone-number"
        name="phoneNumber"
        type="tel"
        value={phoneNumber}
        onChange={onChange}
        placeholder="Enter your phone number ..."
        required
        validationErrorMessage={validation?.errors.PhoneNumber[0]}
      />
      <Input
        label="Birth date"
        id="birth-date"
        name="birthDate"
        type="date"
        value={birthDate.split("T")[0]}
        onChange={onChange}
        required
        validationErrorMessage={validation?.errors.BirthDate[0]}
      />
      <Textarea
        label="Profile biography (optional)"
        id="profile-bio"
        name="profileBio"
        value={profileBio ?? ""}
        onChange={onChange}
        placeholder="Tell us something about yourself ..."
        validationErrorMessage={validation?.errors.ProfileBio[0]}
      />
      <Dropdown
        label="Chattiness preferences (optional)"
        id="chattiness-prefs"
        name="chattinessPrefs"
        value={chattinessPrefs ?? ""}
        onChange={onChange}
        options={[
          { label: "I love chatting!", value: "I love chatting!" },
          {
            label: "I chat only when I feel like it.",
            value: "I chat only when I feel like it.",
          },
          {
            label: "I prefer silence while driving.",
            value: "I prefer silence while driving.",
          },
        ]}
        validationErrorMessage={validation?.errors.ChattinessPrefs[0]}
      />
      <Dropdown
        label="Music preferences (optional)"
        id="music-prefs"
        name="musicPrefs"
        value={musicPrefs ?? ""}
        onChange={onChange}
        options={[
          { label: "Let there be music!", value: "Let there be music!" },
          {
            label: "I like music only when I'm in the right mood.",
            value: "I like music only when I'm in the right mood.",
          },
          {
            label: "Silence is golden.",
            value: "Silence is golden.",
          },
        ]}
        validationErrorMessage={validation?.errors.MusicPrefs[0]}
      />
      <Dropdown
        label="Smoking preferences (optional)"
        id="smoking-prefs"
        name="smokingPrefs"
        value={smokingPrefs ?? ""}
        onChange={onChange}
        options={[
          {
            label: "I have no problem with smoking in the car.",
            value: "I have no problem with smoking in the car.",
          },
          {
            label: "Smoke pauses outside the car are fine with me.",
            value: "Smoke pauses outside the car are fine with me.",
          },
          {
            label: "No smoking, please.",
            value: "No smoking, please.",
          },
        ]}
        validationErrorMessage={validation?.errors.SmokingPrefs[0]}
      />
      <Dropdown
        label="Pets preferences (optional)"
        id="pets-prefs"
        name="petsPrefs"
        value={petsPrefs ?? ""}
        onChange={onChange}
        options={[
          { label: "I adore pets!", value: "I adore pets!" },
          {
            label: "I only like certain kinds of pets.",
            value: "I only like certain kinds of pets.",
          },
          {
            label: "I prefer not to travel with pets.",
            value: "I prefer not to travel with pets.",
          },
        ]}
        validationErrorMessage={validation?.errors.PetsPrefs[0]}
      />
      <Button
        type="submit"
        label={
          navigation.state === "submitting"
            ? "Submitting..."
            : email !== undefined
            ? "Register"
            : "Edit user"
        }
        disabled={navigation.state === "submitting"}
      />
    </form>
  );
};

export default UserForm;
