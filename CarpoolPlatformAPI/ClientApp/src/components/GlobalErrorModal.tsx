import React, { useEffect, useRef } from "react";
import { useAppDispatch, useAppSelector } from "../hooks/store-hooks";
import { errorActions } from "../store/error-slice";
import Button from "./Button";
import classes from "./Modal.module.css";
import { createPortal } from "react-dom";

const GlobalErrorModal: React.FC = () => {
  const errorMessage = useAppSelector((state) => state.error.message);
  const dispatch = useAppDispatch();
  const dialogRef = useRef<HTMLDialogElement | null>(null);

  const handleClose = () => {
    if (dialogRef.current) {
      dialogRef.current.close();
    }
    dispatch(errorActions.clearError());
  };

  useEffect(() => {
    if (errorMessage && dialogRef.current) {
      dialogRef.current.showModal();
    }
  }, [errorMessage]);

  if (!errorMessage) return null;

  return createPortal(
    <dialog className={classes["modal"]} ref={dialogRef} onClose={handleClose}>
      <h3>Woops! An error has occured.</h3>
      <div className={classes["modal-content"]}>
        <p>{errorMessage}</p>
      </div>
      <div className={classes["modal-actions"]}>
        <Button label="Close" onClick={handleClose} />
      </div>
    </dialog>,
    document.getElementById("modal") as HTMLElement
  );
};

export default GlobalErrorModal;
