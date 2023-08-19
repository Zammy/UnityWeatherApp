using NUnit.Framework;

namespace Toastylib.Tests
{
    [TestFixture]
    public static class TestFixture
    {
        [Test]
        public static void CorrectTextIsSetToToastMessage()
        {
            var msg = "Message";

            Toasty.DisplayToastMessage(msg);

            Assert.AreEqual(Toasty.sToastClass.ToastText, msg);
        }
    }
}
