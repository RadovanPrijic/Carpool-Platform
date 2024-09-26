import { LoaderFunctionArgs } from "react-router";
import { queryClient } from "../../utils/api-config";
import { User } from "./types";
import { getUserById } from "../../services/user-service";

export async function loader({ params }: LoaderFunctionArgs) {
  if (!params.id) {
    throw new Error("User ID parameter is required.");
  }
  return queryClient.fetchQuery<User>({
    queryKey: ["user", params.id],
    queryFn: () => getUserById(params.id!),
  });
}
