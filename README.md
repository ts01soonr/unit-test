# Dotnet Unit-Test-github-action
This repository shows an example of dotnet with github action:

    -nunit test
    -mstest
    -xunit
    -chrome browser setup
    -chrome driver setup
    -multi-projects


# Enviroment:
ubuntu-latest, .NET6, Chrome Browser v125 and Chrome Driver v125.0.x
  
    - name: Setup .NET
      uses: actions/setup-dotnet@v1
      with:
        dotnet-version: 6.x

    - name: Setup Chrome-Browser
      uses: browser-actions/setup-chrome@v1
      with:
      # Optional: do not specify to match Chrome's version
        chrome-version: '125'

    - name: Setup Chrome-driver
      uses: nanasess/setup-chromedriver@v2
      with:
      # Optional: do not specify to match Chrome's version
        chromedriver-version: '125.0.6422.14100'

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

# Test Report
![App Screenshot](/img/report.png)

# Socket Channel
![App Screenshot](/img/socket.png)