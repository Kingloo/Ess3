using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Ess3.Common;
using Ess3.Extensions;
using Ess3.Model;
using Ess3.Views;

namespace Ess3.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        #region Events
        public event EventHandler<ActivityProgressChangedEventArgs> ActivityProgressChanged;
        private void OnActivityProgressChanged(string title, string percent, double progress)
        {
            var args = new ActivityProgressChangedEventArgs(title, percent, progress);

            ActivityProgressChanged?.Invoke(this, args);
        }
        #endregion

        #region Fields
        private const string appName = "Ess3";
        private readonly Ess3Settings ess3Settings = null;
        private Ess3Bucket selectedBucket = null;
        private const string progressFormat = "0.0";
        #endregion

        #region Properties
        private bool _activity = false;
        public bool Activity
        {
            get => _activity;
            set
            {
                _activity = value;

                RaisePropertyChanged(nameof(Activity));
            }
        }

        private readonly ObservableCollection<Ess3Bucket> _buckets
            = new ObservableCollection<Ess3Bucket>();
        public IReadOnlyCollection<Ess3Bucket> Buckets => _buckets;
        #endregion

        #region Commands
        private DelegateCommandAsync<Ess3Object> _showACLDisplayWindowCommandAsync = null;
        public DelegateCommandAsync<Ess3Object> ShowACLDisplayWindowCommandAsync
        {
            get
            {
                if (_showACLDisplayWindowCommandAsync is null)
                {
                    _showACLDisplayWindowCommandAsync = new DelegateCommandAsync<Ess3Object>(ShowACLDisplayWindowAsync, CanExecute);
                }

                return _showACLDisplayWindowCommandAsync;
            }
        }

        private async Task ShowACLDisplayWindowAsync(Ess3Object obj)
        {
            S3AccessControlList acl = await S3.GetACLAsync(ess3Settings, obj, CancellationToken.None);

            if (acl == null)
            {
                MessageBox.Show(
                    $"Failed to retrieve ACL for {obj.Key}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            else
            {
                ACLDisplayWindow win = new ACLDisplayWindow(acl)
                {
                    WindowStartupLocation = WindowStartupLocation.CenterScreen
                };

                win.ShowDialog();
            }
        }

        private DelegateCommand<Ess3File> _copyPublicUriToClipboardCommand = null;
        public DelegateCommand<Ess3File> CopyPublicUriToClipboardCommand
        {
            get
            {
                if (_copyPublicUriToClipboardCommand is null)
                {
                    _copyPublicUriToClipboardCommand = new DelegateCommand<Ess3File>(CopyPublicUriToClipboard, CanExecute);
                }

                return _copyPublicUriToClipboardCommand;
            }
        }

        private void CopyPublicUriToClipboard(Ess3File file)
        {
            string s3Host = $"s3-{ess3Settings.S3Config.RegionEndpoint.SystemName}.amazonaws.com";

            string s3Path = $"{file.Bucket.BucketName}/{file.Key}";
            
            Uri uri = new UriBuilder
            {
                Scheme = "http",
                Host = s3Host,
                Path = s3Path
            }
            .Uri;

            Clipboard.SetText(uri.AbsoluteUri, TextDataFormat.Text);
        }

        private DelegateCommandAsync _listAllBucketsCommandAsync = null;
        public DelegateCommandAsync ListAllBucketsCommandAsync
        {
            get
            {
                if (_listAllBucketsCommandAsync is null)
                {
                    _listAllBucketsCommandAsync = new DelegateCommandAsync(ListAllBucketsAsync, CanExecute);
                }

                return _listAllBucketsCommandAsync;
            }
        }

        public async Task ListAllBucketsAsync()
        {
            var allBuckets = await S3.ListAllBucketsAsync(ess3Settings);

            _buckets.Clear();

            _buckets.AddRange(allBuckets);

            selectedBucket = Buckets.First();
        }

        public async Task RefreshBucketAsync(Ess3Bucket bucket)
        {
            selectedBucket = bucket ?? throw new ArgumentNullException(nameof(bucket));

            var allEss3Objects = await S3.ListAllObjectsForBucketAsync(
                ess3Settings,
                bucket,
                CancellationToken.None);
            
            bucket.AssembleObjectsIntoTree(allEss3Objects);
        }

        private DelegateCommandAsync<Ess3File> _downloadFileCommandAsync = null;
        public DelegateCommandAsync<Ess3File> DownloadFileCommandAsync
        {
            get
            {
                if (_downloadFileCommandAsync is null)
                {
                    _downloadFileCommandAsync = new DelegateCommandAsync<Ess3File>(DownloadFileAsync, CanExecute);
                }

                return _downloadFileCommandAsync;
            }
        }

        private async Task DownloadFileAsync(Ess3File file)
        {
            if (file is null) { throw new ArgumentNullException(nameof(file)); }

            string downloadDir = GetDownloadDirectory();
            string fileName = GetLocalFileNameFromEss3File(file);

            string localFilePath = Path.Combine(downloadDir, fileName);

            OnActivityProgressChanged(fileName, string.Empty, 0);
            
            Activity = true;

            HttpStatusCode downloadResult = await S3.DownloadFileAsync(ess3Settings, file, localFilePath, CancellationToken.None, DownloadProgress);

            Activity = false;

            OnActivityProgressChanged(string.Empty, string.Empty, 0);

            if (downloadResult != HttpStatusCode.OK)
            {
                MessageBox.Show(
                    $"Downloading {file.Key} failed: {downloadResult.ToString()}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private static string GetDownloadDirectory()
        {
            string userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string folder = "share";
            
            return Path.Combine(userProfile, folder);
        }

        private static string GetLocalFileNameFromEss3File(Ess3File file)
        {
            if (file == null) { throw new ArgumentNullException(nameof(file)); }

            string[] segments = file.Key.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);

            return segments.Last();
        }

        private void DownloadProgress(object sender, WriteObjectProgressArgs e)
        {
            decimal transferred = Convert.ToDecimal(e.TransferredBytes);
            decimal total = Convert.ToDecimal(e.TotalBytes);

            decimal percent = (transferred / total) * 100;

            string percentText = string.Format(CultureInfo.CurrentCulture, "{0} %", percent.ToString(progressFormat));
            double percentDouble = Convert.ToDouble(percent);

            Action updateUI = () => OnActivityProgressChanged(null, percentText, percentDouble);

            updateUI.DispatchSafely(Application.Current.Dispatcher, DispatcherPriority.ApplicationIdle);
        }

        private DelegateCommandAsync<Ess3Directory> _uploadFileCommandAsync = null;
        public DelegateCommandAsync<Ess3Directory> UploadFileCommandAsync
        {
            get
            {
                if (_uploadFileCommandAsync is null)
                {
                    _uploadFileCommandAsync = new DelegateCommandAsync<Ess3Directory>(UploadFileAsync, CanExecute);
                }

                return _uploadFileCommandAsync;
            }
        }

        private async Task UploadFileAsync(Ess3Directory dir)
        {
            var ofd = new System.Windows.Forms.OpenFileDialog
            {
                CheckFileExists = true,
                CheckPathExists = true,
                Multiselect = false,
                Title = "Choose a file to upload..."
            };

            var result = ofd.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                string prefix = (dir != null) ? dir.Key : string.Empty;
                
                OnActivityProgressChanged(ofd.FileName, string.Empty, 0);

                Activity = true;

                HttpStatusCode uploadResult = await S3.UploadFileInPartsAsync(ess3Settings, ofd.FileName, selectedBucket.BucketName, prefix, CancellationToken.None, UploadProgress);

                Activity = false;
                
                if (uploadResult == HttpStatusCode.OK)
                {
                    await RefreshBucketAsync(selectedBucket);
                }
                else
                {
                    MessageBox.Show(
                        $"Upload failed:{Environment.NewLine}{uploadResult.ToString()}",
                        "Failure",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }

            OnActivityProgressChanged(null, string.Empty, 0);
        }

        private void UploadProgress(object sender, UploadProgressArgs e)
        {
            decimal transferred = Convert.ToDecimal(e.TransferredBytes);
            decimal total = Convert.ToDecimal(e.TotalBytes);

            decimal percent = (transferred / total) * 100;

            string percentText = string.Format(CultureInfo.CurrentCulture, "{0} %", percent.ToString(progressFormat));
            double percentDouble = Convert.ToDouble(percent);

            Action updateUI = () => OnActivityProgressChanged(null, percentText, percentDouble);

            updateUI.DispatchSafely(Application.Current.Dispatcher, DispatcherPriority.ApplicationIdle);
        }

        private DelegateCommandAsync<Ess3File> _makeFilePublicReadCommandAsync = null;
        public DelegateCommandAsync<Ess3File> MakeFilePublicReadCommandAsync
        {
            get
            {
                if (_makeFilePublicReadCommandAsync is null)
                {
                    _makeFilePublicReadCommandAsync = new DelegateCommandAsync<Ess3File>(MakeFilePublicReadAsync, CanExecute);
                }

                return _makeFilePublicReadCommandAsync;
            }
        }

        private async Task MakeFilePublicReadAsync(Ess3File file)
        {
            if (file == null) { throw new ArgumentNullException(nameof(file)); }

            S3CannedACL acl = S3CannedACL.PublicRead;

            HttpStatusCode result = await S3.SetACLAsync(ess3Settings, file, acl, CancellationToken.None)
                .ConfigureAwait(false);

            if (result != HttpStatusCode.OK)
            {
                MessageBox.Show(
                    $"Setting {acl.Value} on {file.Key} failed: {result.ToString()}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private DelegateCommandAsync<Ess3File> _makeFilePrivateCommandAsync = null;
        public DelegateCommandAsync<Ess3File> MakeFilePrivateCommandAsync
        {
            get
            {
                if (_makeFilePrivateCommandAsync is null)
                {
                    _makeFilePrivateCommandAsync = new DelegateCommandAsync<Ess3File>(MakeFilePrivateAsync, CanExecute);
                }

                return _makeFilePrivateCommandAsync;
            }
        }

        private async Task MakeFilePrivateAsync(Ess3File file)
        {
            if (file == null) { throw new ArgumentNullException(nameof(file)); }

            S3CannedACL acl = S3CannedACL.Private;

            HttpStatusCode result = await S3.SetACLAsync(ess3Settings, file, acl, CancellationToken.None)
                .ConfigureAwait(false);

            if (result != HttpStatusCode.OK)
            {
                MessageBox.Show(
                    $"Failed to set ACL {acl.Value} on {file.Key}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private DelegateCommandAsync<Ess3Directory> _createDirectoryCommandAsync = null;
        public DelegateCommandAsync<Ess3Directory> CreateDirectoryCommandAsync
        {
            get
            {
                if (_createDirectoryCommandAsync is null)
                {
                    _createDirectoryCommandAsync = new DelegateCommandAsync<Ess3Directory>(CreateDirectoryAsync, CanExecute);
                }

                return _createDirectoryCommandAsync;
            }
        }

        private async Task CreateDirectoryAsync(Ess3Directory directory)
        {
            string prefix = directory != null ? directory.Key : string.Empty;

            CreateDirectoryWindow win = new CreateDirectoryWindow(prefix)
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };

            bool? createDirWinResult = win.ShowDialog();

            if (createDirWinResult.HasValue)
            {
                if (createDirWinResult.Value == true)
                {
                    HttpStatusCode createDirS3Result = HttpStatusCode.NoContent;

                    string fullDirectoryKey = win.DirectoryName;
                    
                    if (directory == null)
                    {
                        createDirS3Result = await CreateDirectoryInBucketRootAsync(fullDirectoryKey);
                    }
                    else
                    {
                        createDirS3Result = await CreateDirectoryInDirectoryAsync(directory, fullDirectoryKey);
                    }

                    if (createDirS3Result == HttpStatusCode.OK)
                    {
                        await RefreshBucketAsync(selectedBucket);
                    }
                    else
                    {
                        MessageBox.Show(
                            $"Creating directory failed: {createDirS3Result.ToString()}",
                            "Failure",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
                    }
                }
            }
        }

        private async Task<HttpStatusCode> CreateDirectoryInBucketRootAsync(string fullDirectoryKey)
        {
            PutObjectRequest req = new PutObjectRequest
            {
                BucketName = selectedBucket.BucketName,
                CannedACL = S3CannedACL.Private,
                Key = fullDirectoryKey,
                StorageClass = S3StorageClass.Standard
            };

            return await S3.CreateDirectoryAsync(ess3Settings, req, CancellationToken.None)
                .ConfigureAwait(false);
        }

        private Task<HttpStatusCode> CreateDirectoryInDirectoryAsync(Ess3Directory dir, string fullDirectoryKey)
        {
            PutObjectRequest req = new PutObjectRequest
            {
                BucketName = dir.Bucket.BucketName,
                CannedACL = S3CannedACL.Private,
                Key = fullDirectoryKey,
                StorageClass = S3StorageClass.Standard
            };

            return S3.CreateDirectoryAsync(ess3Settings, req, CancellationToken.None);
        }

        private DelegateCommandAsync<Ess3Directory> _deleteDirectoryCommandAsync = null;
        public DelegateCommandAsync<Ess3Directory> DeleteDirectoryCommandAsync
        {
            get
            {
                if (_deleteDirectoryCommandAsync is null)
                {
                    _deleteDirectoryCommandAsync = new DelegateCommandAsync<Ess3Directory>(DeleteDirectoryAsync, CanExecute);
                }

                return _deleteDirectoryCommandAsync;
            }
        }

        private async Task DeleteDirectoryAsync(Ess3Directory directory)
        {
            if (directory == null) { throw new ArgumentNullException(nameof(directory)); }

            HttpStatusCode result = HttpStatusCode.Unused;

            bool isDirectoryEmpty = !directory.Ess3Objects.Any();
            
            if (isDirectoryEmpty)
            {
                result = await ActuallyDeleteDirectoryAsync(directory, isDirectoryEmpty);

                if (result == HttpStatusCode.NoContent)
                {
                    // sometimes NoContent means success
                    result = HttpStatusCode.OK;
                }
            }
            else
            {
                var mbr = MessageBox.Show(
                    "Directory is not empty.\r\nDelete anyway?",
                    "Wait",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);
                
                if (mbr == MessageBoxResult.Yes)
                {
                    result = await ActuallyDeleteDirectoryAsync(directory, isDirectoryEmpty);

                    if (result == HttpStatusCode.NoContent)
                    {
                        // sometimes NoContent means success
                        result = HttpStatusCode.OK;
                    }
                }
                else
                {
                    return;
                }
            }
            
            if (result == HttpStatusCode.OK)
            {
                await RefreshBucketAsync(directory.Bucket);
            }
            else
            {
                MessageBox.Show(
                    $"Deletion failed:{Environment.NewLine}{result.ToString()}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private Task<HttpStatusCode> ActuallyDeleteDirectoryAsync(Ess3Directory directory, bool isDirectoryEmpty)
        {
            if (directory == null) { throw new ArgumentNullException(nameof(directory)); }

            return S3.DeleteDirectoryAsync(ess3Settings, directory, isDirectoryEmpty, CancellationToken.None);
        }

        private DelegateCommandAsync<Ess3File> _setS3StorageClassCommandAsync = null;
        public DelegateCommandAsync<Ess3File> SetS3StorageClassCommandAsync
        {
            get
            {
                if (_setS3StorageClassCommandAsync is null)
                {
                    _setS3StorageClassCommandAsync = new DelegateCommandAsync<Ess3File>(SetS3StorageClassAsync, CanExecute);
                }

                return _setS3StorageClassCommandAsync;
            }
        }

        private async Task SetS3StorageClassAsync(Ess3File file)
        {
            if (file is null) { throw new ArgumentNullException(nameof(file)); }

            S3StorageClass newClass;

            if (file.StorageClass == S3StorageClass.Standard)
            {
                newClass = S3StorageClass.OneZoneInfrequentAccess;
            }
            else if (file.StorageClass == S3StorageClass.StandardInfrequentAccess)
            {
                newClass = S3StorageClass.OneZoneInfrequentAccess;
            }
            else
            {
                newClass = S3StorageClass.Standard;
            }

            HttpStatusCode status = await S3.SetS3StorageClassAsync(ess3Settings, file, newClass, CancellationToken.None);

            if (status != HttpStatusCode.OK)
            {
                string message = string.Format(CultureInfo.CurrentCulture, "setting storage class on file ({0}) failed: {1}", file.Key, status.ToString());

                await Log.MessageAsync(message);
            }

            await RefreshBucketAsync(file.Bucket);
        }

        private DelegateCommandAsync<Ess3File> _deleteFileCommandAsync = null;
        public DelegateCommandAsync<Ess3File> DeleteFileCommandAsync
        {
            get
            {
                if (_deleteFileCommandAsync is null)
                {
                    _deleteFileCommandAsync = new DelegateCommandAsync<Ess3File>(DeleteFileAsync, CanExecute);
                }

                return _deleteFileCommandAsync;
            }
        }

        private async Task DeleteFileAsync(Ess3File file)
        {
            if (file == null) { throw new ArgumentNullException(nameof(file)); }
            
            HttpStatusCode result = await S3
                .DeleteFileAsync(ess3Settings, file, CancellationToken.None);

            // a successful deletion returns NoContent, not OK
            if (result == HttpStatusCode.NoContent)
            {
                await RefreshBucketAsync(file.Bucket);
            }
            else
            {
                MessageBox.Show(
                    $"Deleting {file.Key} failed: {result.ToString()}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
        
        private bool CanExecute(object _) => !Activity;
        #endregion

        public MainWindowViewModel(Ess3Settings settings)
        {
            ess3Settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }
        
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(GetType().FullName);
            sb.Append(ess3Settings.ToString());
            sb.AppendLine(string.Format(CultureInfo.CurrentCulture, "Number of buckets: {0}", Buckets.Count));

            return sb.ToString();
        }
    }
}