// <copyright file="DataTransferQueue.cs" company="City of San Diego">
// Copyright (c) City of San Diego. All rights reserved.
// </copyright>

namespace FireJobUtilities.Helpers
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;
    using ESRI.ArcGIS.esriSystem;
    using ESRI.ArcGIS.Geodatabase;
    using ESRI.ArcGIS.Geoprocessing;
    using ESRI.ArcGIS.Geoprocessor;
    using FireJobUtilities.Events;
    using FireJobUtilities.Models;
    using FireJobUtilities.Configuration;
    using FireJobUtilities.Helpers;

    /// <summary>
    /// Process for queuing and transfering IDataset objects to a destination workspace.
    /// Is compatible with single threaded apartment (STA)
    /// </summary>
    /// <remarks>
    /// Author: Andrew Tangeman
    /// Date: 02/02/2017
    /// </remarks>
    /// 
    public class DataTransferProcess : ProcessBase, IProcess
    {
        #region Members

        private Queue<DataTransferModel> OriginDatasetQueue { get; set; }

        private bool _IsRunning = false;

        private string _ProcessName = string.Empty;

        /// <summary>
        /// Gets a value indicating whether boolean set to true if process is running.
        /// </summary>
        public bool IsRunning
        {
            get
            {
                return this._IsRunning;
            }

            private set
            {
                this._IsRunning = value;
            }
        }

        /// <summary>
        /// Gets name for process when reporting to message and debug delegates.
        /// </summary>
        public string ProcessName
        {
            get
            {
                return this._ProcessName;
            }
        }

        #endregion Members

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DataTransferProcess"/> class.
        /// Default constructor for initializing DataTransferQueue
        /// </summary>
        public DataTransferProcess()
        {
            OriginDatasetQueue = new Queue<DataTransferModel>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataTransferProcess"/> class.
        /// Single parameter constructor for initializing DataTransferQueue
        /// </summary>
        /// <param name="dtModel">Target workspace to copy objects to</param>
        public DataTransferProcess(DataTransferModel dtModel)
        {
            this._ProcessName = this.GetType().Name; // initializes process name to class name using reflection.
            OriginDatasetQueue = new Queue<DataTransferModel>();
            OriginDatasetQueue.Enqueue(dtModel);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataTransferProcess"/> class.
        /// Constructor for initializing DataTransferQueue
        /// </summary>
        /// <param name="dtModels">IEnumerable container of DataTransferModel parameters to load to destination workspace</param>
        public DataTransferProcess(List<DataTransferModel> dtModels)
        {
            this._ProcessName = this.GetType().Name; // initializes process name to class name using reflection.
            OriginDatasetQueue = new Queue<DataTransferModel>();
            AddDTModels(dtModels);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataTransferProcess"/> class.
        /// Constructor for initializing DataTransferQueue
        /// </summary>
        /// <param name="name">name of dt process</param>
        /// <param name="origName">name of origin dataset to be copied</param>
        /// <param name="origWorkspace">name of origin workspace</param>
        /// <param name="targetName">output target to copy</param>
        /// <param name="targetWorkspace">name of target workspace to copy to</param>
        /// <param name="transferMethod">method to use for data transfer operation</param>
        public DataTransferProcess(string name, string origName, string origWorkspace, string targetName, string targetWorkspace, string transferMethod)
        {
            this._ProcessName = this.GetType().Name; // initializes process name to class name using reflection.
            AddDTModel(name, origName, origWorkspace, targetWorkspace, targetName, transferMethod);
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Adds datasets to data transfer queue
        /// </summary>
        /// <param name="dataModels">IEnumerable container of IDatasets</param>
        public void AddDTModels(List<DataTransferModel> dtModels)
        {
            foreach (DataTransferModel dt in dtModels)
            {
                AddDTModel(dt);
            }
        }

        /// <summary>
        /// Adds datasets to data transfer queue
        /// </summary>
        /// <param name="eDTModel">IEnumerable container of IDatasets</param>
        public void AddDTModel(DataTransferModel dtModels)
        {
            OriginDatasetQueue.Enqueue(dtModels);
        }

        /// <summary>
        /// Adds a new data transfer object to the process list
        /// </summary>
        /// <param name="name">name of dt process</param>
        /// <param name="origName">name of origin dataset to be copied</param>
        /// <param name="origWorkspace">name of origin workspace</param>
        /// <param name="targetName">output target to copy</param>
        /// <param name="targetWorkspace">name of target workspace to copy to</param>
        /// <param name="transferMethod">method to use for data transfer operation</param>
        public void AddDTModel(string name, string origName, string origWorkspace, string targetName, string targetWorkspace, string transferMethod)
        {
            DataTransferModel dt = new DataTransferModel();
            dt.Name = "";
            dt.OriginName = origName;
            dt.OriginWorkspace = origWorkspace;
            dt.TargetName = targetName;
            dt.TargetWorkspace = targetWorkspace;
            dt.TransferMethod = transferMethod;
            OriginDatasetQueue.Enqueue(dt);
        }

        /// <summary>
        /// Starts process queue
        /// </summary>
        public void StartProcess()
        {
            try
            {
                this.RaiseProcessEvent(ProcessEventTypes.INFO, $"Data transfer process started.");
                this._IsRunning = true;
                if (OriginDatasetQueue.Count() > 0)
                {
                    DataTransferModel nextProcess = OriginDatasetQueue.Dequeue();
                    switch (nextProcess.TransferMethod)
                    {
                        default:
                            break;
                        case "TRUNCATE_APPEND":
                            TruncateAppendFeatureClass(nextProcess);
                            break;
                        case "COPY":
                            CopyToSDEWorkspace(nextProcess);
                            break;
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        private void TruncateAppendTable(ITable inTable, ITable outTable)
        {

        }

        /// <summary>
        /// Truncates and appends input feature class
        /// </summary>
        /// <param name="originFC">Origin feature class</param>
        /// <param name="originWS">origin workspace</param>
        /// <param name="targetFC">destination feature class</param>
        /// <param name="targetWS">destination workspace</param>
        private void TruncateAppendFeatureClass(object originFC, string originWS, object targetFC, string targetWS)
        {
            IFeatureClass truncFC;
            IGPProcess truncateGP = FireGeoprocessingTools.GetTruncateFeatureTableGP(targetFC);
            RunGeoprocess(ref truncateGP, targetWS, true, 0, out truncFC);

            IGPProcess appendGP = FireGeoprocessingTools.GetAppendFeatureTableGP(originFC, targetFC);
            RunGeoprocess(ref appendGP, targetWS);
        }

        /// <summary>
        /// Copies feature class or dataset to SDE Workspace
        /// </summary>
        /// <param name="dtModel">DTModel to copy</param>
        public void TruncateAppendFeatureClass(DataTransferModel dtModel)
        {
            bool overrideOutput;
            if (bool.TryParse(dtModel.OverrideOutput, out overrideOutput))
            {
                TruncateAppendFeatureClass(dtModel.OriginName, dtModel.OriginWorkspace, dtModel.TargetName, dtModel.TargetWorkspace);
            }
        }

        /// <summary>
        /// Copies feature class or dataset to SDE Workspace
        /// </summary>
        /// <param name="originDataName">name of origin dataset to be copied</param>
        /// <param name="originWS">name of origin workspace</param>
        /// <param name="destinationName">output target to copy</param>
        /// <param name="destinationPath">name of target workspace to copy to</param>
        public void CopyToSDEWorkspace(string originDataName, string originWS, string destinationName, string destinationPath, bool deleteExisting)
        {
            RaiseLogEvent($"Copying {originDataName}");

            if (deleteExisting)
            {
                RaiseLogEvent($"Deleting {originDataName}");
                IGPProcess deleteGP = FireGeoprocessingTools.GetDeleteFeatureClassGP(destinationName);
                RunGeoprocess(ref deleteGP, destinationPath);
            }
            else
            {
                IGPProcess renameGP = FireGeoprocessingTools.GetRenameGP(destinationName, destinationName + "_OLD");
                RunGeoprocess(ref renameGP, destinationPath);
            }

            IGPProcess copyGP = FireGeoprocessingTools.GetCopyDataGP($"{originWS}\\{originDataName}", $"{destinationName}");
            RunGeoprocess(ref copyGP, destinationPath, true);
        }

        /// <summary>
        /// Copies feature class or dataset to SDE Workspace
        /// </summary>
        /// <param name="dtModel">DTModel to copy</param>
        public void CopyToSDEWorkspace(DataTransferModel dtModel)
        {
            bool overrideOutput;
            if (bool.TryParse(dtModel.OverrideOutput, out overrideOutput))
            {
                CopyToSDEWorkspace(dtModel.OriginName, dtModel.OriginWorkspace, dtModel.TargetName, dtModel.TargetWorkspace, overrideOutput);
            }
        }

        /// <summary>
        /// Runnings generic geoprocess passed from FireGeoprocessingTools static class.
        /// </summary>
        /// <param name="inGPObj">In geoprocessing object to run.</param>
        /// <param name="workspacePath">Workspace path to use</param>
        /// <param name="overwriteOutput">bool set to true if output is to be overwritten</param>
        private void RunGeoprocess(ref IGPProcess inGPObj, string workspacePath, bool overwriteOutput = true)
        {
            using (FeatureProcessingFactory featureProcessing = new FeatureProcessingFactory(workspacePath, overwriteOutput))
            {
                RaiseLogEvent($"Running {inGPObj.ToolName}");
                featureProcessing.OnProcessChangedEvent += base.RaiseProcessChangedEvent;
                featureProcessing.OnProcessExceptionEvent += this.RaiseExceptionEvent;
                featureProcessing.RunGeoprocessingOperation(inGPObj);
                featureProcessing.OnProcessChangedEvent -= base.RaiseProcessChangedEvent;
                featureProcessing.OnProcessExceptionEvent -= this.RaiseExceptionEvent;
            }
        }

        /// <summary>
        /// Runnings generic geoprocess passed from FireGeoprocessingTools static class.
        /// </summary>
        /// <param name="inGPObj">In geoprocessing object to run.</param>
        /// <param name="workspacePath">Workspace path to use</param>
        /// <param name="returnIdx">index of return object.</param>
        /// <param name="overwriteOutput">bool set to true if output is to be overwritten</param>
        /// <param name="result">IFeatureClass object returned from GPOperation</param>
        private void RunGeoprocess(ref IGPProcess inGPObj, string workspacePath, bool overwriteOutput, int returnIdx, out IFeatureClass result)
        {
            using (FeatureProcessingFactory featureProcessing = new FeatureProcessingFactory(workspacePath, overwriteOutput))
            {
                RaiseLogEvent($"Running {inGPObj.ToolName}");
                featureProcessing.OnProcessChangedEvent += base.RaiseProcessChangedEvent;
                featureProcessing.OnProcessExceptionEvent += this.RaiseExceptionEvent;
                featureProcessing.RunGeoprocessingOperation(inGPObj);
                result = featureProcessing.GPResultToFeatureClass(returnIdx);
                featureProcessing.OnProcessChangedEvent -= base.RaiseProcessChangedEvent;
                featureProcessing.OnProcessExceptionEvent -= this.RaiseExceptionEvent;
            }
        }

        private void RaiseExceptionEvent(object sender, Exception ex)
        {
            this.RaiseProcessException(ex);
        }

        /// <summary>
        /// Stops the process or subroutine.
        /// </summary>
        public void StopProcess()
        {
            this._IsRunning = false;
            this.OriginDatasetQueue.Clear();
            this.RaiseProcessComplete(FireProcessReturnCodes.SUCCESS, "Data transfer process complete!");
        }

        /// <summary>
        /// Empties all objects from processing queue
        /// </summary>
        public void EmptyProcessQueue()
        {
            this._IsRunning = false;
            this.OriginDatasetQueue.Clear();
        }
        #endregion Methods
    }
}
