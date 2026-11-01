# Copilot Review Notes

## Concurrent user updates

- **Asked:** "Review this code for bugs, missing validation, unhandled exceptions and performance issues."
- **Suggested:** The PUT handler could return 404 when another request updates the same existing user between `TryGetValue` and `TryUpdate`.
- **Before:** A failed `TryUpdate` immediately returned 404, conflating a concurrent write with a missing user.
- **After:** The handler retries against the latest value and returns 404 only after the user is no longer present.

## Listing users

- **Asked:** "Review this code for bugs, missing validation, unhandled exceptions and performance issues."
- **Suggested:** Sorting the entire in-memory collection on every GET-all request adds avoidable `O(n log n)` work.
- **Before:** The GET-all handler sorted every user by ID for each request.
- **After:** The handler returns the collection without sorting, reducing the endpoint's work to collection retrieval.
