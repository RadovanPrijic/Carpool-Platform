import { useEffect } from "react";
import { useSearchParams } from "react-router-dom";
import { confirmEmail } from "../../../services/auth-service";
import { useMutation } from "@tanstack/react-query";
import { useAppDispatch } from "../../../hooks/store-hooks";
import { authActions } from "../auth-slice";
import { userActions } from "../../users/user-slice";
import { errorActions } from "../../../store/error-slice";

const EmailConfirmationPage = () => {
  const [searchParams] = useSearchParams();
  const userId = searchParams.get("userId");
  const confirmationToken = searchParams.get("confirmationToken");
  const changeToken = searchParams.get("changeToken");
  const newEmail = searchParams.get("newEmail");
  const dispatch = useAppDispatch();

  const {
    mutate: tryConfirmEmail,
    isSuccess,
    isError,
  } = useMutation({
    mutationFn: confirmEmail,
    onSuccess: () => {
      if (confirmationToken) {
        dispatch(authActions.confirmEmail());
      } else {
        dispatch(userActions.changeUserEmail(newEmail!));
      }
    },
    onError: (error) => {
      dispatch(errorActions.setError(error.message));
    },
  });

  useEffect(() => {
    if (userId && confirmationToken) {
      tryConfirmEmail({
        id: userId,
        confirmationToken: encodeURIComponent(confirmationToken),
        emailChange: false,
      });
    } else if (userId && changeToken && newEmail) {
      tryConfirmEmail({
        id: userId,
        confirmationToken: encodeURIComponent(changeToken),
        emailChange: true,
        newEmail,
      });
    }
  }, [userId, confirmationToken, changeToken, newEmail, tryConfirmEmail]);

  return (
    <div>
      Your email has
      {confirmationToken && isSuccess && " been confirmed."}
      {changeToken && isSuccess && " been changed."}
      {confirmationToken && isError && " not been confirmed."}
      {changeToken && isError && " not been changed."}
    </div>
  );
};

export default EmailConfirmationPage;
