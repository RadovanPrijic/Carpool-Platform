import { useMutation } from "@tanstack/react-query";
import { updateUser } from "../../../services/user-service";
import { UserUpdateDTO } from "../types";
import { useAppDispatch, useAppSelector } from "../../../hooks/store-hooks";
import { useState } from "react";
import { userActions } from "../user-slice";
import UserForm from "../components/UserForm";
import { ValidationError, ValidationErrorResponse } from "../../../utils/http";
import { errorActions } from "../../../store/error-slice";

const EditUserPage = () => {
  const currentUser = useAppSelector((state) => state.user.currentUser!);
  const {
    id,
    // eslint-disable-next-line @typescript-eslint/no-unused-vars
    email,
    // eslint-disable-next-line @typescript-eslint/no-unused-vars
    rating,
    // eslint-disable-next-line @typescript-eslint/no-unused-vars
    createdAt,
    // eslint-disable-next-line @typescript-eslint/no-unused-vars
    picture,
    // eslint-disable-next-line @typescript-eslint/no-unused-vars
    notifications,
    ...initialState
  } = currentUser;
  const [formData, setFormData] = useState<UserUpdateDTO>({ ...initialState });
  const [validation, setValidation] = useState<ValidationErrorResponse | null>(
    null
  );
  const dispatch = useAppDispatch();

  const { mutate: tryUpdateUser } = useMutation({
    mutationFn: updateUser,
    onSuccess: (updatedUser) => {
      dispatch(userActions.setCurrentUser({ ...updatedUser }));
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

  const handleUpdateUser = (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    tryUpdateUser({ id, userUpdateDTO: formData });
  };

  return (
    <UserForm
      firstName={formData.firstName}
      lastName={formData.lastName}
      phoneNumber={formData.phoneNumber}
      birthDate={formData.birthDate}
      profileBio={formData.profileBio}
      chattinessPrefs={formData.chattinessPrefs}
      musicPrefs={formData.musicPrefs}
      smokingPrefs={formData.smokingPrefs}
      petsPrefs={formData.petsPrefs}
      onSubmit={handleUpdateUser}
      onChange={handleInputChange}
      validation={validation}
    />
  );
};

export default EditUserPage;
