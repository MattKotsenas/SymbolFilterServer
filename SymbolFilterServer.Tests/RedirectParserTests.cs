using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SymbolFilterServer.Tests
{
    [TestClass]
    public class RedirectParserTests
    {
        private readonly RedirectParser _parser = new RedirectParser();

        [TestMethod]
        public void a_redirect_with_no_protocol_is_invalid()
        {
            var result = _parser.Parse("/symbol.ext/7A01F824C132A37330E0E2F2FCF9C9A11/symbol.ext");
            Assert.IsFalse(result.IsValid);
        }

        [TestMethod]
        public void a_redirect_with_a_protocol_is_valid()
        {
            const string path = "http://symbolserver/symbol.ext/7A01F824C132A37330E0E2F2FCF9C9A11/symbol.ext";
            var result = _parser.Parse(path);

            Assert.IsTrue(result.IsValid);
            Assert.AreEqual(path, result.Redirect);
        }
    }
}
