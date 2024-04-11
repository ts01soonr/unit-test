# unit-test-github-action
This repository shows an example of dotnet nunit test , repo generation, selenium setup, test reporter  with github action

# Seleinum Chrome Browser and Chrome Driver

# steps:
  - uses: browser-actions/setup-chrome@v1
    with:
      chrome-version: 123

"# dotnet unit-report" 

    - name: Setup .NET
      uses: actions/setup-dotnet@v1
      with:
        dotnet-version: 6.x

    - name: Setup Chrome-Browser
      uses: browser-actions/setup-chrome@v1
      with:
      # Optional: do not specify to match Chrome's version
        chrome-version: '123'

    - name: Setup Chrome-driver
      uses: nanasess/setup-chromedriver@v2
      with:
      # Optional: do not specify to match Chrome's version
        chromedriver-version: '123.0.6312.122'
