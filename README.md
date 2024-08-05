# Dotnet Unit-Test-github-action
This repository shows an example of dotnet with github action:

    -nunit test
    -mstest
    -xunit
    -chrome browser setup
    -chrome driver setup
    -multi-projects
	-mutation demo
	-SwaggerAPI


# Enviroment:
ubuntu-latest, .NET6, Chrome Browser v127 and Chrome Driver v127
  
    - name: Setup .NET
      uses: actions/setup-dotnet@v1
      with:
        dotnet-version: 6.x

    - name: Setup Chrome-Browser
      uses: browser-actions/setup-chrome@v1
      with:
      # Optional: do not specify to match Chrome's version
        chrome-version: '127'

    - name: Setup Chrome-driver
      uses: nanasess/setup-chromedriver@v2
      with:
      # Optional: do not specify to match Chrome's version
        chromedriver-version: '127'

    - name: stryker-mutator .NET
      uses: stryker-mutator/github-action@v0.0.1
      with:
        testProject: "Solution/TestPalindrome/" # required
        breakAt: "75" # Optional

    - name: Stryker-Html-Report
      uses: actions/upload-artifact@v3
      with:
        name: html-report
        path: ${{github.workspace}}/Solution/TestPalindrome/StrykerOutput/**/**/*.html	

# Update Permission for report (dotnet-trx):

    permissions:
      issues: write
      contents: write
      actions: write
      checks: write
    refer: https://sjramblings.io/github-actions-resource-not-accessible-by-integration
      
# Combine nUnit and MS Test together 

    <PackageReference Include="NUnit" Version="3.13.2" />
    <PackageReference Include="NUnit3TestAdapter" Version="4.0.0" />
    <PackageReference Include="MSTest.TestAdapter" Version="2.2.7" />
    <PackageReference Include="MSTest.TestFramework" Version="2.2.7" />

# Seperate Test and TestLibrary
  
    - TestLibrary
    - UnitTestProject
    - SocketDemo
    - RestSharpAPI
    - AppiumTest
	- Palindrome (Mutation via stryker.Net )
	- TestPalindrome
	- EntityFrameworkCore.MySQL

# Mutation Test Report
![App Screenshot](/img/stryker.Net.png)

# Test Report
![App Screenshot](/img/report.png)

# Socket Channel
![App Screenshot](/img/socket.png)