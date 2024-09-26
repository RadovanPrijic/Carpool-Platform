import { useMutation, useQuery } from "@tanstack/react-query";
import { useNavigate, useParams } from "react-router";
import {
  deleteReview,
  getReviewsForUser,
} from "../../../services/review-service";
import ReviewComponent from "../components/ReviewComponent";
import { queryClient } from "../../../utils/api-config";
import { useAppDispatch, useAppSelector } from "../../../hooks/store-hooks";
import { errorActions } from "../../../store/error-slice";

const GivenReviewsPage = () => {
  const userId = useAppSelector((state) => state.auth.userId);
  const params = useParams();
  const dispatch = useAppDispatch();
  const navigate = useNavigate();

  const {
    data: givenReviews,
    isLoading,
    error,
  } = useQuery({
    queryKey: ["given-reviews", params.id!, true],
    queryFn: () => getReviewsForUser(params.id!, true),
  });

  const { mutate: tryDeleteReview } = useMutation({
    mutationFn: deleteReview,
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: ["given-reviews", userId, true],
      });
    },
    onError: (error) => {
      dispatch(errorActions.setError(error.message));
    },
  });

  const handleDelete = (id: number) => {
    tryDeleteReview(id);
  };

  const handleNavigation = (id: number) => {
    navigate(`/reviews/edit/${id}`);
  };

  let content;
  if (isLoading) {
    content = <p>Loading ...</p>;
  } else if (error) {
    content = <p>Error!</p>;
  } else if (givenReviews) {
    content = (
      <>
        <ul>
          {givenReviews.map((review) => (
            <ReviewComponent
              key={review.id}
              review={review}
              onEdit={() => handleNavigation(review.id)}
              onDelete={handleDelete}
            />
          ))}
        </ul>
      </>
    );
  }

  return (
    <>
      <header>
        <h2>Given reviews</h2>
      </header>
      {content}
    </>
  );
};

export default GivenReviewsPage;
