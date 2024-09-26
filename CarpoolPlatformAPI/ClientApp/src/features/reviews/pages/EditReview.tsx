import { useMutation } from "@tanstack/react-query";
import { updateReview } from "../../../services/review-service";
import { useLoaderData, useParams } from "react-router";
import { Review, ReviewUpdateDTO } from "../types";
import { useState } from "react";
import { useAppDispatch, useAppSelector } from "../../../hooks/store-hooks";
import { queryClient } from "../../../utils/api-config";
import ReviewForm from "../components/ReviewForm";
import { ValidationError, ValidationErrorResponse } from "../../../utils/http";
import { errorActions } from "../../../store/error-slice";

const EditReviewPage = () => {
  const review = useLoaderData() as Review;
  const [formData, setFormData] = useState<ReviewUpdateDTO>({
    rating: review.rating,
    comment: review.comment,
  });
  const [validation, setValidation] = useState<ValidationErrorResponse | null>(
    null
  );
  const userId = useAppSelector((state) => state.auth.userId);
  const dispatch = useAppDispatch();
  const params = useParams();

  const { mutate: tryUpdateReview } = useMutation({
    mutationFn: updateReview,
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: ["received-reviews", userId, false],
      });
      queryClient.invalidateQueries({
        queryKey: ["given-reviews", userId, true],
      });
    },
    onError: (error) => {
      if (error instanceof ValidationError) {
        setValidation(error.validationErrors);
      } else {
        dispatch(errorActions.setError(error.message));
      }
    },
  });

  const handleInputChange = (
    e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>
  ) => {
    const { name, value } = e.target;
    setFormData((prevFormData: ReviewUpdateDTO) => ({
      ...prevFormData,
      [name]: value,
    }));
  };

  const handleUpdateReview = (e: React.FormEvent) => {
    e.preventDefault();
    tryUpdateReview({ id: parseInt(params.id!), reviewUpdateDTO: formData });
  };

  return (
    <ReviewForm
      comment={formData.comment}
      rating={formData.rating}
      type="update"
      onChange={handleInputChange}
      onSubmit={handleUpdateReview}
      validation={validation}
    />
  );
};

export default EditReviewPage;
