Automates the process of getting a new AWS Session configured. Uses .NET 9 Runtime 

- Setup configurations at the top of the code to match your access keys, region, output type etc
- Can either run from IDE, or build into an exe
- Provide 2FA Code when asked - the rest of the commands should all be automated

This will delete the existing config and credentials files stored in the .aws folder when ran
