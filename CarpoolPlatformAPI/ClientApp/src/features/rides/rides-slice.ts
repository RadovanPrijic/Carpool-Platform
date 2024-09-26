import { createSlice, PayloadAction } from "@reduxjs/toolkit";
import { Ride } from "./types";

interface RidesState {
  rides?: Ride[];
  filteredRides?: Ride[];
}

const initialState: RidesState = {
  rides: undefined,
  filteredRides: undefined,
};

export const ridesSlice = createSlice({
  name: "rides",
  initialState,
  reducers: {
    setRides: (state, action: PayloadAction<Ride[]>) => {
      state.rides = action.payload;
      state.filteredRides = action.payload;
    },
    setFilteredRides: (state, action: PayloadAction<Ride[]>) => {
      state.filteredRides = action.payload;
    },
    clearAllRides: (state) => {
      state.rides = undefined;
      state.filteredRides = undefined;
    },
  },
});

export const ridesActions = ridesSlice.actions;

export default ridesSlice.reducer;
