export interface RegistrationRequestDTO {
  email: string;
  password: string;
  firstName: string;
  lastName: string;
  phoneNumber: string;
  birthDate: string;
  profileBio?: string;
  chattinessPrefs?: string;
  musicPrefs?: string;
  smokingPrefs?: string;
  petsPrefs?: string;
}

export interface LoginRequestDTO {
  email: string;
  password: string;
}

export interface LoginResponseDTO {
  token: string;
  emailConfirmed: boolean;
}

export interface PasswordDTO {
  password: string;
  newPassword: string;
  newPasswordConfirmation: string;
}

export interface EmailDTO {
  email: string;
  newEmail: string;
  newEmailConfirmation: string;
}
