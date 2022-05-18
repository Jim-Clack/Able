using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AbleStrategiesServices.Support
{

    /// <summary>
    /// Used for tracking edits to a record for persisting to a DB
    /// </summary>
    public enum EditFlag
    {
        Unchanged = 0,      // Freshly read from the DB (don't re-persist: no-op)
        Modified = 1,       // Modified from DB (re-persist: update)
        New = 2,            // Freshly created in memory (persist: insert)
        Deleted = 3,        // To be deleted (remove from db: delete)
        Zombie = 999,       // Deleted, replaced, or otherwise abandoned
    }

    public abstract class BaseDbRecord
    {

        /// <summary>
        /// Globally unique Id for this record.
        /// </summary>
        private Guid _id = Guid.Empty;
        public Guid Id { get => _id; set => _id = value; }

        /// <summary>
        /// [private] Not persisted to DB, for tracking edits.
        /// </summary>
        private EditFlag _editFlag = Support.EditFlag.New;

        /// <summary>
        /// When was this record created?
        /// </summary>
        public DateTime _dateCreated = DateTime.Now;

        /// <summary>
        /// When was this record last modified?
        /// </summary>
        public DateTime _dateModified = DateTime.Now;

        /// <summary>
        /// Unique record type discriminator (note: implement as a call to a static method)
        /// </summary>
        public virtual string RecordKind { get; }

        /// <summary>
        /// Should only elevate, never get lower, and never be persisted to the DB
        /// </summary>
        public EditFlag EditFlag
        {
            get
            {
                return _editFlag;
            }
            set
            {
                _editFlag = (EditFlag)((value > _editFlag) ? value : _editFlag);
            }
        }

        /// <summary>
        /// Convenience method for internal use.
        /// </summary>
        protected void Mod()
        {
            EditFlag = EditFlag.Modified;
        }

        /// <summary>
        /// Convenience method for DB use.
        /// </summary>
        public void UnMod()
        {
            EditFlag = EditFlag.Unchanged;
        }

        /// <summary>
        /// Date Created.
        /// </summary>
        public DateTime DateCreated
        {
            get
            {
                return _dateCreated;
            }
            set
            {
                _dateCreated = value;
                Mod();
            }
        }

        /// <summary>
        /// Date Modified.
        /// </summary>
        public DateTime DateModified
        {
            get
            {
                return _dateModified;
            }
            set
            {
                _dateModified = value;
                Mod();
            }
        }

        /// <summary>
        /// Update all data fields except for Id - keep this.Id, ignore source.Id (adjusts EditFlag, too)
        /// </summary>
        /// <remarks>The source record is expected to be dropped from the DB, replaced by this one</remarks>
        /// <param name="source">record from which to copy all data except for Id</param>
        public abstract void Update(BaseDbRecord source);

        /// <summary>
        /// To be called by the Update() method in each derived class.
        /// </summary>
        /// <param name="source">record from which to copy all data except for Id, abandoning it afterward</param>
        /// <returns>success</returns>
        protected bool UpdateBase(BaseDbRecord source)
        {
            if (source.RecordKind != RecordKind)
            {
                Logger.Error(null, "Bad DB Record mismatch " + RecordKind + " != " + source.RecordKind);
                return false;
            }
            this.EditFlag = EditFlag.Modified;
            this.EditFlag = source.EditFlag;
            this.DateCreated = source.DateCreated;
            this.DateModified = source.DateModified;
            source.EditFlag = EditFlag.Zombie;
            return true;
        }

    }
}
