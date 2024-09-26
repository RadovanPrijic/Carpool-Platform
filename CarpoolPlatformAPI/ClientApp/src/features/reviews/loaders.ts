import { LoaderFunctionArgs } from "react-router";
import { queryClient } from "../../utils/api-config";
import { Review } from "./types";
import { getReviewById } from "../../services/review-service";

export async function loader({ params }: LoaderFunctionArgs) {
  if (!params.id) {
    throw new Error("Review ID parameter is required.");
  }
  return queryClient.fetchQuery<Review>({
    queryKey: ["review", params.id],
    queryFn: () => getReviewById(parseInt(params.id!)),
  });
}
