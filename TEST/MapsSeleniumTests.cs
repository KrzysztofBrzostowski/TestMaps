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


//        class Page {
//        [FindsBy(How = How.XPath, Using = "//*[@id=\"introAgreeButton\"]/span/span")]
//        public IWebElement introAgreeButton { get; set; }
//		  }


//    //[TestFixture(typeof(FirefoxDriver))]
//    //[TestFixture(typeof(ChromeDriver))]
//	class MapsSeleniumTests<TWebDriver> where TWebDriver : IWebDriver, new()

//            var binary = new FirefoxBinary()(@"----Firefox.exe Local Path------");
//            var profile = new FirefoxProfile();
//            FirefoxDriverService service = FirefoxDriverService.CreateDefaultService(@"--GeckoDriver Path-----");
//            service.FirefoxBinaryPath = @"----Firefox.exe Local Path------";
//            driverInstance = new FirefoxDriver(service);

//            //this.driver = new TWebDriver();
//            var env = Environment.CurrentDirectory;
//            var typ = typeof(TWebDriver);
//            string driversPath = Environment.CurrentDirectory + @"\TEST\bin\Debug\";
//            //var instance = Activator.CreateInstance(typeof(TWebDriver), new object[] { driversPath }) as IWebDriver;
			

