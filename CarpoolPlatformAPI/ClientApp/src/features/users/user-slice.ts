import { createSlice, PayloadAction } from "@reduxjs/toolkit";
import { Notification, Picture, User } from "./types";

interface UserState {
  currentUser?: User;
}

const initialState: UserState = {
  currentUser: undefined,
};

export const userSlice = createSlice({
  name: "user",
  initialState,
  reducers: {
    setCurrentUser: (state, action: PayloadAction<User>) => {
      state.currentUser = action.payload;
    },
    clearCurrentUser: (state) => {
      state.currentUser = undefined;
    },
    changeUserEmail: (state, action: PayloadAction<string>) => {
      state.currentUser!.email = action.payload;
    },
    uploadProfilePicture: (state, action: PayloadAction<Picture>) => {
      state.currentUser!.picture = action.payload;
    },
    deleteProfilePicture: (state) => {
      state.currentUser!.picture = undefined;
    },
    updateUserNotifications: (state, action: PayloadAction<Notification[]>) => {
      state.currentUser!.notifications = action.payload;
    },
  },
});

export const userActions = userSlice.actions;

export default userSlice.reducer;
