# Copilot Review Notes

## Concurrent user updates

- **Asked:** "Review this code for bugs, missing validation, unhandled exceptions and performance issues."
- **Suggested:** The PUT handler could return 404 when another request updates the same existing user between `TryGetValue` and `TryUpdate`.
- **Before:** A failed `TryUpdate` immediately returned 404, conflating a concurrent write with a missing user.
- **After:** The handler retries against the latest value and returns 404 only after the user is no longer present.
