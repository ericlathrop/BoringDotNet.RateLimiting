using System;

namespace BoringDotNet.RateLimiting
{
    public interface ITokenBucketRepository
    {
        TokenBucket FindByNameAndPeriod(string name, int tokensPerPeriod, TimeSpan period);
        void Save(string name, TimeSpan period, TokenBucket bucket);
    }
}