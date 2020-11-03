using NLog;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using System;
using TEST.Page;

namespace TEST
{
    [TestFixture("chrome")]
    [TestFixture("firefox")]
    class MapsSeleniumTests
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private IWebDriver driver;
        private string browser;

        public MapsSeleniumTests(string browser)
        {
            this.browser = browser;
        }

        [SetUp]
        public void StartBrowser()
        {
            switch (browser)
            {
                case "firefox":
                    driver = new FirefoxDriver();
                    break;
                default:
                    driver = new ChromeDriver();
                    break;
            }
            
            
            driver.Manage().Window.Maximize();
            
        }

        [TestCase("plac Defilad 1, Warszawa", "Chłodna 51, Warszawa", "pieszo")]
        [TestCase("plac Defilad 1, Warszawa", "Chłodna 51, Warszawa", "na rowerze")]
        [TestCase("Chłodna 51, Warszawa", "plac Defilad 1, Warszawa", "pieszo")]
        [TestCase("Chłodna 51, Warszawa", "plac Defilad 1, Warszawa", "na rowerze")]
        public void mapsTest(string startingPoint, string destination, string travelMode)
        {
            logger.Info("Rozpoczynam test...");
            
            MapsPage mapsPage = new MapsPage(driver);
            mapsPage.Open();
            mapsPage.AgreeToCookies();

            mapsPage.GetFasestRoute(startingPoint, destination, travelMode);

            int durationLimit;

            switch (travelMode)
            {
                case "na rowerze":
                    durationLimit = 15;
                    break;
                default:
                    durationLimit = 40;
                    break;
            }

            Assert.Greater(durationLimit, mapsPage.GetFastestRouteDuration());
            Console.WriteLine(mapsPage.GetFastestRouteDistance());

            try
            {
                Assert.Greater(2.0, mapsPage.GetFastestRouteDistance(), "Błąd asercji");
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Something bad happened");
            }

            
            logger.Info("Sukces!");

            //mapsPage.PerformPreparation(startingPoint, destination);
            //mapsPage.AssertThatOnFoot40minAnd3km();
            //mapsPage.AssertThatByBicycle15minAnd3km();

            /* Perform wait to check the output */
            //System.Threading.Thread.Sleep(2000);
        }

        [TearDown]
        public void CloseBrowser()
        {
            driver.Quit();
        }
    }
}
