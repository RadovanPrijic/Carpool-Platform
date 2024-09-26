import { useRef, useState } from "react";
import { useAppDispatch, useAppSelector } from "../../../hooks/store-hooks";
import { useMutation } from "@tanstack/react-query";
import { userActions } from "../user-slice";
import {
  deleteProfilePicture,
  uploadProfilePicture,
} from "../../../services/user-service";
import Button from "../../../components/Button";
import Input from "../../../components/Input";
import { useNavigation } from "react-router";
import { errorActions } from "../../../store/error-slice";
import Modal, { ModalHandle } from "../../../components/Modal";

const ProfilePicturePage = () => {
  const userId = useAppSelector((state) => state.auth.userId);
  const userProfilePicture = useAppSelector(
    (state) => state.user.currentUser!.picture
  );
  const [profilePictureInput, setProfilePictureInput] =
    useState<HTMLInputElement | null>(null);
  const deleteModalRef = useRef<ModalHandle>(null);
  const dispatch = useAppDispatch();
  const navigation = useNavigation();

  const { mutate: tryUploadProfilePicture } = useMutation({
    mutationFn: uploadProfilePicture,
    onSuccess: (picture) => {
      setProfilePictureInput(null);
      dispatch(userActions.uploadProfilePicture(picture));
    },
    onError: (error) => {
      dispatch(errorActions.setError(error.message));
    },
  });

  const { mutate: tryDeleteProfilePicture } = useMutation({
    mutationFn: deleteProfilePicture,
    onSuccess: () => {
      dispatch(userActions.deleteProfilePicture());
    },
    onError: (error) => {
      dispatch(errorActions.setError(error.message));
    },
  });

  const handleInputChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setProfilePictureInput(event.target);
  };

  const handleUploadProfilePicture = (event: React.FormEvent) => {
    event.preventDefault();
    const formData = new FormData();
    formData.append("file", profilePictureInput!.files![0]);
    tryUploadProfilePicture({ file: formData, userId });
  };

  const handleConfirmDelete = () => {
    tryDeleteProfilePicture(userProfilePicture!.id);
    deleteModalRef.current!.close();
  };

  return (
    <>
      <form onSubmit={handleUploadProfilePicture}>
        <Input
          label="Profile picture"
          id="profile-picture"
          name="profilePicture"
          type="file"
          value={profilePictureInput?.value ?? ""}
          onChange={handleInputChange}
          required
        />
        <Button
          type="submit"
          label={
            navigation.state === "submitting"
              ? "Uploading ..."
              : "Upload picture"
          }
          disabled={navigation.state === "submitting"}
        />
      </form>
      {userProfilePicture && (
        <div>
          <Button
            label="Delete picture"
            onClick={() => deleteModalRef.current!.open()}
          />
          <Modal
            title="Profile picture removal"
            ref={deleteModalRef}
            onCancel={() => deleteModalRef.current!.close()}
            onConfirm={handleConfirmDelete}
          >
            <p>Are you sure you want to remove your profile picture?</p>
          </Modal>
        </div>
      )}
      {userProfilePicture && (
        <div>
          <img
            src={userProfilePicture.filePath}
            alt="Profile picture"
            style={{ width: "150px", height: "150px" }}
          />
        </div>
      )}
    </>
  );
};

export default ProfilePicturePage;
