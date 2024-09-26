import { Message } from "../types";
import classes from "../pages/Chat.module.css";
import { useRef, useState } from "react";
import { useMutation } from "@tanstack/react-query";
import { updateMessage } from "../../../services/message-service";
import { queryClient } from "../../../utils/api-config";
import { useAppDispatch, useAppSelector } from "../../../hooks/store-hooks";
import Modal, { ModalHandle } from "../../../components/Modal";
import Button from "../../../components/Button";
import Textarea from "../../../components/Textarea";
import { errorActions } from "../../../store/error-slice";
import { ValidationError } from "../../../utils/http";

interface MessageComponentProps {
  message: Message;
  receiverId: string;
}

const MessageComponent: React.FC<MessageComponentProps> = ({
  message,
  receiverId,
}) => {
  const userId = useAppSelector((state) => state.auth.userId);
  const [messageEdit, setMessageEdit] = useState<string>("");
  const [editMessageId, setEditMessageId] = useState<number | null>(null);
  const [removeMessageId, setRemoveMessageId] = useState<number | null>(null);
  const editModalRef = useRef<ModalHandle>(null);
  const deleteModalRef = useRef<ModalHandle>(null);
  const dispatch = useAppDispatch();

  const { mutate: tryUpdateMessage } = useMutation({
    mutationFn: updateMessage,
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: ["inbox", userId],
      });
      queryClient.invalidateQueries({
        queryKey: ["conversation-messages", userId, receiverId],
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

  const openModal = (
    modalType: "edit" | "delete",
    messageId: number,
    currentContent?: string
  ) => {
    if (modalType === "edit" && currentContent) {
      setEditMessageId(messageId);
      setMessageEdit(currentContent);
      editModalRef.current!.open();
    } else if (modalType === "delete") {
      setRemoveMessageId(messageId);
      deleteModalRef.current!.open();
    }
  };

  const closeModal = (modalType: "edit" | "delete") => {
    if (modalType === "edit") {
      setEditMessageId(null);
      setMessageEdit("");
      editModalRef.current!.close();
    } else if (modalType === "delete") {
      setRemoveMessageId(null);
      deleteModalRef.current!.close();
    }
  };

  const handleInputChange = (event: React.ChangeEvent<HTMLTextAreaElement>) => {
    setMessageEdit(event.target.value);
  };

  const handleEdit = () => {
    if (editMessageId !== null) {
      tryUpdateMessage({
        id: editMessageId,
        messageUpdateDTO: { content: messageEdit },
      });
      closeModal("edit");
    }
  };

  const handleDelete = () => {
    if (removeMessageId !== null) {
      tryUpdateMessage({
        id: removeMessageId,
        messageUpdateDTO: {
          content: "This message has been deleted by the user.",
        },
      });
      closeModal("delete");
    }
  };

  return (
    <div
      className={
        classes[`message ${message.sender.id === userId ? "sent" : "received"}`]
      }
    >
      <p>{message.content}</p>
      <div>
        {message.sender.id === userId && (
          <>
            {message.content !==
              "This message has been deleted by the user." && (
              <>
                <Button
                  label="Edit"
                  onClick={() => openModal("edit", message.id, message.content)}
                />
                <Button
                  label="Delete"
                  onClick={() => openModal("delete", message.id)}
                />
              </>
            )}
          </>
        )}
        <Modal
          title="Message update"
          ref={editModalRef}
          onCancel={() => closeModal("edit")}
          onConfirm={() => handleEdit()}
        >
          <Textarea
            label="Edit message content"
            id="message"
            name="message"
            value={messageEdit}
            onChange={handleInputChange}
            placeholder="Edit your message ..."
            required
          />
        </Modal>
        <Modal
          title="Message deletion"
          ref={deleteModalRef}
          onCancel={() => closeModal("delete")}
          onConfirm={() => handleDelete()}
        >
          <p>Are you sure you want to delete this message?</p>
        </Modal>
      </div>
      <span className={classes["timestamp"]}>{message.createdAt}</span>
    </div>
  );
};

export default MessageComponent;
