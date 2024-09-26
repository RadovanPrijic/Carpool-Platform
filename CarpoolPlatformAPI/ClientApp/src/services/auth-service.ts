import { getAuthToken } from "../utils/auth";
import {
  EmailDTO,
  LoginRequestDTO,
  LoginResponseDTO,
  PasswordDTO,
  RegistrationRequestDTO,
} from "../features/authentication/types";
import { API_ROUTES } from "../utils/api-config";
import { User } from "../features/users/types";
import { httpRequest } from "../utils/http";

export async function login(
  loginRequestDTO: LoginRequestDTO
): Promise<LoginResponseDTO> {
  return httpRequest<LoginResponseDTO>(`${API_ROUTES.USERS}/login`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(loginRequestDTO),
  });
}

export async function register(
  registrationRequestDTO: RegistrationRequestDTO
): Promise<User> {
  return httpRequest<User>(`${API_ROUTES.USERS}/register`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(registrationRequestDTO),
  });
}

interface InitiateEmailChangeArgs {
  id: string;
  emailDTO: EmailDTO;
}

export async function initiateEmailConfirmation(id: string): Promise<string> {
  await httpRequest<void>(
    `${API_ROUTES.USERS}/initiate-email-confirmation/${id}`,
    {
      method: "POST",
      headers: {
        Authorization: `Bearer ${getAuthToken()}`,
      },
    }
  );
  return "Check your email address for an email confirmation link.";
}

export async function initiateEmailChange({
  id,
  emailDTO,
}: InitiateEmailChangeArgs): Promise<string> {
  await httpRequest<void>(`${API_ROUTES.USERS}/initiate-email-change/${id}`, {
    method: "POST",
    headers: {
      Authorization: `Bearer ${getAuthToken()}`,
      "Content-Type": "application/json",
    },
    body: JSON.stringify(emailDTO),
  });
  return "Check your email address for an email change link.";
}

interface ConfirmEmailArgs {
  id: string;
  confirmationToken: string;
  emailChange: boolean;
  newEmail?: string;
}

export async function confirmEmail({
  id,
  confirmationToken,
  emailChange,
  newEmail,
}: ConfirmEmailArgs): Promise<string> {
  await httpRequest<void>(
    `${
      API_ROUTES.USERS
    }/confirm-email?id=${id}&confirmationToken=${confirmationToken}&emailChange=${emailChange}${
      newEmail !== undefined ? `&newEmail=${newEmail}` : ""
    }`,
    {
      method: "POST",
      headers: {
        Authorization: `Bearer ${getAuthToken()}`,
      },
    }
  );
  return `Your email address has been ${
    emailChange ? "changed" : "confirmed"
  }.`;
}

export async function initiatePasswordReset(email: string): Promise<string> {
  await httpRequest<void>(
    `${API_ROUTES.USERS}/initiate-password-reset/${email}`,
    {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
    }
  );
  return "Check your email address for a password reset link.";
}

interface ResetPasswordArgs {
  email: string;
  resetToken: string;
  passwordDTO: PasswordDTO;
}

export async function resetPassword({
  email,
  resetToken,
  passwordDTO,
}: ResetPasswordArgs): Promise<string> {
  await httpRequest<void>(
    `${API_ROUTES.USERS}/reset-password?email=${email}&resetToken=${resetToken}`,
    {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(passwordDTO),
    }
  );
  return "Your password has been reset.";
}
