export interface User {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber: string;
  birthDate: string;
  profileBio?: string;
  rating?: number;
  chattinessPrefs?: string;
  musicPrefs?: string;
  smokingPrefs?: string;
  petsPrefs?: string;
  createdAt: string;
  picture?: Picture;
  notifications: Notification[];
}

export interface UserUpdateDTO {
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

export interface Notification {
  id: number;
  message: string;
  checkedStatus: boolean;
  createdAt: string;
}

export interface Picture {
  id: number;
  filePath: string;
}
