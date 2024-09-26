import {
  Conversation,
  Message,
  MessageCreateDTO,
  MessageUpdateDTO,
} from "../features/messages/types";
import { API_ROUTES } from "../utils/api-config";
import { getAuthToken } from "../utils/auth";
import { httpRequest } from "../utils/http";

export async function getAllConversationsForUser(
  userId: string
): Promise<Conversation[]> {
  return httpRequest<Conversation[]>(`${API_ROUTES.MESSAGES}/inbox/${userId}`, {
    method: "GET",
    headers: {
      Authorization: `Bearer ${getAuthToken()}`,
      "Content-Type": "application/json",
    },
  });
}

export async function getAllConversationMessages(
  userOne: string,
  userTwo: string
): Promise<Message[]> {
  return httpRequest<Message[]>(
    `${API_ROUTES.MESSAGES}/?userOne=${userOne}&userTwo=${userTwo}`,
    {
      method: "GET",
      headers: {
        Authorization: `Bearer ${getAuthToken()}`,
        "Content-Type": "application/json",
      },
    }
  );
}

export async function sendMessage(
  messageCreateDTO: MessageCreateDTO
): Promise<Message> {
  return httpRequest<Message>(`${API_ROUTES.MESSAGES}/`, {
    method: "POST",
    headers: {
      Authorization: `Bearer ${getAuthToken()}`,
      "Content-Type": "application/json",
    },
    body: JSON.stringify(messageCreateDTO),
  });
}

interface UpdateMessageArgs {
  id: number;
  messageUpdateDTO: MessageUpdateDTO;
}

export async function updateMessage({
  id,
  messageUpdateDTO,
}: UpdateMessageArgs): Promise<Message> {
  return httpRequest<Message>(`${API_ROUTES.MESSAGES}/${id}`, {
    method: "PUT",
    headers: {
      Authorization: `Bearer ${getAuthToken()}`,
      "Content-Type": "application/json",
    },
    body: JSON.stringify(messageUpdateDTO),
  });
}

interface MarkConversationMessagesAsReadArgs {
  userId: string;
  otherUserId: string;
}

export async function markConversationMessagesAsRead({
  userId,
  otherUserId,
}: MarkConversationMessagesAsReadArgs): Promise<Message[]> {
  return httpRequest<Message[]>(
    `${API_ROUTES.MESSAGES}/chat?userId=${userId}&otherUserId=${otherUserId}`,
    {
      method: "PUT",
      headers: {
        Authorization: `Bearer ${getAuthToken()}`,
        "Content-Type": "application/json",
      },
    }
  );
}
