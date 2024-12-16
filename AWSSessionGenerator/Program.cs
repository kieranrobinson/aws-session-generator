using System.Management.Automation;

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
            RunPowershellCommand($"aws configure set aws_secret_key {awsSecretKey}");
            RunPowershellCommand($"aws configure set region {region}");
            RunPowershellCommand($"aws configure set output {outputType}");
            
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
            Console.WriteLine("Setting Temporary Session Credentials...");
            
            //TODO: Set temporary session credentials
            
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

