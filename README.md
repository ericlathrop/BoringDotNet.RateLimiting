BoringDotNet.RateLimiting
===============================
Implementation of the token bucket rate limiting algorithm

This library implements the [token bucket rate limiting algorithm](https://en.wikipedia.org/wiki/Token_bucket). The buckets should be assigned to a unique key, like a session id or user id, and stored in a cache. This library uses [BoringDotNet.Caching](https://github.com/ericlathrop/BoringDotNet.Caching) to abstract the caching.
