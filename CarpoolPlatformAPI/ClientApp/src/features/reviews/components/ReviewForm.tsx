import { ChangeEvent } from "react";
import Button from "../../../components/Button";
import Input from "../../../components/Input";
import Textarea from "../../../components/Textarea";
import { useNavigation } from "react-router";
import { ValidationErrorResponse } from "../../../utils/http";

interface ReviewFormProps {
  rating: number;
  comment: string;
  type: "create" | "update";
  onSubmit: (event: React.FormEvent<HTMLFormElement>) => void;
  onChange: (
    event: ChangeEvent<HTMLInputElement | HTMLTextAreaElement>
  ) => void;
  validation?: ValidationErrorResponse | null;
}

const ReviewForm: React.FC<ReviewFormProps> = ({
  rating,
  comment,
  type,
  onSubmit,
  onChange,
  validation,
}) => {
  const navigation = useNavigation();

  return (
    <form onSubmit={onSubmit}>
      <Input
        label="Rating"
        id="rating"
        name="rating"
        type="number"
        value={rating}
        min={1}
        max={5}
        onChange={onChange}
        placeholder="Choose your rating ..."
        required
        validationErrorMessage={validation?.errors.Rating[0]}
      />
      <Textarea
        label="Comment"
        id="comment"
        name="comment"
        value={comment}
        maxLength={1000}
        onChange={onChange}
        placeholder="Enter your comment ..."
        validationErrorMessage={validation?.errors.Comment[0]}
      />
      <Button
        type="submit"
        label={
          navigation.state === "submitting"
            ? "Submitting ..."
            : type === "create"
            ? "Post review"
            : "Edit review"
        }
        disabled={navigation.state === "submitting"}
      />
    </form>
  );
};

export default ReviewForm;
