using NuGet.Versioning;
using Pizza.Common;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Velopack;
using Velopack.Logging;

namespace Pizza.KeyboardAndMouseAdapter.Backend
{
    //TODO tidy up this class
    class AppUpdater
    {

//        private bool IsNewerVersionAvailable(List<ReleaseEntry> releaseEntries)
//        {
//            SemanticVersion currentVersion = VersionUtil.GetSemanticVersion();
//            Log.Information("AppUpdater.UpdateIfAvailable() current version " + currentVersion.Version.ToString());
//
//            bool newerVersion = false;
//            foreach (ReleaseEntry releaseEntry in releaseEntries)
//            {
//                SemanticVersion newVersion = releaseEntry.Version;
//                Log.Information("AppUpdater.UpdateIfAvailable() 'new' version found: " + newVersion.Version.ToString());
//
//                if (newVersion > currentVersion)
//                {
//                    newerVersion = true;
//                    Log.Information("AppUpdater.UpdateIfAvailable() newer version found");
//                }
//            }
//
//            return newerVersion;
//        }
//
//        private void PrintCurrentlyInstalledNugetVersion(UpdateManager mgr)
//        {
//            try
//            {
//                SemanticVersion nugetCurrentVersion = mgr.CurrentlyInstalledVersion();
//                if (nugetCurrentVersion != null)
//                {
//                    Log.Information("AppUpdater.printCurrentlyInstalledNugetVersion() " + nugetCurrentVersion.Version.ToString());
//                }
//                else
//                {
//                    Log.Information("AppUpdater.printCurrentlyInstalledNugetVersion() null");
//                }
//            }
//            catch (Exception ex)
//            {
//                ExceptionLogger.LogException("AppUpdater.RemoveNewShortcuts Shortcut deletion failed", ex);
//            }
//        }

        private void RemoveNewShortcuts()
        {
            try
            {
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                //TODO does Velopack still do this ?
//                File.Delete(Path.Combine(desktopPath, "EasyHookSvc.lnk"));
//                File.Delete(Path.Combine(desktopPath, "EasyHookSvc64.lnk"));
//                File.Delete(Path.Combine(desktopPath, "EasyHookSvc32.lnk"));
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException("AppUpdater.RemoveNewShortcuts Shortcut deletion failed", ex);
            }
        }

        // Log notes
        // logically we want Log.Debug, however this happens prior to the log level being set,
        // So we are using Log.Information so we dont lose information
        public void UpdateIfAvailable()
        {
            Log.Information("AppUpdater.UpdateIfAvailable() start");
            try
            {
                UpdateInfo updateInfo; 
                UpdateManager updateManager = new UpdateManager("https://github.com/starshinata/PS4-Keyboard-and-Mouse-Adapter");

                try
                {
                    Log.Information("AppUpdater.UpdateIfAvailable() trying updateManager.CheckForUpdates()");
                    updateInfo = updateManager.CheckForUpdates();
                }
                catch (Exception e)
                {
                    ExceptionLogger.LogException("Velopack check for update error", e);
                    return;
                }

                if (updateInfo == null || updateInfo.TargetFullRelease == null || updateInfo.TargetFullRelease.Version == null) {
                    Log.Information("AppUpdater.UpdateIfAvailable() updateInfo null, thus no update");
                }
                Log.Information("updateInfo.ToString(); " + updateInfo.ToString());

                if (updateInfo.TargetFullRelease.Version > updateManager.CurrentVersion)
                {
                    Log.Information("updateInfo NEWER");
                    
                    Log.Information("NOTE: update will be triggered on clean exit of this application!");

                    // "wait for exit"
                    // so update when the user is done
                    bool silent = true;
                    bool restart = false;
                    updateManager.WaitExitThenApplyUpdates(updateInfo.TargetFullRelease, silent, restart);
                }
                else {
                    Log.Information("updateInfo OLDER");
                }
                
                
                //{
                //    Log.Information("AppUpdater.UpdateIfAvailable() update check");
                //    UpdateInfo updateResult = await mgr.CheckForUpdate();
                //    Log.Information("AppUpdater.UpdateIfAvailable() update check returned");

                //    PrintCurrentlyInstalledNugetVersion(mgr);

                //    bool newerVersion = IsNewerVersionAvailable(updateResult.ReleasesToApply);

                //    if (newerVersion)
                //    {
                //        Log.Information("AppUpdater.UpdateIfAvailable() trying UpdateApp");
                //        await mgr.UpdateApp();
                //        RemoveNewShortcuts();
                //        Log.Information("AppUpdater.UpdateIfAvailable() UpdateApp complete");
                //    }
                //    else
                //    {
                //        Log.Information("AppUpdater.UpdateIfAvailable() updates skipped");
                //    }
                //}
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException("AppUpdater.UpdateIfAvailable() Github Update failed", ex);
            }
            Log.Information("AppUpdater.UpdateIfAvailable() update check completed");
        }

        //private void CheckForUpdate(UpdateManager _um)
        //{
           


            
        //}
        //private void UpdateStatus(UpdateManager _um)
        //{
        //    StringBuilder sb = new StringBuilder();
        //    sb.AppendLine($"Velopack version: {VelopackRuntimeInfo.VelopackNugetVersion}");
        //    sb.AppendLine($"This app version: {(_um.IsInstalled ? _um.CurrentVersion : "(n/a - not installed)")}");

        //    if (_update != null)
        //    {
        //        sb.AppendLine($"Update available: {_update.TargetFullRelease.Version}");
        //        BtnDownloadUpdate.IsEnabled = true;
        //    }
        //    else
        //    {
        //        BtnDownloadUpdate.IsEnabled = false;
        //    }

        //    if (_um.UpdatePendingRestart != null)
        //    {
        //        sb.AppendLine("Update ready, pending restart to install");
        //        BtnRestartApply.IsEnabled = true;
        //    }
        //    else
        //    {
        //        BtnRestartApply.IsEnabled = false;
        //    }

        //    TextStatus.Text = sb.ToString();
        //    BtnCheckUpdate.IsEnabled = true;
        //}
    }
}
