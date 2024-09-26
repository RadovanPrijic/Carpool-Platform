import { forwardRef, useEffect, useImperativeHandle, useRef } from "react";
import { createPortal } from "react-dom";
import classes from "./Modal.module.css";
import Button from "./Button";

interface ModalProps {
  title: string;
  children: React.ReactNode;
  onCancel: () => void;
  onConfirm: () => void;
}

export interface ModalHandle {
  open: () => void;
  close: () => void;
}

const Modal = forwardRef<ModalHandle, ModalProps>(
  ({ title, children, onCancel, onConfirm }, ref) => {
    const dialogRef = useRef<HTMLDialogElement | null>(null);

    useImperativeHandle(ref, () => ({
      open: () => {
        if (dialogRef.current) {
          dialogRef.current.showModal();
        }
      },
      close: () => {
        if (dialogRef.current) {
          dialogRef.current.close();
        }
      },
    }));

    useEffect(() => {
      const modal = dialogRef.current;

      return () => {
        if (modal) {
          modal.close();
        }
      };
    }, []);

    return createPortal(
      <dialog className={classes["modal"]} ref={dialogRef} onClose={onCancel}>
        <h3>{title}</h3>
        <div className={classes["modal-content"]}>{children}</div>
        <div className={classes["modal-actions"]}>
          <Button label="Close" onClick={onCancel} />
          <Button label="Confirm" onClick={onConfirm} />
        </div>
      </dialog>,
      document.getElementById("modal") as HTMLElement
    );
  }
);

export default Modal;
