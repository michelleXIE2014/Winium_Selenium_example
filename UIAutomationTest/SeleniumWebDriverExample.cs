namespace Tests
{
    using Amazon;
    using Amazon.EC2;
    using Amazon.EC2.Model;
    using NUnit.Framework;
    using OpenQA.Selenium;
    using OpenQA.Selenium.Chrome;
    using OpenQA.Selenium.Interactions;
    using OpenQA.Selenium.Remote;
    using Renci.SshNet;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Threading;


    public class SeleniumWebDriverExample
    {
        string env = System.Environment.MachineName;
        string currentEnv = "local"; //local, jenkins, teamcity
        string driverType = "remote"; //local, remote selenium driver
        string currentDir = Directory.GetCurrentDirectory();
        //IWebDriver wdriver = null;
        String seleniumNodeIp = "127.0.0.1";
        RemoteWebDriver wdriver;

        [SetUp]
        public void Setup()
        {
            Console.WriteLine("###################Setup start");
          
        }

        [Test, Ignore("Not for Automation Guild 2023")]
        public void LocalDriver()
        {
            wdriver = SeleniumDriver("local");
            wdriver.Navigate().GoToUrl("http://google.com");
            wdriver.Manage().Window.Maximize();

            wdriver.FindElementByCssSelector(".gLFyf.gsfi").SendKeys("Michelle XIE");
            wdriver.FindElementByCssSelector("[name='q']").SendKeys(Keys.Enter);

            Assert.IsTrue(wdriver.FindElementById("resultStats").Displayed);
            
            ScreenShot(wdriver, new StackTrace().GetFrame(0).GetMethod().Name);
            wdriver.Close();
            
        }

        [Test, Ignore("Not for Automation Guild 2023")]
        public void RemoteDriver()
        {
            wdriver = SeleniumDriver("remote");
            wdriver.Navigate().GoToUrl("http://google.com");
            wdriver.Manage().Window.Maximize();

            wdriver.FindElementByCssSelector(".gLFyf.gsfi").SendKeys("Michelle XIE");
            wdriver.FindElementByCssSelector("[name='q']").SendKeys(Keys.Enter);

            Assert.IsTrue(wdriver.FindElementById("resultStats").Displayed);

            ScreenShot(wdriver, new StackTrace().GetFrame(0).GetMethod().Name);
            wdriver.Close();

        }

        private RemoteWebDriver SeleniumDriver(String driverType) {
            RemoteWebDriver wdriver = null;
            Console.WriteLine("###################Test1=======");
            System.Environment.SetEnvironmentVariable("webdriver.chrome.driver", @"c:\chromedriver.exe");

            if (driverType.Equals("local"))
            {
                wdriver = new ChromeDriver();

            }
            else
            {
                ChromeOptions Options = new ChromeOptions();
                Options.PlatformName = "windows";
                // Options.AddArgument("headless");
                Options.AddAdditionalCapability("platform", "WIN10", true);
                Options.AddUserProfilePreference("download.default_directory", "c:\\Users\\support\\Downloads");
                Options.AddUserProfilePreference("download.prompt_for_download", false);
                Options.AddUserProfilePreference("safebrowsing.enabled", true);
                Options.AddUserProfilePreference("download.directory_upgrade", true);
                Options.AddUserProfilePreference("profile.default_content_settings.popups", 0);

                //Supported values: "VISTA" (Windows 7), "WIN8" (Windows 8), "WIN8_1" (windows 8.1), "WIN10" (Windows 10), "LINUX" (Linux)
                Options.AddAdditionalCapability("version", "latest", true);
                // for Chrome only version=latest can be used.
                //Options.AddAdditionalCapability("gridlasticUser", USERNAME, true);
                //Options.AddAdditionalCapability("gridlasticKey", ACCESS_KEY, true);
                //Options.AddAdditionalCapability("video", "True", true);
                wdriver = new RemoteWebDriver(new Uri("http://" + seleniumNodeIp + ":4444/wd/hub/"), Options.ToCapabilities(), TimeSpan.FromSeconds(100));// NOTE: connection timeout of 600 seconds or more required for time to launch grid nodes if non are available.
                //wdriver.Manage().Window.Maximize();
            }

            return wdriver;
        }

        private void ScreenShot(IWebDriver driver, string prefix)
        {
            try
            {
                //Take the screenshot
                Screenshot image = ((ITakesScreenshot)driver).GetScreenshot();
                //Save the screenshot
                string currentDir = Directory.GetCurrentDirectory();
                string screenshotDir = "";
                Console.WriteLine("current directory is " + currentDir);


                if (currentEnv.Equals("teamcity"))
                {
                    string solutionDir = currentDir.Substring(0, currentDir.IndexOf("UIAutomationTest"));
                    Console.WriteLine("screenshot directory is " + solutionDir);
                    screenshotDir = solutionDir + "artifacts\\screenshot\\";
                    Console.WriteLine("screenshot directory is " + screenshotDir);
                }
                else {
                    if (currentDir.Contains("Microsoft Visual Studio"))
                    {
                        currentDir = @"C:\Users\mxie\source\repos\UIAutomationTest\UIAutomationTest\screenshot";
                    }


                    screenshotDir = currentDir + "\\screenshot\\";
                    Console.WriteLine("screenshot directory is " + screenshotDir);
                }
                
                // If directory does not exist, create it. 
                if (!Directory.Exists(screenshotDir))
                {
                    Directory.CreateDirectory(screenshotDir);
                }
                image.SaveAsFile(screenshotDir + prefix + DateTime.Now.ToString("dd-hh-mm-ss") + ".png", OpenQA.Selenium.ScreenshotImageFormat.Png);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Assert.Fail("Failed with Exception: " + e);
            }


        }

    }
}
