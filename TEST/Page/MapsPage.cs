﻿using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TEST.Model.MapsModel;

namespace TEST.Page
{
    public class MapsPage
    {
        private IWebDriver driver;
        private WebDriverWait wait;
        Int32 timeout = 30000;
        private IWebElement travelModeOnFootButton;
        private IWebElement travelModeByBicycleButton;

        private By cookiesConsentFrame = By.CssSelector(".widget-consent-frame");
        private By cookiesConsentButton = By.CssSelector("#introAgreeButton > span > span");
        private By cookiesConsentWidget = By.ClassName("widget-consent-fullscreen");
        private By routeButton = By.Id("searchbox-directions");
        private By routeStartingPointField = By.CssSelector("#sb_ifc51 > input");
        private By routeDestinationtField = By.CssSelector("#sb_ifc52 > input");
        private By onFootRouteModeButton = By.CssSelector("div[data-travel_mode='2'] > button > img");
        private By byBikeRouteModeButton = By.CssSelector("div[data-travel_mode='1'] > button > img");
        private By routeSearchFirstResult = By.Id("section-directions-trip-0");
        private By routeSearchFirstResultDuration = By.CssSelector(".section-directions-trip-duration");
        private By routeSearchFirstResultDistance = By.CssSelector(".section-directions-trip-distance");

        public MapsPage(IWebDriver driver)
        {
            this.driver = driver;
            wait = new WebDriverWait(driver, TimeSpan.FromMilliseconds(timeout));
        }

        public void Open()
        {
            driver.Navigate().GoToUrl("https://www.google.pl/maps/");
        }

        public void AgreeToCookies()
        {
            IWebElement consentFrame = wait.Until(ExpectedConditions.ElementIsVisible(cookiesConsentFrame));
            driver.SwitchTo().Frame(consentFrame);
            wait.Until(ExpectedConditions.ElementIsVisible(cookiesConsentButton)).Click();
            driver.SwitchTo().DefaultContent();
            wait.Until(ExpectedConditions.InvisibilityOfElementLocated(cookiesConsentWidget));            
        }

        public void GetFasestRoute(string startingPoint, string destination, string travelMode = "pieszo")
        {
            driver.FindElement(routeButton).Click();
            IWebElement routeStartingPointFieldElement = wait.Until(ExpectedConditions.ElementIsVisible(routeStartingPointField));
            routeStartingPointFieldElement.Clear();
            routeStartingPointFieldElement.SendKeys(startingPoint);
            driver.FindElement(routeDestinationtField).SendKeys(destination);

            switch (travelMode)
            {
                case "na rowerze":
                    driver.FindElement(byBikeRouteModeButton).Click();
                    break;
                default:
                    driver.FindElement(onFootRouteModeButton).Click();
                    break;
            }

            wait.Until(ExpectedConditions.ElementIsVisible(routeSearchFirstResult));
        }

        public int GetFastestRouteDuration()
        {
            string firstResultDuration = driver.FindElement(routeSearchFirstResultDuration).Text;
            return int.Parse(firstResultDuration.Remove(firstResultDuration.IndexOf(" ")));
        }

        public Double GetFastestRouteDistance()
        {
            //string firstResultDistance = driver.FindElement(routeSearchFirstResultDistance).Text.Replace(",", ".");
            string firstResultDistance = driver.FindElement(routeSearchFirstResultDistance).Text;
            //firstResultDistance = firstResultDistance.Remove(firstResultDistance.IndexOf(" "));
            //Double dist = Double.Parse(firstResultDistance);
            return float.Parse(firstResultDistance.Remove(firstResultDistance.IndexOf(" ")));

            //return float.Parse(firstResultDistance.Remove(firstResultDistance.IndexOf(" ")));
        }

        public void PerformPreparation(string from, string destination)
        {
            //there is only one frame on page, switch to it
            wait.Until(ExpectedConditions.FrameToBeAvailableAndSwitchToIt(By.TagName("iframe")));
            IWebElement introAgreeButton = driver.FindElement(By.XPath("//*[@id=\"introAgreeButton\"]/span/span"));
            introAgreeButton.Click();

            //Find and click route button
            IWebElement routeButton = driver.FindElement(By.XPath("//*[@id=\"searchbox-directions\"]"));
            routeButton.Click();

            wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//*[@id='sb_ifc51']/input")));
            IWebElement inputFrom = driver.FindElement(By.XPath("//*[@id=\"sb_ifc51\"]/input"));
            inputFrom.Clear();
            inputFrom.SendKeys(from);

            IWebElement inputDestination = driver.FindElement(By.XPath("//*[@id=\"sb_ifc52\"]/input"));
            inputDestination.SendKeys(destination);

            //travel mode enumeration
            //IList type - for easier change to FindsByAttribute pattern, if mandatory
            IList<IWebElement> travelModeItems = driver.FindElements(By.ClassName("directions-travel-mode-icon"));
            var travelModeEnumerator = travelModeItems.GetEnumerator();
            int i = 0;
            while (travelModeEnumerator.MoveNext())
            {
                i++;
                if (i == 4)//"on foot"
                {
                    travelModeOnFootButton = travelModeEnumerator.Current;
                }
                if (i == 5)//"by bicycle"
                {
                    travelModeByBicycleButton = travelModeEnumerator.Current;
                    break;
                }
            }
        }

        public List<PathItems> DurationAndDistCommon()
        {
            wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//*[@id=\"section-directions-trip-0\"]")));

            var durationAndDistanceItems = driver.FindElements(By.ClassName("section-directions-trip-numbers"));
            var durationAndDistanceEnumerator = durationAndDistanceItems.GetEnumerator();

            var durationAndDistList = new List<PathItems>();
            while (durationAndDistanceEnumerator.MoveNext())
            {
                var durationAndDistArray = durationAndDistanceEnumerator.Current.Text.Split('\n');
                var durationStr = durationAndDistArray[0];
                var distanceStr = durationAndDistArray[1];

                var durationDbl = Double.Parse(durationStr.Remove(durationStr.IndexOf(' ')));
                var distanceDbl = Double.Parse(distanceStr.Remove(distanceStr.IndexOf(' ')));

                durationAndDistList.Add(new PathItems() { Duration = durationDbl, Distance = distanceDbl });
            }

            return durationAndDistList;
        }

        public void AssertThatOnFoot40minAnd3km()
        {
            travelModeOnFootButton.Click();
            var durationAndDistanceList = DurationAndDistCommon();

            System.Threading.Thread.Sleep(1000);

            Assert.Greater(40.0, durationAndDistanceList[0].Duration);
            Assert.Greater(3.0, durationAndDistanceList[0].Distance);
        }

        public void AssertThatByBicycle15minAnd3km()
        {
            travelModeByBicycleButton.Click();
            var durationAndDistanceList = DurationAndDistCommon();

            Assert.Greater(15.0, durationAndDistanceList[0].Duration);
            Assert.Greater(3.0, durationAndDistanceList[0].Distance);
        }

    }
}
