import { Outlet } from "react-router";
import GlobalErrorModal from "../components/GlobalErrorModal";

const RootLayout = () => {
  return (
    <>
      <Outlet />
      <GlobalErrorModal />
    </>
  );
};

export default RootLayout;
