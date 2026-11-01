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

## User request validation

- **Asked:** "Add validation. The simplest approach is data annotations plus a helper."
- **Suggested:** Validate required fields, field lengths, and email format before create/update handlers use request data.
- **Before:** Request records had unvalidated positional string properties; empty, oversized, malformed, or missing values could be stored.
- **After:** Create/update requests use required init-only properties with data annotations. An endpoint-filter helper runs `Validator.TryValidateObject` and returns a 400 validation problem response for invalid requests. A runtime test showed `AddValidation()` alone did not reject these payloads in this project, so that approach was rejected. The `.http` file includes an invalid-payload example.

## Unhandled exceptions

- **Asked:** "Review this code for bugs, missing validation, unhandled exceptions and performance issues."
- **Suggested:** Check how malformed JSON and normal in-memory operations fail.
- **Before:** No custom exception handling was present.
- **After:** No handler-level exception path requiring a code change was found; malformed JSON is rejected by ASP.NET Core request binding with HTTP 400. No custom catch block was added.

## Phase 3 middleware and peer-review preparation

- **Asked:** "Create ErrorHandlingMiddleware, TokenAuthMiddleware, and RequestLoggingMiddleware; register them in the specified order; add a README, request examples, notes, test screenshots, and push the changes."
- **Suggested:** Add centralized exception responses and logging, bearer-token authentication, and request/response-status logging.
- **Before:** The API routes were unauthenticated and there was no middleware, run guide, or local token setup.
- **After:** Added the three middleware classes and registered them in the requested order. The API requires a configured `Auth:Token`, compares bearer tokens in constant time, and fails startup when the token is not configured. Added local setup and endpoint documentation in `README.md`; updated `.http` examples with authorization headers and an unauthenticated request. Added a verification report and screenshot in `docs/`. No token is committed to the public repository.
