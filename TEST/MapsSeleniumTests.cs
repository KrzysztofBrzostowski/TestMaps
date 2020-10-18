using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using TEST.Page;

namespace TEST
{
    class MapsSeleniumTests
    {
        private IWebDriver driver;

        [SetUp]
        public void StartBrowser()
        {
            driver = new ChromeDriver();
            driver.Manage().Window.Maximize();
            driver.Navigate().GoToUrl("https://www.google.pl/maps/");
        }

        [Test]
        [TestCase("Plac defilad 1, Warszawa", "Chłodna 51, Warszawa")]
        [TestCase("Chłodna 51, Warszawa", "Plac defilad 1, Warszawa")]
        public void mapsTest(string from, string destination)
        {
            MapsPage mapsPage = new MapsPage(driver);
            mapsPage.PerformPreparation(from, destination);

            mapsPage.AssertThatOnFoot40minAnd3km();
            mapsPage.AssertThatByBicycle15minAnd3km();

            /* Perform wait to check the output */
            System.Threading.Thread.Sleep(2000);
        }

        [TearDown]
        public void CloseBrowser()
        {
            driver.Quit();
        }
    }
}
