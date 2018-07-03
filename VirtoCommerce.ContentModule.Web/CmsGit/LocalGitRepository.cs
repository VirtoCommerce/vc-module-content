using LibGit2Sharp;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace VirtoCommerce.ContentModule.Web.CmsGit
{
    public class LocalGitRepository
    {
        //private string _repositoryPath = @"D:\VC\SOLUTIONS\vc-storefront-core\VirtoCommerce.Storefront\wwwroot\cms-content\Git";
        private string _repositoryPath = String.Empty; //@"D:\VC\SOLUTIONS\vc-storefront-core-json\VirtoCommerce.Storefront\wwwroot\cms-content\git";

        private string _tempRepositoryName = "temp";
        private string _originalRepositoryName = "original";

        private string _branchDraftName = "draft";
        public string BranchDraftName
        {
            get
            {
                return _branchDraftName;
            }
        }

        private string _branchMasterName = "master";
        public string BranchMasterName
        {
            get
            {
                return _branchMasterName;
            }
        }

        private Signature _userMaster = null;

        const string _msgRepositoryWasCreated = "The origin repository has been created";
        const string _msgSavingFile = "Autosaving the file";

        public LocalGitRepository()
        {
            this._userMaster = new Signature("Master", "master@vc.com", DateTime.Now);

            this._repositoryPath = System.Web.Configuration.WebConfigurationManager.AppSettings["GitRepositoryPath"];

            CreateOriginalRepository();
        }


        public void SetFile(string userName, string fileName, string content)
        {
            Signature user = new Signature(userName, userName, DateTime.Now);

            var branchName = userName.ToLower();

            CheckBranchFolder(branchName, user);

            UpdateBranch(branchName, user, _branchDraftName);

            var workingPath = _repositoryPath + @"\" + branchName;
            var filePath = workingPath + @"\" + fileName;

            using (var repo = new Repository(workingPath))
            {
                var branch = GetForceBranch(repo, user, branchName);

                // Saving the file
                File.WriteAllText(filePath, content);
            }
        }

        public string GetFile(string userName, string fileName)
        {
            Signature user = new Signature(userName, userName, DateTime.Now);

            var branchName = userName.ToLower();

            CheckBranchFolder(branchName, user);

            UpdateBranch(branchName, user, _branchDraftName);

            var workingPath = _repositoryPath + @"\" + branchName;
            var filePath = workingPath + @"\" + fileName;

            string retVal = String.Empty;

            using (var repo = new Repository(workingPath))
            {
                var branch = GetForceBranch(repo, user, branchName);

                if (File.Exists(filePath))
                {
                    // Getting the file
                    retVal = File.ReadAllText(filePath);
                }
            }

            return retVal;
        }

        public void MoveTo(string userName, string fileName, string toBranchName)
        {
            Signature user = new Signature(userName, userName, DateTime.Now);

            var branchName = userName.ToLower();

            CheckBranchFolder(branchName, user);

            var workingPath = _repositoryPath + @"\" + branchName;
            var filePath = workingPath + @"\" + fileName;

            using (var repo = new Repository(workingPath))
            {
                Pull(repo, branchName, user);

                // Commit this file only
                Commit(repo, userName, fileName);

                Push(repo, repo.Branches[branchName]);

                // Merge and Push into branch "to"
                MergeAndPush(repo, user, branchName, toBranchName);
            }
        }

        public void Update(string branchName)
        {
            var workingPath = _repositoryPath + @"\" + branchName;

            using (var repo = new Repository(workingPath))
            {
                try
                {
                    var branch = repo.Branches[branchName];
                    branch = Commands.Checkout(repo, branch);

                    Remote remote = repo.Network.Remotes["origin"];

                    repo.Branches.Update(branch,
                        b => b.Remote = remote.Name,
                        b => b.UpstreamBranch = branch.CanonicalName);

                    Commands.Pull(repo, _userMaster, new PullOptions() { MergeOptions = new MergeOptions() { FastForwardStrategy = FastForwardStrategy.FastForwardOnly /*, FileConflictStrategy = CheckoutFileConflictStrategy.Theirs*/ } });
                }
                catch { }
            }

        }

        public bool FileExists(string userName, string fileName)
        {
            return GetFile(userName, fileName) != String.Empty;
        }

        public object PermalinkUnique(string userName, string fileName, string permalink)
        {
            var workingPath = _repositoryPath + @"\" + userName;

            string[] files = Directory.GetFiles(workingPath, "*.json", SearchOption.AllDirectories);

            bool retVal = true;

            foreach (string file in files)
            {
                if (!fileName.Equals(Path.GetFileName(file)))
                {
                    var json = File.ReadAllText(file);

                    JArray blocks = (JArray)JsonConvert.DeserializeObject(json);

                    foreach (var block in blocks)
                    {
                        if (block["type"].ToString() == "settings" && block["permalink"] != null)
                        {
                            var exists = block["permalink"].ToString() == permalink;
                            if (exists)
                            {
                                retVal = false;
                                break;
                            }
                        }
                    }
                }

                if (!retVal)
                {
                    break;
                }
            }

            return retVal;
        }


        #region Preparing

        private void CreateOriginalRepository()
        {
            var tempPath = _repositoryPath + @"\" + _tempRepositoryName;
            var originalPath = _repositoryPath + @"\" + _originalRepositoryName;

            // Creating a temp repository to create an original one
            if (!Repository.IsValid(tempPath))
            {
                var repoPath = Repository.Init(tempPath);
                using (var repo = new Repository(repoPath))
                {
                    repo.Commit("", _userMaster, _userMaster, new CommitOptions() { AllowEmptyCommit = true });
                }
            }

            // Creating a temp repository to create an original one
            if (!Repository.IsValid(originalPath))
            {
                Repository.Clone(tempPath, originalPath, new CloneOptions() { IsBare = true });
            }

            CheckBranchFolder(_branchDraftName, _userMaster);

            CheckBranchFolder(_branchMasterName, _userMaster);

        }

        #endregion Preparing

        #region Support functions 

        private void CheckBranchFolder(string branchName, Signature user)
        {
            var workingPath = _repositoryPath + @"\" + branchName;
            var sourcePath = _repositoryPath + @"\" + _originalRepositoryName;
            var repoPath = workingPath;

            if (!Repository.IsValid(workingPath))
            {
                repoPath = Repository.Clone(sourcePath, workingPath);
            }

            using (var repo = new Repository(repoPath))
            {
                GetForceBranch(repo, user, branchName);
            }
        }

        private Branch GetForceBranch(Repository repo, Signature user, string branchName)
        {
            // Each branch is a separated git-folder which is connected to original one

            Branch retVal = null;

            var workingPath = _repositoryPath + @"\" + branchName;

            retVal = repo.Branches[branchName];

            if (retVal == null)
            {
                // Checkout master-branch

                var branch = repo.Branches[_branchMasterName];
                Branch currentBranch = Commands.Checkout(repo, branch);

                if (branchName != _branchDraftName)
                {
                    // Create a new branch
                    var branchDraft = repo.CreateBranch(_branchDraftName);
                    // Checkout the new branch
                    branchDraft = Commands.Checkout(repo, branchDraft);

                    Remote remote = repo.Network.Remotes["origin"];

                    repo.Branches.Update(branchDraft,
                        b => b.Remote = remote.Name,
                        b => b.UpstreamBranch = branchDraft.CanonicalName);

                    Commands.Pull(repo, user, new PullOptions() { MergeOptions = new MergeOptions() { FastForwardStrategy = FastForwardStrategy.FastForwardOnly /*, FileConflictStrategy = CheckoutFileConflictStrategy.Theirs*/ } });
                }

                // Create a new branch
                retVal = repo.CreateBranch(branchName);
                // Checkout the new branch
                retVal = Commands.Checkout(repo, retVal);
                // Commit this file only
                repo.Commit("", user, user, new CommitOptions() { AllowEmptyCommit = true });
                // Push it to origin
                Push(repo, retVal);


                //Remote remote = repo.Network.Remotes["origin"];

                //Branch branchHead = repo.Head;
                //Branch updatedBranch = repo.Branches.Update(branchHead,
                //    b => b.Remote = remote.Name,
                //    b => b.UpstreamBranch = branchHead.CanonicalName);
            }
            else
            {
                retVal = Commands.Checkout(repo, retVal);
            }

            return retVal;
        }

        private void Commit(Repository repo, string userName, string fileName)
        {
            // Commit the file

            Signature user = new Signature(userName, userName, DateTime.Now);

            Commands.Stage(repo, fileName);
            repo.Commit(_msgSavingFile, user, user, new CommitOptions() { AllowEmptyCommit = true });
        }

        private void UpdateBranch(string branchName, Signature user, string fromBranchName)
        {
            CheckBranchFolder(branchName, user);

            var workingPath = _repositoryPath + @"\" + branchName;
            using (var repo = new Repository(workingPath))
            {
                // Update repository "from"
                Pull(repo, fromBranchName, user);

                // Merge and Push from branch "branchFrom"
                MergeAndPush(repo, user, fromBranchName, branchName);
            }
        }

        private void Pull(Repository repo, string branchName, Signature user)
        {
            try
            {
                var branchFrom = repo.Branches[branchName];
                branchFrom = Commands.Checkout(repo, branchFrom);

                Remote remote = repo.Network.Remotes["origin"];

                repo.Branches.Update(branchFrom,
                    b => b.Remote = remote.Name,
                    b => b.UpstreamBranch = branchFrom.CanonicalName);

                Commands.Pull(repo, user, new PullOptions() { MergeOptions = new MergeOptions() { FastForwardStrategy = FastForwardStrategy.FastForwardOnly /*, FileConflictStrategy = CheckoutFileConflictStrategy.Theirs*/ } });
            }
            catch
            {
            }
        }

        private void Push(Repository repo, Branch branch)
        {
            Remote remote = repo.Network.Remotes["origin"];

            repo.Branches.Update(branch,
                b => b.Remote = remote.Name,
                b => b.UpstreamBranch = branch.CanonicalName);

            PushOptions options = new PushOptions();

            //options.CredentialsProvider = new LibGit2Sharp.Handlers.CredentialsHandler(
            //    (url, usernameFromUrl, types) =>
            //        new UsernamePasswordCredentials()
            //        {
            //            Username = _scenario.UserName,
            //            Password = _scenario.Password
            //        });

            repo.Network.Push(branch, options);
        }

        private void MergeAndPush(Repository repo, Signature user, string branchFromName, string branchToName)
        {
            // Checkout repository "To"

            var branchTo = repo.Branches[branchToName];
            branchTo = Commands.Checkout(repo, branchTo);

            // PULL repository "To"

            try
            {
                Remote remote = repo.Network.Remotes["origin"];

                repo.Branches.Update(branchTo,
                    b => b.Remote = remote.Name,
                    b => b.UpstreamBranch = branchTo.CanonicalName);

                Commands.Pull(repo, user, new PullOptions() { MergeOptions = new MergeOptions() { FastForwardStrategy = FastForwardStrategy.FastForwardOnly /*, FileConflictStrategy = CheckoutFileConflictStrategy.Theirs*/ } });
            }
            catch { }

            // MERGE 

            var branchFrom = repo.Branches[branchFromName];
            MergeResult result = repo.Merge(branchFrom, user, new MergeOptions() { FileConflictStrategy = CheckoutFileConflictStrategy.Ours });

            // PUSH
            Push(repo, branchTo);
        }

        #endregion Support functions 
    }
}