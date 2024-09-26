import { createSlice, PayloadAction } from "@reduxjs/toolkit";

interface AuthState {
  isAuthenticated: boolean;
  emailConfirmed: boolean;
  userId: string;
}

const initialState: AuthState = {
  isAuthenticated: false,
  emailConfirmed: false,
  userId: "",
};

export const authSlice = createSlice({
  name: "auth",
  initialState,
  reducers: {
    login: (
      state,
      action: PayloadAction<{ emailConfirmationStatus: boolean; id: string }>
    ) => {
      state.isAuthenticated = true;
      state.emailConfirmed = action.payload.emailConfirmationStatus;
      state.userId = action.payload.id;
    },
    logout: (state) => {
      state.isAuthenticated = false;
      state.emailConfirmed = false;
      state.userId = "";
    },
    confirmEmail: (state) => {
      state.emailConfirmed = true;
    },
  },
});

export const authActions = authSlice.actions;

export default authSlice.reducer;
