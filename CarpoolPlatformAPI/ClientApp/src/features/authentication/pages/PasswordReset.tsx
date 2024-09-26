import { useNavigation, useSearchParams } from "react-router-dom";
import { useMutation } from "@tanstack/react-query";
import { resetPassword } from "../../../services/auth-service";
import { useState } from "react";
import { PasswordDTO } from "../types";
import Button from "../../../components/Button";
import Input from "../../../components/Input";
import { ValidationError, ValidationErrorResponse } from "../../../utils/http";
import { useAppDispatch } from "../../../hooks/store-hooks";
import { errorActions } from "../../../store/error-slice";

const initialFormData: PasswordDTO = {
  password: "",
  newPassword: "",
  newPasswordConfirmation: "",
};

const PasswordResetPage = () => {
  const [formData, setFormData] = useState<PasswordDTO>(initialFormData);
  const [validation, setValidation] = useState<ValidationErrorResponse | null>(
    null
  );
  const [searchParams] = useSearchParams();
  const email = searchParams.get("userEmail");
  const resetToken = searchParams.get("resetToken");
  const dispatch = useAppDispatch();
  const navigation = useNavigation();

  const { mutate: tryResetPassword } = useMutation({
    mutationFn: resetPassword,
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

  const handleInputChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = event.target;
    setFormData((prevData) => ({
      ...prevData,
      [name]: value,
    }));
  };

  const handleResetPassword = (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    if (email && resetToken) {
      tryResetPassword({
        email,
        resetToken: encodeURIComponent(resetToken),
        passwordDTO: formData,
      });
    }
  };

  return (
    <form onSubmit={handleResetPassword}>
      <Input
        label="Current password"
        id="current-password"
        name="password"
        type="password"
        value={formData.password}
        onChange={handleInputChange}
        placeholder="Enter your current password ..."
        required
        validationErrorMessage={validation?.errors.Password[0]}
      />
      <Input
        label="New password"
        id="new-password"
        name="newPassword"
        type="password"
        value={formData.newPassword}
        onChange={handleInputChange}
        placeholder="Enter your new password ..."
        required
        validationErrorMessage={validation?.errors.NewPassword[0]}
      />
      <Input
        label="New password confirmation"
        id="new-password-confirmation"
        name="newPasswordConfirmation"
        type="password"
        value={formData.newPasswordConfirmation}
        onChange={handleInputChange}
        placeholder="Repeat your new password ..."
        required
        validationErrorMessage={validation?.errors.NewPasswordConfirmation[0]}
      />
      <Button
        type="submit"
        label={
          navigation.state === "submitting"
            ? "Submitting ..."
            : "Change password"
        }
        disabled={navigation.state === "submitting"}
      />
    </form>
  );
};

export default PasswordResetPage;
