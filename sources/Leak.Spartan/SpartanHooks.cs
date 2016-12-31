﻿using System;
using Leak.Events;

namespace Leak.Spartan
{
    public class SpartanHooks
    {
        /// <summary>
        /// Called when the task was started and no other task is ongoing.
        /// </summary>
        public Action<TaskStarted> OnTaskStarted;

        /// <summary>
        /// Called when the task was completed and another pending task
        /// can be probably started.
        /// </summary>
        public Action<TaskCompleted> OnTaskCompleted;

        /// <summary>
        /// Called when repository data was successfully allocated
        /// in the destination directory and there was a change
        /// which affected the file system.
        /// </summary>
        public Action<DataAllocated> OnDataAllocated;

        /// <summary>
        /// Called when repository data was verified against metafile
        /// and the actual bitfield is known.
        /// </summary>
        public Action<DataVerified> OnDataVerified;

        /// <summary>
        /// Called when some piece was successfully downloaded, verified
        /// and therefore the retrieving process changed.
        /// </summary>
        public Action<DataChanged> OnDataChanged;

        /// <summary>
        /// Called when the all pieces were successfully retrieved
        /// and validated against file hash. It means there is no
        /// more pieces to download and no more progress can be made.
        /// </summary>
        public Action<DataCompleted> OnDataCompleted;

        /// <summary>
        /// Called when some piece was successfully verified against
        /// the hash and the piece is considered to be completed.
        /// </summary>
        public Action<PieceAccepted> OnPieceAccepted;

        /// <summary>
        /// Called when some piece was unsuccessfully verified against
        /// the hash and the piece is considered to be incompleted.
        /// </summary>
        public Action<PieceRejected> OnPieceRejected;
    }
}