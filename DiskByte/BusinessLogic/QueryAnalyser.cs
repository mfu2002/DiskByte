using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace DiskByte.BusinessLogic
{
    class QueryAnalyser
    {
        /// <summary>
        /// Fills the directory info asyncroniously. 
        /// </summary>
        /// <param name="directory">Directory that needs to be filled. </param>
        /// <param name="cancellationToken">token to cancel the operation.</param>
        /// <param name="directoryInfo">DirectoryInfo that holds the data.</param>
        /// <returns></returns>
        public async Task FillInformationAsync(Directory directory, CancellationToken cancellationToken, DirectoryInfo directoryInfo = null)
        {

            //Throw an argument exception if there is not way to figure out the FullPath of the file. 
            if (directoryInfo == null && string.IsNullOrWhiteSpace(directory?.FullPath))
            {
                throw new ArgumentException();
            }


            //Get the directoryinfo from the path from the given directory.
            if (directoryInfo == null)
            {
                directoryInfo = new DirectoryInfo(directory.FullPath);
            }
            else
            {
                //Set the fullpath to the directory from the dirctoryInfo. 
                directory.FullPath = directoryInfo.FullName;
            }

            directory.DirectoryAnalysing = true;
            directory.Name = directoryInfo.Name;
            directory.Attributes = directoryInfo.Attributes;
            directory.LastChanged = directoryInfo.LastWriteTime;


            List<Task> DirRetrivalTasks = new List<Task>();

            try
            {
                var fileListTask = GetFileArray(directoryInfo, cancellationToken);
                var subDirs = directoryInfo.GetDirectories();
                directory.SubDirectories = new Directory[subDirs.Length];

                for (int i = 0; i < subDirs.Length; i++)
                {

                    cancellationToken.ThrowIfCancellationRequested();

                    Directory newChild = new Directory();
                    directory.AddDirectory(newChild, i);
                    DirRetrivalTasks.Add(FillInformationAsync(newChild, cancellationToken, subDirs[i]));
                }

                directory.Files = await fileListTask.ConfigureAwait(false);
                await Task.WhenAll(DirRetrivalTasks);
            }
            catch (UnauthorizedAccessException)
            {

                File accessDeniedFile = new File("Access Denied!",default, default, default, default, default);
                directory.Files = new File[] { accessDeniedFile };

            }



            directory.DirectoryAnalysing = false;

        }


        /// <summary>
        /// Gets the list of files in a directory. 
        /// </summary>
        /// <param name="dirInfo">DirectoryInfo of the parent Directory. </param>
        /// <param name="cancellationToken">Token to cancel the operation. </param>
        /// <returns></returns>
        public Task<File[]> GetFileArray(DirectoryInfo dirInfo, CancellationToken cancellationToken)
        {
            List<Task<File>> FileRetrivalTask = new List<Task<File>>();
            foreach (var subFile in dirInfo.GetFiles())
            {
                cancellationToken.ThrowIfCancellationRequested();
                FileRetrivalTask.Add(GetFileInfo(subFile));
            }
            return Task.WhenAll(FileRetrivalTask);
        }

        /// <summary>
        /// Returns a file object filled with information. 
        /// </summary>
        /// <param name="fileInfo">FileInfo to retrive the data from.</param>
        /// <returns></returns>
        public Task<File> GetFileInfo(FileInfo fileInfo)
        {
            if (fileInfo == null)
            {
                throw new ArgumentNullException();
            }

            return Task.Run(() => new File(
                fileInfo.Name,
                fileInfo.Extension,
                fileInfo.FullName,
                fileInfo.Length,
                fileInfo.LastWriteTime,
                fileInfo.Attributes));
        }

    }

}