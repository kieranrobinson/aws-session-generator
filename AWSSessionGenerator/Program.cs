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
    
        RunPowershellCommand($"aws configure set aws_access_key_id {awsAccessKey}");
        RunPowershellCommand($"aws configure set aws_secret_key {awsSecretKey}");
        RunPowershellCommand($"aws configure set region {region}");
        RunPowershellCommand($"aws configure set output {outputType}");
        
        Console.Write("Enter MFA Code: ");
        string mfaCode = Console.ReadLine();
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

