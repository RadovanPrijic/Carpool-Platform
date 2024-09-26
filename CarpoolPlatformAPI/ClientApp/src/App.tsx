import "./App.css";
import {
  createBrowserRouter,
  createRoutesFromElements,
  Route,
  RouterProvider,
} from "react-router-dom";
import HomePage from "./pages/Home";
import LoginPage from "./features/authentication/pages/Login";
import { tokenLoader } from "./utils/auth";
import { QueryClientProvider } from "@tanstack/react-query";
import { queryClient } from "./utils/api-config";
import RegistrationPage from "./features/authentication/pages/Registration";
import LogoutPage from "./features/authentication/pages/Logout";
import ProtectedRoute from "./components/ProtectedRoute";
import EditUserPage from "./features/users/pages/EditUser";
import UserProfilePage from "./features/users/pages/UserProfile";
import EmailChangePage from "./features/authentication/pages/EmailChange";
import EmailConfirmationPage from "./features/authentication/pages/EmailConfirmation";
import PasswordResetPage from "./features/authentication/pages/PasswordReset";
import InitiatePasswordResetPage from "./features/authentication/pages/InitiatePasswordReset";
import ProfilePicturePage from "./features/users/pages/ProfilePicture";
import FilteredRidesPage from "./features/rides/pages/FilteredRides";
import UserRidesPage from "./features/rides/pages/UserRides";
import SingleRidePage from "./features/rides/pages/SingleRide";
import EditRidePage from "./features/rides/pages/EditRide";
import NewRidePage from "./features/rides/pages/NewRide";
import GivenReviewsPage from "./features/reviews/pages/GivenReviews";
import ReceivedReviewsPage from "./features/reviews/pages/ReceivedReviews";
import NewReviewPage from "./features/reviews/pages/NewReview";
import EditReviewPage from "./features/reviews/pages/EditReview";
import { loader as reviewLoader } from "./features/reviews/loaders";
import RootLayout from "./pages/RootLayout";
import NotificationsPage from "./features/users/pages/Notifications";
import InboxPage from "./features/messages/pages/Inbox";
import ChatPage from "./features/messages/pages/Chat";
import { loader as inboxLoader } from "./features/messages/loaders";
import BookingsPage from "./features/bookings/pages/Bookings";
import { loader as userLoader } from "./features/users/loaders";
import { loader as rideLoader } from "./features/rides/loaders";

function App() {
  const router = createBrowserRouter(
    createRoutesFromElements(
      <>
        <Route path="/" element={<RootLayout />}>
          <Route path="" element={<HomePage />} loader={tokenLoader} />
          <Route path="login" element={<LoginPage />} />
          <Route path="register" element={<RegistrationPage />} />

          <Route path="user">
            <Route
              path="initiate-password-reset"
              element={<InitiatePasswordResetPage />}
            />
            <Route path="password-reset" element={<PasswordResetPage />} />
            <Route
              path="profile/:id"
              element={<UserProfilePage />}
              loader={userLoader}
            ></Route>
            <Route element={<ProtectedRoute />}>
              <Route path="edit" element={<EditUserPage />} />
              <Route path="profile-picture" element={<ProfilePicturePage />} />
              <Route path="change-email" element={<EmailChangePage />} />
              <Route
                path="email-confirmation"
                element={<EmailConfirmationPage />}
              />
              <Route path="messages">
                <Route
                  path="inbox/:id"
                  element={<InboxPage />}
                  loader={inboxLoader}
                />
                <Route path="chat/:id" element={<ChatPage />} />
              </Route>
              <Route path="notifications" element={<NotificationsPage />} />
              <Route path="logout" element={<LogoutPage />} />
            </Route>
          </Route>

          <Route path="rides">
            <Route path="filtered" element={<FilteredRidesPage />} />
            <Route
              path=":id"
              element={<SingleRidePage />}
              loader={rideLoader}
            />
            <Route element={<ProtectedRoute />}>
              <Route index element={<UserRidesPage />} />
              <Route path="new" element={<NewRidePage />} />
              <Route
                path="edit/:id"
                element={<EditRidePage />}
                loader={rideLoader}
              />
              <Route
                path="review/:id"
                element={<NewReviewPage />}
                loader={rideLoader}
              />
            </Route>
          </Route>

          <Route path="reviews">
            <Route path="received/:id" element={<ReceivedReviewsPage />} />
            <Route element={<ProtectedRoute />}>
              <Route path="given/:id" element={<GivenReviewsPage />} />
              <Route
                path="edit/:id"
                element={<EditReviewPage />}
                loader={reviewLoader}
              />
            </Route>
          </Route>

          <Route path="bookings">
            <Route element={<ProtectedRoute />}>
              <Route path=":id" element={<BookingsPage />} />
            </Route>
          </Route>
        </Route>
      </>
    )
  );

  return (
    <QueryClientProvider client={queryClient}>
      <RouterProvider router={router} />
    </QueryClientProvider>
  );
}

export default App;
