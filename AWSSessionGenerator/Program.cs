using System.Management.Automation;
using System.Text.Json;

class Program
{
    static void Main(string[] args)
    {
        string awsAccessKey = "accesskey";
        string awsSecretKey = "secretkey";
        string mfaSerial = "mfaserial";
        string region = "eu-west-1";
        string outputType = "json";
 
        try
        {
            RunPowershellCommand($"aws configure set aws_access_key_id {awsAccessKey}");
            Console.WriteLine("Access Key Set");
            RunPowershellCommand($"aws configure set aws_secret_access_key {awsSecretKey}");
            Console.WriteLine("Secret Key Set");
            RunPowershellCommand($"aws configure set region {region}");
            Console.WriteLine($"Region Set To {region}");
            RunPowershellCommand($"aws configure set output {outputType}");
            Console.WriteLine($"Output Type Set To {outputType}");
            
            Console.Write("Enter MFA Code: ");
            string? mfaCode = Console.ReadLine();

            while (string.IsNullOrWhiteSpace(mfaCode))
            {
                Console.WriteLine("MFA Code is required.");
                Console.Write("Enter MFA Code: ");
                mfaCode = Console.ReadLine();
            }
            
            Console.WriteLine("Retrieving Session Token...");
            string sessionTokenResult =
                RunPowershellCommand($"aws sts get-session-token --serial-number {mfaSerial} --token-code {mfaCode}");
            Console.WriteLine("Token retrieved.");
            
            //TODO: Set temporary session credentials
            var sessionTokenResponseJson = JsonDocument.Parse(sessionTokenResult);
            string tempAccessKey = sessionTokenResponseJson.RootElement.GetProperty("Credentials").GetProperty("AccessKeyId").GetString();
            string tempSecretKey = sessionTokenResponseJson.RootElement.GetProperty("Credentials")
                .GetProperty("SecretAccessKey").GetString();
            string tempSessionToken = sessionTokenResponseJson.RootElement.GetProperty("Credentials").GetProperty("SessionToken").GetString();

            Console.WriteLine("Setting Temporary Session Credentials...");
            RunPowershellCommand($"aws configure set aws_access_key_id {tempAccessKey}");
            RunPowershellCommand($"aws configure set aws_secret_access_key {tempSecretKey}");
            RunPowershellCommand($"aws configure set aws_session_token {tempSessionToken}");
            Console.WriteLine("Temporary Session Credentials Set Successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An Error has occured: {ex.Message}");
        }
    }

    private static string RunPowershellCommand(string command)
    {
        using (PowerShell powerShell = PowerShell.Create())
        {
            powerShell.AddScript(command);
            var results = powerShell.Invoke();

            if (powerShell.HadErrors)
            {
                foreach (var error in powerShell.Streams.Error)
                {
                    Console.WriteLine(error.ToString());
                }
                throw new Exception("PowerShell Error");
            }

            string output = "";
            foreach (var result in results)
            {
                output += result.ToString() + Environment.NewLine;
            }

            return output;
        }
    }
}

