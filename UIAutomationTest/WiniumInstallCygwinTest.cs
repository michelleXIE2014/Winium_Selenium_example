namespace Tests
{
    using Amazon;
    using Amazon.EC2;
    using Amazon.EC2.Model;
    using NUnit.Framework;
    using OpenQA.Selenium;
    using OpenQA.Selenium.Interactions;
    using OpenQA.Selenium.Remote;
    using Renci.SshNet;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Threading;


    public class WiniumDemoTest
    {


        public string env = Environment.MachineName;
        public string currentEnv = "local"; //local, jenkins, teamcity
        public string currentDir = Directory.GetCurrentDirectory();
        public string publicIp = "";

        [SetUp]
        public void Setup()
        {
            Console.WriteLine("###################Setup start");
            Console.WriteLine("current running in which environment = " + env);
            publicIp = ConfigurationManager.AppSettings["PublicIp"];
            if (env.Contains("mxie"))
            {
                currentEnv = "local";
                if (currentDir.Contains("jenkins"))
                {
                    currentEnv = "jenkins";
                }
                if (currentDir.Contains("builds"))
                {
                    currentEnv = "teamcity";

                }

            }

            Console.WriteLine("currentEnv = " + currentEnv);
        }

        [Test]
        public void WiniumInstallCygwinTest()
        {

            //if (currentEnv.Equals("local")) {
            //     publicIp = StartInstanceAndGetIP();

            //}
            //StartSeleniumWiniumServer(publicIp);
            //OpenNotepad(publicIp);
            InstallCygwin(publicIp);


        }


        public string StartInstanceAndGetIP()
        {
            string Region = "us-east-2";
            string instanceId = "i-2222222";
            string publicIP = "";
            var client = new AmazonEC2Client(RegionEndpoint.GetBySystemName(Region));

            var response = client.StartInstances(new StartInstancesRequest()
            {
                InstanceIds = new List<string> { instanceId },
            });

            Assert.IsTrue(response.HttpStatusCode == System.Net.HttpStatusCode.OK);

            DescribeInstancesRequest req = new DescribeInstancesRequest();
            List<Amazon.EC2.Model.Reservation> result = client.DescribeInstances(req).Reservations;

            foreach (Reservation reservation_item in result)
            {
                foreach (Instance instance_item in reservation_item.Instances)
                {
                    //Console.Out.WriteLine(instance_item.InstanceId);
                    //Console.Out.WriteLine(instance_item.PrivateIpAddress);
                    //Console.Out.WriteLine(instance_item.PublicIpAddress);
                    if (instance_item.InstanceId.Equals(instanceId))
                    {
                        publicIP = instance_item.PublicIpAddress;

                    }
                }
            }

            Console.Out.WriteLine(publicIP);
            return publicIP;
        }


        private void CheckFileExists(string publicIp, string filename)
        {
            using (var client = new SshClient(publicIp, "", ""))
            {
                client.Connect();
                var filenameFullPath = @"c:\users\mxie\Downloads\" + filename;
                string commandString = "IF EXIST \"" + filenameFullPath + "\" (echo true) else (echo false)";
                Console.Out.WriteLine("commandString=" + commandString);

                var result = client.CreateCommand(commandString).Execute();
                Console.Out.WriteLine("result=" + result);
                client.Disconnect();

            }
        }


        private void StartWiniumServer(string publicIp)
        {
            using (var client = new SshClient(publicIp, "", ""))
            {
                client.Connect();
                var result = client.CreateCommand(@"start /B c:\Winium.Desktop.Driver.exe --verbose > c:\users\support\winium.log").Execute();
                Console.Out.WriteLine("result=" + result);
                client.Disconnect();

            }
        }


        private void OpenNotepad(string publicIp)
        {
            var dc = new DesiredCapabilities();
            dc.SetCapability("app", @"C:\Windows\system32\notepad.exe");
            var driver = new RemoteWebDriver(new Uri("http://" + publicIp + ":9999"), dc);

            var windowSetup = driver.FindElementByClassName("Edit");

            Actions action = new Actions(driver);
            ScreenShot(driver);
            Thread.Sleep(10000);
            action.SendKeys("Hi, I am Michelle. I am going to install cygwin").Build().Perform();
            ScreenShot(driver);
            action.SendKeys(Keys.Control + "S").Build().Perform();
            ScreenShot(driver);
            action.Release();
            driver.Quit();
        }

        private void InstallCygwin(string publicIp)
        {

            const string AppName = @"c:\cygwin-setup-x86_64.exe";
            const string InstallPath = @"C:\cygwin64";
            //Winium
            var dc = new DesiredCapabilities();
            dc.SetCapability("app", AppName);
            var driver = new RemoteWebDriver(new Uri("http://" + publicIp + ":9999"), dc);

            Actions action = new Actions(driver);
            var windowSetup = driver.FindElementByName("Cygwin Setup");
            ScreenShot(driver);
            action.SendKeys(Keys.Alt + "N").Build().Perform();
            //windowSetup.FindElement(By.Name("Next >")).Click();  //not working
            //windowSetup.SendKeys(Keys.Alt + "N");

            Assert.IsNotNull(driver.FindElementsByName("Cygwin Setup - Choose Installation Type"));
            ScreenShot(driver, currentEnv);
            windowSetup = driver.FindElementByName("Cygwin Setup - Choose Installation Type");
            //action.SendKeys(Keys.Alt + "N").Build().Perform();

            Assert.IsNotNull(driver.FindElementsByName("Cygwin Setup - Choose Installation Directory"));
            windowSetup = driver.FindElementByName("Cygwin Setup - Choose Installation Directory");
            ScreenShot(driver, currentEnv);
            action.SendKeys(InstallPath).Build().Perform();
            action.SendKeys(Keys.Alt + "N").Build().Perform();

            Assert.IsNotNull(driver.FindElementsByName("Cygwin Setup - Select Local Package Directory"));
            windowSetup = driver.FindElementByName("Cygwin Setup - Select Local Package Directory");
            ScreenShot(driver, currentEnv);
            action.SendKeys(InstallPath).Build().Perform();
            action.SendKeys(Keys.Alt + "N").Build().Perform();

            Assert.IsNotNull(driver.FindElementsByName("Cygwin Setup - Select Connection Type"));
            ScreenShot(driver, currentEnv);
            action.SendKeys(Keys.Alt + "N").Build().Perform();

            Assert.IsNotNull(driver.FindElementsByName("Cygwin Setup - Choose Download Site(s)"));
            ScreenShot(driver, currentEnv);
            action.SendKeys(Keys.Alt + "N").Build().Perform();

            Assert.IsNotNull(driver.FindElementsByName("Cygwin Setup - Select Packages"));
            ScreenShot(driver, currentEnv);
            action.SendKeys(Keys.Alt + "N").Build().Perform();

            Assert.IsNotNull(driver.FindElementsByName("Cygwin Setup - Review and confirm changes"));
            ScreenShot(driver, currentEnv);
            action.SendKeys(Keys.Alt + "N").Build().Perform();

            Assert.IsNotNull(driver.FindElementsByName("Cygwin Setup - Installation Status and Create Icons"));
            ScreenShot(driver, currentEnv);

            //driver.FindElementByName("Finish").Click(); not working
            //driver.FindElementByCssSelector("[AutomationID = '12325']").Click(); not implemented
            action.SendKeys(Keys.Alt + "C").Build().Perform();
            action.SendKeys(Keys.Enter).Build().Perform();
            action.Release();

            if (driver != null)
            {
                driver.Close();
                driver.Quit();
                
            }
            driver = null;

        }

        private Boolean ElementFoundByAutomationId(RemoteWebDriver driver, string automationId)
        {
            Boolean flag = true;
            try
            {
                driver.FindElementByCssSelector(automationId);
            }
            catch (NoSuchElementException e)
            {
                flag = false;
            }
            return flag;
        }
        private void ScreenShot(IWebDriver driver, [System.Runtime.CompilerServices.CallerMemberName] string memberName = "")
        {

            try
            {
                string ImageName = "Win10 pro";
                //Take the screenshot
                Screenshot image = ((ITakesScreenshot)driver).GetScreenshot();
                //Save the screenshot
                string currentDir = Directory.GetCurrentDirectory();
                string screenshotDir = "Screenshot";
                Console.WriteLine("current directory is " + currentDir);


                if (currentEnv.Equals("teamcity"))
                {
                    string solutionDir = currentDir.Substring(0, currentDir.IndexOf("UIAutomationTest"));
                    Console.WriteLine("screenshot directory is " + solutionDir);
                    screenshotDir = solutionDir + "artifacts\\screenshot\\";
                    Console.WriteLine("screenshot directory is " + screenshotDir);
                }

                // If directory does not exist, create it. 
                if (!Directory.Exists(screenshotDir))
                {
                    Directory.CreateDirectory(screenshotDir);
                }

                string screenshotSubDir = screenshotDir + ImageName + "-" + memberName;//new StackTrace().GetFrame(1).GetMethod().Name;
                if (!Directory.Exists(screenshotSubDir))
                {
                    Directory.CreateDirectory(screenshotSubDir);
                }

                image.SaveAsFile(screenshotSubDir + "\\" + DateTime.Now.ToString("dd-hh-mm-ss") + ".png", ScreenshotImageFormat.Png);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Assert.Fail("Failed with Exception: " + e);
            }

        }
    }
}
