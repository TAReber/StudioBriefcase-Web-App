using Azure;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using System.Runtime.CompilerServices;

namespace StudioBriefcase.Startup
{
    public static class ConfigurationSetup
    {

        public static bool VerifyKeyVaultSecrets(this IConfigurationBuilder configuration, string _keyvault, List<string> secretList)
        {
            Uri keyvault = new Uri(_keyvault);
            bool isVerified = true;
            try
            {
                var azureCredentials = new DefaultAzureCredential();
                var client = new SecretClient(keyvault, azureCredentials);

                foreach (string secret in secretList)
                {
                    
                    try
                    {
                        client.GetSecret(secret);
                    }
                    catch (Exception ex)
                    {
                        isVerified = false;
                        Console.WriteLine($"Failed to Access KeyVault\nSuggestion 1: Check for Correct Keyvaul path\nSuggestion 2: If Testing in Development build, Check account in Tools-->Options->Azure Service Authentication in menu:\n{ex.Message}\n");
                    }

                }
                configuration.AddAzureKeyVault(keyvault, new DefaultAzureCredential());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception Thrown While Adding Keyvault: {ex.Message}");
                isVerified = false;
            }

            return isVerified;
        }
    }
}
