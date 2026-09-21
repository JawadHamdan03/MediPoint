import { ApiError } from "../api/client";

export function formatError(err: unknown): string[] {
  if (err instanceof ApiError) {
    if (err.problem.errors) {
      return Object.values(err.problem.errors).flat();
    }
    return [err.message];
  }
  return ["Something went wrong. Try again."];
}
