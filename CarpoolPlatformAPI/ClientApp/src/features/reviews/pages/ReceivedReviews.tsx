import { useQuery } from "@tanstack/react-query";
import { useParams } from "react-router";
import { getReviewsForUser } from "../../../services/review-service";
import ReviewComponent from "../components/ReviewComponent";

const ReceivedReviewsPage = () => {
  const params = useParams();

  const {
    data: receivedReviews,
    isLoading,
    error,
  } = useQuery({
    queryKey: ["received-reviews", params.id!, false],
    queryFn: () => getReviewsForUser(params.id!, false),
  });

  let content;
  if (isLoading) {
    content = <p>Loading ...</p>;
  } else if (error) {
    content = <p>Error!</p>;
  } else if (receivedReviews) {
    content = (
      <>
        <ul>
          {receivedReviews.map((review) => (
            <ReviewComponent key={review.id} review={review} />
          ))}
        </ul>
      </>
    );
  }

  return (
    <>
      <header>
        <h2>Received reviews</h2>
      </header>
      {content}
    </>
  );
};

export default ReceivedReviewsPage;
