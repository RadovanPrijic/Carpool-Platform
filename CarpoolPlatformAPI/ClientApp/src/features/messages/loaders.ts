import { LoaderFunctionArgs } from "react-router";
import { queryClient } from "../../utils/api-config";
import { Conversation } from "./types";
import { getAllConversationsForUser } from "../../services/message-service";

export async function loader({ params }: LoaderFunctionArgs) {
  if (!params.id) {
    throw new Error("User ID parameter is required.");
  }
  return queryClient.fetchQuery<Conversation[]>({
    queryKey: ["inbox", params.id],
    queryFn: () => getAllConversationsForUser(params.id!),
  });
}
