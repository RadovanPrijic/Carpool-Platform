interface ErrorResponse {
  message: string;
}

// eslint-disable-next-line @typescript-eslint/no-explicit-any
function isErrorResponse(response: any): response is ErrorResponse {
  return typeof response.message === "string";
}

// ASP.NET Validation Error object
export interface ValidationErrorResponse {
  errors: {
    [key: string]: string[];
  };
  title: string;
  status: number;
  traceId: string;
}

// eslint-disable-next-line @typescript-eslint/no-explicit-any
function isValidationErrorResponse(data: any): data is ValidationErrorResponse {
  return data && data.errors && typeof data.errors === "object";
}

// export class ValidationError extends Error {
//   validationErrors: { [key: string]: string[] };

//   constructor(message: string, validationErrors: { [key: string]: string[] }) {
//     super(message);
//     this.validationErrors = validationErrors;
//   }
// }

// Custom ValidationError class for better error handling
export class ValidationError extends Error {
  validationErrors: ValidationErrorResponse;

  constructor(message: string, validationErrors: ValidationErrorResponse) {
    super(message);
    this.validationErrors = validationErrors;
  }
}

export async function httpRequest<T>(
  url: string,
  options: RequestInit
): Promise<T> {
  try {
    const response = await fetch(url, options);

    if (response.status === 204) {
      return {} as T;
    }

    if (!response.ok) {
      const contentType = response.headers.get("content-type");

      if (contentType && contentType.includes("json")) {
        const errorData = await response.json();

        if (isErrorResponse(errorData)) {
          throw new Error(errorData.message);
        }

        if (isValidationErrorResponse(errorData)) {
          throw new ValidationError("Validation failure.", errorData);
        }
      }
    }

    const result: T = await response.json();
    return result;
  } catch (error) {
    if (error instanceof ValidationError) {
      console.log("Caught validation error(s):", error.validationErrors.errors);
      throw error;
    }

    if (error instanceof Error) {
      console.error("The HTTP request has failed:", error.message);
      throw new Error(error.message);
    }
    throw new Error("An unexpected error has occurred.");
  }
}
