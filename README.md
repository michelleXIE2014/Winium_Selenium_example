# Winium_Selenium_example
Platform:
- Windows

Programming language:
- C#

Downloads:
- Cygwin installer from https://www.cygwin.com/
- Jenkins: https://jenkins.io/download/
- Java 11(Jenkins requires java 11): https://www.oracle.com/ca-en/java/technologies/javase/jdk11-archive-downloads.html
- NUnit Console: https://nunit.org/download/
- UISpy: https://github.com/2gis/Winium.Cruciatus/tree/master/tools/UISpy


Links:
- Winium: https://github.com/2gis/Winium
- Selenium: https://www.selenium.dev/


To run the tests in Jenkins, you need to

- Start jenkins on Windows by running the following command line in cmd by administrator   
```cd "c:\Program Files\Jenkins"```       
```java -Dhudson.util.ProcessTree.disable=true -jar jenkins.war --httpListenAddress=127.0.0.1 --httpPort=8088 --enable-future-java```  

and

- Start the WinAppDriver on Windows by running the following command line in cmd by administrator   
```c:\winium.Desktop.Driver.exe --verbose```   
