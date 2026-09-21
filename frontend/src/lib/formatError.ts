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

/**
 * FluentValidation error keys come through as a dotted property path (e.g.
 * "doctorRequest.FirstName" or "Details.PhoneNumber") whose shape varies per
 * command. Match forms against the last path segment, lowercased, so a
 * single lookup works regardless of the wrapping record name.
 */
export function fieldErrors(err: unknown): Record<string, string> {
  if (!(err instanceof ApiError) || !err.problem.errors) {
    return {};
  }
  const map: Record<string, string> = {};
  for (const [key, messages] of Object.entries(err.problem.errors)) {
    const segment = key.split(".").pop() ?? key;
    map[segment.toLowerCase()] = messages[0];
  }
  return map;
}
