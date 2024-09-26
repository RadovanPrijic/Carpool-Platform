import { useEffect, useState, useRef } from "react";
import { useParams } from "react-router-dom";
import classes from "./Chat.module.css";
import {
  getAllConversationMessages,
  markConversationMessagesAsRead,
  sendMessage,
} from "../../../services/message-service";
import { useMutation, useQuery } from "@tanstack/react-query";
import { useAppDispatch, useAppSelector } from "../../../hooks/store-hooks";
import { queryClient } from "../../../utils/api-config";
import Input from "../../../components/Input";
import Button from "../../../components/Button";
import MessageComponent from "../components/Message";
import { ValidationError } from "../../../utils/http";
import { errorActions } from "../../../store/error-slice";

const ChatPage = () => {
  const userId = useAppSelector((state) => state.auth.userId);
  const [newMessage, setNewMessage] = useState<string>("");
  const chatEndRef = useRef<HTMLDivElement | null>(null);
  const dispatch = useAppDispatch();
  const params = useParams();

  const {
    data: conversationMessages,
    isLoading,
    error,
  } = useQuery({
    queryKey: ["conversation-messages", userId, params.id],
    queryFn: () => getAllConversationMessages(userId, params.id!),
  });

  const { mutate: trySendMessage } = useMutation({
    mutationFn: sendMessage,
    onSuccess: () => {
      setNewMessage("");
      queryClient.invalidateQueries({
        queryKey: ["inbox", userId],
      });
      queryClient.invalidateQueries({
        queryKey: ["conversation-messages", userId, params.id],
      });
    },
    onError: (error) => {
      if (error instanceof ValidationError) {
        dispatch(
          errorActions.setError(error.validationErrors.errors.Content[0])
        );
      } else {
        dispatch(errorActions.setError(error.message));
      }
    },
  });

  const { mutate: tryMarkMessagesAsRead } = useMutation({
    mutationFn: markConversationMessagesAsRead,
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: ["inbox", userId],
      });
    },
  });

  const handleSendMessage = () => {
    trySendMessage({
      content: newMessage,
      senderId: userId,
      receiverId: params.id!,
    });
  };

  useEffect(() => {
    if (
      conversationMessages &&
      conversationMessages.some((m) => !m.readStatus)
    ) {
      tryMarkMessagesAsRead({ userId, otherUserId: params.id! });
    }
  }, [conversationMessages, userId, params.id, tryMarkMessagesAsRead]);

  useEffect(() => {
    chatEndRef.current?.scrollIntoView({ behavior: "smooth" });
  }, [conversationMessages]);

  let content;
  if (isLoading) {
    content = <p>Loading ...</p>;
  } else if (error) {
    content = <p>Error!</p>;
  } else if (conversationMessages) {
    content = (
      <div className={classes["chat-container"]}>
        <div className={classes["chat-history"]}>
          {conversationMessages.map((message) => (
            <MessageComponent
              key={message.id}
              message={message}
              receiverId={params.id!}
            />
          ))}
          <div ref={chatEndRef}></div>{" "}
        </div>
        <div className={classes["chat-input-container"]}>
          <Input
            label=""
            id="message"
            name="message"
            type="text"
            value={newMessage}
            onChange={(e) => setNewMessage(e.target.value)}
            className={classes["chat-input"]}
            placeholder="Enter your message ..."
            required
          />
          <Button
            label="Send"
            onClick={handleSendMessage}
            className={classes["send-button"]}
          />
        </div>
      </div>
    );
  }

  return (
    <div>
      <header>
        <h2>Messages</h2>
      </header>
      {content}
    </div>
  );
};

export default ChatPage;
