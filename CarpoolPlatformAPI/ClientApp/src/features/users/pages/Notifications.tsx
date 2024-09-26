import { useMutation, useQuery } from "@tanstack/react-query";
import { useAppDispatch, useAppSelector } from "../../../hooks/store-hooks";
import { getUserNotifications } from "../../../services/user-service";
import { useEffect } from "react";
import { queryClient } from "../../../utils/api-config";
import { userActions } from "../user-slice";
import NotificationComponent from "../components/Notification";
import { errorActions } from "../../../store/error-slice";

const NotificationsPage = () => {
  const userId = useAppSelector((state) => state.auth.userId);
  const dispatch = useAppDispatch();

  const {
    data: notifications,
    refetch: refetchNotifications,
    isLoading,
    error,
  } = useQuery({
    queryKey: ["notifications", { id: userId, markAsChecked: false }],
    queryFn: () => getUserNotifications({ id: userId, markAsChecked: false }),
  });

  const { mutate: tryMarkNotificationsAsRead } = useMutation({
    mutationFn: getUserNotifications,
    onSuccess: (updatedNotifications) => {
      queryClient.invalidateQueries({
        queryKey: ["notifications", { id: userId, markAsChecked: false }],
      });
      dispatch(userActions.updateUserNotifications(updatedNotifications));
    },
    onError: (error) => {
      dispatch(errorActions.setError(error.message));
    },
  });

  useEffect(() => {
    const refetchData = async () => {
      const { data: refetchedNotifications } = await refetchNotifications();
      if (userId && refetchedNotifications) {
        dispatch(userActions.updateUserNotifications(refetchedNotifications));
      }
    };

    const interval = setInterval(() => {
      refetchData();
    }, 120000);

    return () => clearInterval(interval);
  }, [userId, refetchNotifications, dispatch]);

  useEffect(() => {
    const handleBeforeUnload = () => {
      tryMarkNotificationsAsRead({ id: userId, markAsChecked: true });
    };
    window.addEventListener("beforeunload", handleBeforeUnload);

    return () => {
      window.removeEventListener("beforeunload", handleBeforeUnload);
      tryMarkNotificationsAsRead({ id: userId, markAsChecked: true });
    };
  }, [userId, tryMarkNotificationsAsRead]);

  let content;
  if (isLoading) {
    content = <p>Loading ...</p>;
  } else if (error) {
    content = <p>Error!</p>;
  } else if (notifications) {
    content = (
      <ul>
        {notifications.map((notification) => (
          <NotificationComponent
            key={notification.id}
            notification={notification}
          />
        ))}
      </ul>
    );
  }

  return (
    <>
      <header>
        <h2>Notifications</h2>
      </header>
      {content}
    </>
  );
};

export default NotificationsPage;
