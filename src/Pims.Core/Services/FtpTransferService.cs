using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Pims.Core.Configuration;
using Pims.Core.Models;

namespace Pims.Core.Services
{
    public sealed class FtpTransferService
    {
        private readonly AppConfiguration _configuration;

        public FtpTransferService(AppConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task UploadJarAsync(string localFilePath, string targetDirectory, UserCredentials credentials, CancellationToken cancellationToken = default)
        {
            if (!File.Exists(localFilePath))
            {
                throw new FileNotFoundException("Local JAR file not found.", localFilePath);
            }

            var fileName = Path.GetFileName(localFilePath);
            var targetUri = BuildTargetUri(targetDirectory, fileName);

            var request = (FtpWebRequest)WebRequest.Create(targetUri);
            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.Credentials = new NetworkCredential(credentials.UserId, credentials.PrimaryPassword);
            request.UseBinary = true;
            request.UsePassive = true;
            request.EnableSsl = false;

            await using var fileStream = File.OpenRead(localFilePath);
            await using var requestStream = await request.GetRequestStreamAsync().ConfigureAwait(false);
            await fileStream.CopyToAsync(requestStream, 81920, cancellationToken).ConfigureAwait(false);

            using var response = (FtpWebResponse)await request.GetResponseAsync().ConfigureAwait(false);
            if (response.StatusCode != FtpStatusCode.ClosingData)
            {
                throw new InvalidOperationException($"Unexpected FTP response: {response.StatusDescription}");
            }
        }

        private Uri BuildTargetUri(string targetDirectory, string fileName)
        {
            var baseUri = new Uri(_configuration.FtpHost.EndsWith('/') ? _configuration.FtpHost : _configuration.FtpHost + "/");
            var directory = string.IsNullOrWhiteSpace(targetDirectory) ? string.Empty : targetDirectory.Trim('/') + "/";
            return new Uri(baseUri, directory + fileName);
        }
    }
}
