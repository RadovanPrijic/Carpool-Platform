import { API_ROUTES } from "../utils/api-config";
import {
  Notification,
  Picture,
  User,
  UserUpdateDTO,
} from "../features/users/types";
import { getAuthToken } from "../utils/auth";
import { httpRequest } from "../utils/http";

export async function getUserById(id: string): Promise<User> {
  return httpRequest<User>(`${API_ROUTES.USERS}/${id}`, {
    method: "GET",
    headers: {
      Authorization: `Bearer ${getAuthToken()}`,
      "Content-Type": "application/json",
    },
  });
}

interface UpdateUserArgs {
  id: string;
  userUpdateDTO: UserUpdateDTO;
}

export async function updateUser({
  id,
  userUpdateDTO,
}: UpdateUserArgs): Promise<User> {
  return httpRequest<User>(`${API_ROUTES.USERS}/${id}`, {
    method: "PUT",
    headers: {
      Authorization: `Bearer ${getAuthToken()}`,
      "Content-Type": "application/json",
    },
    body: JSON.stringify(userUpdateDTO),
  });
}

interface GetUserNotificationsArgs {
  id: string;
  markAsChecked: boolean;
}

export async function getUserNotifications({
  id,
  markAsChecked,
}: GetUserNotificationsArgs): Promise<Notification[]> {
  return httpRequest<Notification[]>(
    `${API_ROUTES.USERS}/notifications/${id}`,
    {
      method: `${markAsChecked ? "PUT" : "GET"}`,
      headers: {
        Authorization: `Bearer ${getAuthToken()}`,
        "Content-Type": "application/json",
      },
    }
  );
}

interface UploadProfilePictureArgs {
  file: FormData;
  userId: string;
}

export async function uploadProfilePicture({
  file,
  userId,
}: UploadProfilePictureArgs): Promise<Picture> {
  return httpRequest<Picture>(
    `${API_ROUTES.USERS}/upload-profile-picture/${userId}`,
    {
      method: "POST",
      headers: {
        Authorization: `Bearer ${getAuthToken()}`,
      },
      body: file,
    }
  );
}

export async function deleteProfilePicture(id: number): Promise<string> {
  return httpRequest<string>(
    `${API_ROUTES.USERS}/remove-profile-picture/${id}`,
    {
      method: "DELETE",
      headers: {
        Authorization: `Bearer ${getAuthToken()}`,
      },
    }
  );
}
