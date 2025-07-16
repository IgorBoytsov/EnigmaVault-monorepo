namespace EnigmaVault.SecretService.Domain
{
    public static class Guard
    {
        public static void Against(bool condition, Func<Exception> exceptionFactory)
        {
            if (condition)
            {
                throw exceptionFactory();
            }
        }
    }
}