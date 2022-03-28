# Automated-UI-Testing
Library for easily running automated UI tests in Visual Studio projects through writing test scenario's in JSON.

## Table of Contents
- [Directories](#directories)
- [The input file](#the-input-file)
- [Using library in unit test project](#using-library-in-unit-test-project)
## Directories
Test case files have to be placed to the directory ``` /TestCases ``` below the root folder where the ``` .csproj ``` project file is located.
Blueprints can be added in a subdirectory of this test case using this folder structure; ``` /TestCases/BluePrints ```
***Directory names are case sensitive.***


## The input file
To use the library, a valid input file in JSON format is required with the following properties:
 - ``` testName ``` is the name of the test. It is required to ensure that tests can be distinguished of other tests.
 - ``` osType``` is an enumeration that describes on which platform your testscenario will run. This is metadata that can be used in your test framework. Current supported values are:
	- ``` Windows```
	- ``` MacOS```
	- ``` Linux```
 - ``` webBrowserType``` is an enumeration which describes on which web browser your tests will run. Currently supported values are:
	- ``` Chrome```
	- ``` Edge```
	- ``` FireFox```
	- ``` Safari```
 - ``` driverPath``` is an optional value that describes the URI of the web driver. If the web driver is contained in the path variable, this field can be ignored. Example: ``` C:\\Users\\frankmolengraaf\\ChromeDriver\\chromedriver_win32```
This web driver has to be the same version as your browser.
 - ``` url ``` contains the system under test url.
 - ``` actions ``` is an array that contains the actions that have to be executed in procedural order. There are multiple types of actions that can be used.
	 - Blueprints: Blueprints are used to be able to reuse a set of actions. The blueprint json files consist of:  ``` bluePrintName``` and an array of ``` actions ```.  
	 - Click: This action simulates a mouse click. Required fields:
		 - ``` actionType ``` is an enumeration. Required value for Click actions is ``` Click ```.
		 - ``` locatorType ``` is an enumeration used to describe which locator has to be targeted. Possible values:
			 - ``` ClassName```
			 - ``` Id```
			 - ``` XPath```			 
		 - ``` targetElement ``` contains the 'name' of the target element.
		 Example when using the ``` locatorType``` ``` Id``` ``` targetElement : "username" ```
		 - ```delayInSeconds``` integer value that results in a delay (in seconds) of the specified action.
	  - SendKeys: This action simulates keyboard input in input fields. Required fields: ```SendKeys```
		  - ``` actionType ``` is an enumeration. Required value for Click actions is ``` SendKeys ```.
		 - ``` locatorType ``` is an enumeration used to describe which locator has to be targeted. Possible values:
			 - ``` ClassName```
			 - ``` Id```
			 - ``` XPath```			 
		 - ``` targetElement ``` contains the 'name' of the target element.
		 Example when using the ``` locatorType``` ``` Id``` ``` targetElement : "email" ```
		- ``` data ```  contains the value that has to be filled into the form.
		- ```delayInSeconds``` integer value that results in a delay (in seconds) of the specified action.
- ```finalCondition``` contains the expected final condition.
	- ```locatorType``` is an enumeration used to describe which locator has to be targeted. Possible values:
			 - ``` Url```
			 - ``` ClassName```
			 - ``` Id```
			 - ``` XPath```		
	 - 	 ``` targetElement ``` contains the 'name' of the expected final target element that should be visible. Used when ```locatorType``` is ``` ClassName```, ``` Id``` or ``` XPath``` 
	- ```url``` contains the expected final url. Only used when ```locatorType``` is ```url```
#### Example Template

```json
 {
  "testName": "Log_In_And_Open_Report",
  "osType": "Windows",
  "webBrowserType": "Chrome",
  "url": "https://app.webdashboard.com",
  "actions": [
    {
      "bluePrintName": "LogIn"
    },
    {
      "actionType": "Click",
      "locatorType": "ClassName",
      "targetElement": "report-item",
      "delayInSeconds": "15"
    }
  ],
  "finalCondition": {
    "locatorType": "Url",
    "targetElement": "",
    "url": "https://app.webdashboard.com/en/report"
  }
}
```
#### BluePrint Template Example

```json
{
  "bluePrintName": "LogIn",
  "actions": [
    {
      "actionType": "Click",
      "locatorType": "ClassName",
      "targetElement": "login-button"
    },
    {
      "actionType": "SendKeys",
      "locatorType": "Id",
      "targetElement": "Email",
      "data": "hello@insumma.nl"
    },
    {
      "actionType": "Click",
      "locatorType": "Id",
      "targetElement": "nextbutton"
    },
    {
      "actionType": "Click",
      "locatorType": "Id",
      "targetElement": "loginbutton",
      "delayInSeconds": "5"
    }
  ]
}
```

## Using library in unit test project
To use the library in the unit test project, follow these steps:
1. Create new unit test (project).
2. Import Automated UI Test library.
3. Call the `StartUp.Run` method:
```csharp
var testResult = StartUp.Run(new Uri(templatePath));
```
4. *Optional:* Use `TestExtensions.GetTestCases()` to get TestCaseData.
### Example
```csharp
using InSummaFrontEndAutomatedTesting.Main;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace InSummaWebdashboard.UITests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [TestCaseSource(nameof(GetTestCases))]
        public static void Test(string templatePath)
        {
            var testResult = StartUp.Run(new Uri(templatePath));
            Assert.AreNotEqual(testResult.ActionResults.Count(), 0);
            Assert.AreEqual(testResult.TestCompletedSuccessfully, true);
        }

        private static IEnumerable<TestCaseData> GetTestCases()
        {
            var testCases = TestExtensions.GetTestCases();
            foreach(var testCase in testCases)
            {
                yield return new TestCaseData(testCase.Path).SetArgDisplayNames(testCase.Name);
            }
        }
    }
}
```
