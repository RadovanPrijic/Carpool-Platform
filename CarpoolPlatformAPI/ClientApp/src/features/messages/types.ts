import { User } from "../users/types";

export interface Message {
  id: number;
  content: string;
  readStatus: boolean;
  createdAt: string;
  sender: User;
  receiver: User;
}

export interface Conversation {
  user: User;
  lastMessage: Message;
  unreadMessagesCount: number;
}

export interface MessageCreateDTO {
  content: string;
  senderId: string;
  receiverId: string;
}

export interface MessageUpdateDTO {
  content: string;
}
