import { useMutation } from "@tanstack/react-query";
import { EmailDTO } from "../types";
import { initiateEmailChange } from "../../../services/auth-service";
import { useAppDispatch, useAppSelector } from "../../../hooks/store-hooks";
import { useState } from "react";
import Input from "../../../components/Input";
import Button from "../../../components/Button";
import { useNavigation } from "react-router";
import { ValidationError, ValidationErrorResponse } from "../../../utils/http";
import { errorActions } from "../../../store/error-slice";

const initialFormData: EmailDTO = {
  email: "",
  newEmail: "",
  newEmailConfirmation: "",
};

const EmailChangePage = () => {
  const [formData, setFormData] = useState<EmailDTO>(initialFormData);
  const [validation, setValidation] = useState<ValidationErrorResponse | null>(
    null
  );
  const userId = useAppSelector((state) => state.auth.userId);
  const dispatch = useAppDispatch();
  const navigation = useNavigation();

  const { mutate: tryInitiateEmailChange } = useMutation({
    mutationFn: initiateEmailChange,
    onSuccess: () => {
      setFormData(initialFormData);
      setValidation(null);
    },
    onError: (error) => {
      if (error instanceof ValidationError) {
        setValidation(error.validationErrors);
      } else {
        dispatch(errorActions.setError(error.message));
      }
    },
  });

  const handleInputChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = event.target;
    setFormData((prevData) => ({
      ...prevData,
      [name]: value,
    }));
  };

  const handleInitiateEmailChange = (
    event: React.FormEvent<HTMLFormElement>
  ) => {
    event.preventDefault();
    tryInitiateEmailChange({ id: userId, emailDTO: formData });
  };

  return (
    <form onSubmit={handleInitiateEmailChange}>
      <Input
        label="Current email address"
        id="current-email"
        name="email"
        type="email"
        value={formData.email}
        onChange={handleInputChange}
        placeholder="Enter your current email address ..."
        required
        validationErrorMessage={validation?.errors.Email[0]}
      />

      <Input
        label="New email address"
        id="new-email"
        name="newEmail"
        type="email"
        value={formData.newEmail}
        onChange={handleInputChange}
        placeholder="Enter your new email address ..."
        required
        validationErrorMessage={validation?.errors.NewEmail[0]}
      />
      <Input
        label="New email address confirmation"
        id="new-email-confirmation"
        name="newEmailConfirmation"
        type="email"
        value={formData.newEmailConfirmation}
        onChange={handleInputChange}
        placeholder="Repeat your new email address ..."
        required
        validationErrorMessage={validation?.errors.NewEmailConfirmation[0]}
      />
      <Button
        type="submit"
        label={
          navigation.state === "submitting" ? "Sending ..." : "Send change link"
        }
        disabled={navigation.state === "submitting"}
      />
    </form>
  );
};

export default EmailChangePage;
