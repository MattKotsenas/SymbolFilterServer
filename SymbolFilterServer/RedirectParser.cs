namespace SymbolFilterServer
{
    public class RedirectParser
    {
        public RedirectResult Parse(string path)
        {
            var request = path.TrimStart('/');

            // we're looking for the server that we are supposed to use for this request in our path
            // the point of this is to make it so that you can set your symbol path to include
            // http://localhost:8080/http://your-sym-server/whatever
            // and your-sym-server will be used by the proxy
            if (!request.ToLowerInvariant().StartsWith("http://") && !request.ToLowerInvariant().StartsWith("https://"))
            {
                return new RedirectResult(false, null);
            }
            return new RedirectResult(true, request);
        }
    }

    public struct RedirectResult
    {
        public RedirectResult(bool isValid, string redirect)
        {
            IsValid = isValid;
            Redirect = redirect;
        }

        public bool IsValid { get; }
        public string Redirect { get; }
    }
}
