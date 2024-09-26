import { useRef } from "react";
import Button from "../../../components/Button";
import Modal, { ModalHandle } from "../../../components/Modal";
import { Review } from "../types";

interface ReviewComponentProps {
  review: Review;
  onEdit?: () => void;
  onDelete?: (id: number) => void;
}

const ReviewComponent: React.FC<ReviewComponentProps> = ({
  review,
  onEdit,
  onDelete,
}) => {
  const deleteModalRef = useRef<ModalHandle>(null);

  const openModal = () => {
    deleteModalRef.current!.open();
  };

  const closeModal = () => {
    deleteModalRef.current!.close();
  };

  const handleConfirm = (id: number) => {
    if (onDelete) {
      onDelete(id);
      closeModal();
    }
  };

  return (
    <li>
      {review.id} | {review.comment} | {review.rating} | {review.createdAt}
      {onEdit && <Button label="Edit review" onClick={onEdit} />}
      {onDelete && (
        <>
          <Button label="Delete review" onClick={openModal} />
          <div>
            <Modal
              title="Review deletion"
              ref={deleteModalRef}
              onCancel={closeModal}
              onConfirm={() => handleConfirm(review.id)}
            >
              <p>Are you sure you want to delete this review?</p>
            </Modal>
          </div>
        </>
      )}
    </li>
  );
};

export default ReviewComponent;
