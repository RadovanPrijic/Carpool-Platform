import { Navigate, Outlet } from "react-router-dom";
import { getAuthToken } from "../utils/auth";

const ProtectedRoute = () => {
  const token = getAuthToken();

  if (token === null || token === "EXPIRED") {
    return <Navigate to="/login" />;
  }

  return <Outlet />;
};

export default ProtectedRoute;
