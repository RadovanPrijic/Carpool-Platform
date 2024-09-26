import {
  Review,
  ReviewCreateDTO,
  ReviewUpdateDTO,
} from "../features/reviews/types";
import { API_ROUTES } from "../utils/api-config";
import { getAuthToken } from "../utils/auth";
import { httpRequest } from "../utils/http";

export async function getReviewsForUser(
  id: string,
  given: boolean
): Promise<Review[]> {
  return httpRequest<Review[]>(
    `${API_ROUTES.REVIEWS}/${given ? "given" : "received"}/${id}`,
    {
      method: "GET",
      headers: {
        "Content-Type": "application/json",
      },
    }
  );
}

export async function getReviewById(id: number): Promise<Review> {
  return httpRequest<Review>(`${API_ROUTES.REVIEWS}/${id}`, {
    method: "GET",
    headers: {
      "Content-Type": "application/json",
    },
  });
}

export async function createReview(
  reviewCreateDTO: ReviewCreateDTO
): Promise<Review> {
  return httpRequest<Review>(`${API_ROUTES.REVIEWS}/`, {
    method: "POST",
    headers: {
      Authorization: `Bearer ${getAuthToken()}`,
      "Content-Type": "application/json",
    },
    body: JSON.stringify(reviewCreateDTO),
  });
}

interface UpdateReviewArgs {
  id: number;
  reviewUpdateDTO: ReviewUpdateDTO;
}

export async function updateReview({
  id,
  reviewUpdateDTO,
}: UpdateReviewArgs): Promise<Review> {
  return httpRequest<Review>(`${API_ROUTES.REVIEWS}/${id}`, {
    method: "PUT",
    headers: {
      Authorization: `Bearer ${getAuthToken()}`,
      "Content-Type": "application/json",
    },
    body: JSON.stringify(reviewUpdateDTO),
  });
}

export async function deleteReview(id: number): Promise<string> {
  await httpRequest<void>(`${API_ROUTES.REVIEWS}/${id}`, {
    method: "DELETE",
    headers: {
      Authorization: `Bearer ${getAuthToken()}`,
    },
  });
  return "Your review has been successfully deleted.";
}
