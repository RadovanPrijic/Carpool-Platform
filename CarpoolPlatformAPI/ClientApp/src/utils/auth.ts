import { redirect } from "react-router-dom";
import { JwtPayload } from "jwt-decode";

export interface MyJwtPayload extends JwtPayload {
  nameid: string;
  email: string;
  exp: number;
}

export function getTokenDuration(): number | null {
  const storedExpirationValue = localStorage.getItem("expiration");
  const expirationTime =
    storedExpirationValue !== null ? parseInt(storedExpirationValue) : null;

  if (!expirationTime) {
    return null;
  }

  const currentTime = Math.floor(Date.now() / 1000);
  const duration = expirationTime - currentTime;

  return duration > 0 ? duration * 1000 : 0;
}

export function getAuthToken(): string | null {
  const token = localStorage.getItem("token");
  if (!token) {
    return null;
  }

  const tokenDuration = getTokenDuration();
  if (!tokenDuration) {
    return null;
  }

  if (tokenDuration < 0) {
    return "EXPIRED";
  }

  return token;
}

export function tokenLoader(): string | null {
  const token = getAuthToken();
  return token;
}

export function clearLocalStorage() {
  localStorage.removeItem("token");
  localStorage.removeItem("expiration");
  return redirect("/");
}
