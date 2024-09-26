import { useMutation } from "@tanstack/react-query";
import { useState } from "react";
import { register } from "../../../services/auth-service";
import { RegistrationRequestDTO } from "../types";
import UserForm from "../../users/components/UserForm";
import { ValidationError, ValidationErrorResponse } from "../../../utils/http";
import { useAppDispatch } from "../../../hooks/store-hooks";
import { errorActions } from "../../../store/error-slice";

const initialFormData: RegistrationRequestDTO = {
  email: "",
  password: "",
  firstName: "",
  lastName: "",
  phoneNumber: "",
  birthDate: "",
};

const RegistrationPage = () => {
  const [formData, setFormData] =
    useState<RegistrationRequestDTO>(initialFormData);
  const [validation, setValidation] = useState<ValidationErrorResponse | null>(
    null
  );
  const dispatch = useAppDispatch();

  const { mutate: tryRegister } = useMutation({
    mutationFn: register,
    onSuccess: () => {
      setFormData(initialFormData);
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
    event: React.ChangeEvent<
      HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement
    >
  ) => {
    const { name, value } = event.target;
    setFormData((prevData) => ({
      ...prevData,
      [name]: value,
    }));
  };

  const handleRegister = (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    tryRegister(formData);
  };

  return (
    <UserForm
      email={formData.email}
      password={formData.password}
      firstName={formData.firstName}
      lastName={formData.lastName}
      phoneNumber={formData.phoneNumber}
      birthDate={formData.birthDate}
      profileBio={formData.profileBio}
      chattinessPrefs={formData.chattinessPrefs}
      musicPrefs={formData.musicPrefs}
      smokingPrefs={formData.smokingPrefs}
      petsPrefs={formData.petsPrefs}
      onSubmit={handleRegister}
      onChange={handleInputChange}
      validation={validation}
    />
  );
};

export default RegistrationPage;
