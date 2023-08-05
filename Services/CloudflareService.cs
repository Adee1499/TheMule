using Amazon.Runtime.CredentialManagement;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

namespace TheMule.Services
{
    public static class CloudflareService
    {
        private static RestClient? _client;

        private static string? _publicUrl;

        internal static async Task<string> UploadFile(string filePath, string fileName) {
            _publicUrl = SettingsManager.appSettings.Cloudflare.PublicUrl;

            // AWS credentials setup
            var options = new CredentialProfileOptions {
                AccessKey = SettingsManager.appSettings.Cloudflare.Access_Key,
                SecretKey = SettingsManager.appSettings.Cloudflare.Secret_Key
            };

            var profile = new CredentialProfile("default", options);
            var sharedFile = new SharedCredentialsFile();
            sharedFile.RegisterProfile(profile);

            // Create the RestSharp client
            string baseUrl = "https://28093bfdc7a0c50ad7147518bf3b319b.r2.cloudflarestorage.com/";
            _client = new RestClient(baseUrl);

            byte[] imageData = File.ReadAllBytes(filePath);

            // precompute hash of the body content
            var contentHash = AWS4SignerBase.CanonicalRequestHashAlgorithm.ComputeHash(imageData);
            var contentHashString = AWS4SignerBase.ToHexString(contentHash, true);

            var request = new RestRequest($"pod-library/{fileName}", Method.Put);
            request.AddHeader("content-type", "image/png");
            request.AddHeader(AWS4SignerBase.X_Amz_Content_SHA256, contentHashString);
            request.AddHeader("content-length", imageData.Length.ToString());

            var requestDateTime = DateTime.UtcNow;
            var dateTimeStamp = requestDateTime.ToString(AWS4SignerBase.ISO8601BasicFormat, CultureInfo.InvariantCulture);

            request.AddHeader(AWS4SignerBase.X_Amz_Date, dateTimeStamp);

            var headers = new Dictionary<string, string>
            {
                { AWS4SignerBase.X_Amz_Content_SHA256, contentHashString },
                { "content-type", "image/png" },
                { "content-length", imageData.Length.ToString() }
            };

            var signer = new AWS4SignerForAuthorizationHeader {
                EndpointUri = new Uri(baseUrl + request.Resource),
                HttpMethod = "PUT",
                Service = "s3",
                Region = "auto"
            };

            var authorization = signer.ComputeSignature(headers, "", contentHashString, options.AccessKey, options.SecretKey);

            // place the computed signature into a formatted 'Authorization' header and call S3
            request.AddHeader("Authorization", authorization);

            request.AddParameter("image/png", imageData, ParameterType.RequestBody);
            
            var response = await _client.ExecuteAsync(request);

            if (response.StatusCode == System.Net.HttpStatusCode.OK) {
                return _publicUrl + fileName;
            } else {
                return "Error! " + response.ErrorMessage;
            }
        }
    }
}
