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


    public class WiniumOpenNoepadTest
    {


        string env = System.Environment.MachineName;
        string currentEnv = "local"; //local, jenkins, teamcity
        string driverType = "remote"; //local, remote selenium driver
        string currentDir = Directory.GetCurrentDirectory();
        string publicIp;
        string installername;

        [SetUp]
        public void Setup()
        {
            Console.WriteLine("###################Setup start");
            Console.WriteLine("current running in which environment = " + env);
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

            publicIp = "127.0.0.1";
            installername = "notepad.exe";
        }

        [Test]
        public void WiniumOpenNotepadTest()
        {
           
            //if (currentEnv.Equals("local")) {
           //     publicIp = StartInstanceAndGetIP();

            //}
            //StartSeleniumWiniumServer(publicIp);
            WiniumOpenNotepad(publicIp, installername);
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


        private void WiniumOpenNotepad(string publicIp, string filename)
        {
            var dc = new DesiredCapabilities();
            dc.SetCapability("app", @"C:\Windows\system32\" + filename);
            //var driver = new RemoteWebDriver(new Uri("http://localhost:9999"), dc);
            var driver = new RemoteWebDriver(new Uri("http://" + publicIp + ":9999"), dc);

            var windowSetup = driver.FindElementByClassName("Edit");

            Actions action = new Actions(driver);
            ScreenShot(driver);
            action.SendKeys("Hi, I am Michelle.").Build().Perform();
            ScreenShot(driver);
            action.SendKeys(OpenQA.Selenium.Keys.Control + "S").Build().Perform();
            
            ScreenShot(driver);
            action.Release();

            driver.Quit();
        }

        private Boolean ElementFoundByAutomationId(RemoteWebDriver driver, string automationId) {
            Boolean flag = true;
            try {
                driver.FindElementByCssSelector(automationId);
            } catch (NoSuchElementException e)
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
            string screenshotDir = "";
            Console.WriteLine("current directory is " + currentDir);


            if (currentEnv.Equals("teamcity"))
            {
                string solutionDir = currentDir.Substring(0, currentDir.IndexOf("UIAutomationTest"));
                Console.WriteLine("screenshot directory is " + solutionDir);
                screenshotDir = solutionDir + "artifacts\\screenshot\\";
                Console.WriteLine("screenshot directory is " + screenshotDir);
            }
            else
            {
                if (currentDir.Contains("Microsoft Visual Studio"))
                {
                    currentDir = @"C:\Users\mxie\source\repos\Selenium_Winium_example\UIAutomationTest\screenshot";
                }


                screenshotDir = currentDir + "\\screenshot\\";
                Console.WriteLine("screenshot directory is " + screenshotDir);
            }

            // If directory does not exist, create it. 
            if (!Directory.Exists(screenshotDir))
            {
                Directory.CreateDirectory(screenshotDir);
            }

                string screenshotSubDir = screenshotDir + ImageName + "-" + memberName;//new StackTrace().GetFrame(1).GetMethod().Name;
                if (!Directory.Exists(screenshotSubDir)) {
                    Directory.CreateDirectory(screenshotSubDir);
                }

            image.SaveAsFile(screenshotSubDir + "\\" + DateTime.Now.ToString("dd-hh-mm-ss") + ".png", OpenQA.Selenium.ScreenshotImageFormat.Png);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            Assert.Fail("Failed with Exception: " + e);
        }

        }
    }
}
