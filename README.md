# Dotnet NUnit-test-github-action
This repository shows an example of dotnet nunit test , repo generation, selenium setup, test reporter  with github action
@fang

# Seleinum Chrome Browser and Chrome Driver

# Update Permission for report:
  permissions:
      issues: write
      contents: write
      actions: write
      checks: write

"# dotnet unit-report" 

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

# Update Permission for report:
https://sjramblings.io/github-actions-resource-not-accessible-by-integration
    permissions:
      issues: write
      contents: write
      actions: write
      checks: write
      
# Combine nUnit and MS Test together 

    <PackageReference Include="NUnit" Version="3.13.2" />
    <PackageReference Include="NUnit3TestAdapter" Version="4.0.0" />
    <PackageReference Include="MSTest.TestAdapter" Version="2.2.7" />
    <PackageReference Include="MSTest.TestFramework" Version="2.2.7" />

# Seperate Test and TestLibrary
  
    - TestLibrary
    - UnitTestProject

