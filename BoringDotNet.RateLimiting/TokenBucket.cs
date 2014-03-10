using System;

namespace BoringDotNet.RateLimiting
{
    // Implementation of the Token Bucket rate limiting algorithm
    // http://en.wikipedia.org/wiki/Token_bucket
    [Serializable]
    public class TokenBucket
    {
        private double _tokens;
        private DateTime _lastChecked;

        public TokenBucket(int startingTokens)
            : this(startingTokens, DateTime.Now)
        {
        }

        internal TokenBucket(int startingTokens, DateTime lastChecked)
        {
            _tokens = startingTokens;
            _lastChecked = lastChecked;
        }

        public int TokensLeft(int tokensPerPeriod, TimeSpan period)
        {
            return TokensLeft(tokensPerPeriod, period, DateTime.Now);
        }

        public int TokensLeft(int tokensPerPeriod, TimeSpan period, DateTime now)
        {
            IncrementTokens(tokensPerPeriod, period, now);
            return (int)_tokens;
        }

        private void IncrementTokens(int tokensPerPeriod, TimeSpan period, DateTime now)
        {
            var timePassedMs = now.Subtract(_lastChecked).TotalMilliseconds;
            var tokensToAdd = timePassedMs / period.TotalMilliseconds * tokensPerPeriod;
            _tokens += tokensToAdd;

            var maxTokens = tokensPerPeriod;
            if (_tokens > maxTokens)
                _tokens = maxTokens;

            _lastChecked = now;
        }

        public bool TryTakeToken(int tokensPerPeriod, TimeSpan period)
        {
            return TryTakeToken(tokensPerPeriod, period, DateTime.Now);
        }

        internal bool TryTakeToken(int tokensPerPeriod, TimeSpan period, DateTime now)
        {
            var hasTokens = TokensLeft(tokensPerPeriod, period, now) >= 1;
            if (hasTokens)
            {
                _tokens -= 1;
            }
            return hasTokens;
        }
    }
}
