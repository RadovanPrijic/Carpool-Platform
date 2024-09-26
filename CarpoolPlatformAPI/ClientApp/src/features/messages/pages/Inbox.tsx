import { useLoaderData, useNavigate } from "react-router-dom";
import classes from "./Inbox.module.css";
import { Conversation } from "../types";

const InboxPage = () => {
  const conversations = useLoaderData() as Conversation[];
  const navigate = useNavigate();

  const handleNavigation = (otherUserId: string) => {
    navigate(`/user/messages/chat/${otherUserId}`);
  };

  return (
    <div className={classes["inbox-container"]}>
      <h1>Inbox</h1>
      <ul className={classes["chat-list"]}>
        {conversations.map((conversation) => (
          <li
            key={conversation.lastMessage.id}
            className={classes["chat-item"]}
            onClick={() => handleNavigation(conversation.user.id)}
          >
            <h2 className={classes["chat-user"]}>
              {conversation.user.firstName} {conversation.user.lastName}
            </h2>
            <p className={classes["chat-last-message"]}>
              {conversation.lastMessage.content}
            </p>
            {conversation.unreadMessagesCount > 0 && (
              <span className={classes["chat-unread-count"]}>
                {conversation.unreadMessagesCount} unread
              </span>
            )}
          </li>
        ))}
      </ul>
    </div>
  );
};

export default InboxPage;
