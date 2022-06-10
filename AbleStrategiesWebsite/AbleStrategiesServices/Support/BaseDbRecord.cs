using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
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
        private Guid id = Guid.Empty;
        public Guid Id { get => id; set => id = value; }

        /// <summary>
        /// [private] Not persisted to DB, for tracking edits.
        /// </summary>
        private EditFlag editFlag = Support.EditFlag.New;

        /// <summary>
        /// When was this record created?
        /// </summary>
        private DateTime dateCreated = DateTime.Now;

        /// <summary>
        /// When was this record last modified?
        /// </summary>
        private DateTime dateModified = DateTime.Now;

        /// <summary>
        /// Unique record type discriminator (note: implement as a call to a static method)
        /// </summary>
        [JsonIgnore]
        public virtual string RecordKind { get; }

        /// <summary>
        /// Ctor.
        /// </summary>
        protected BaseDbRecord()
        {
            id = Guid.NewGuid();
        }

        /// <summary>
        /// Should only elevate, never get lower, and never be persisted to the DB
        /// </summary>
        public EditFlag EditFlag
        {
            get
            {
                return editFlag;
            }
            set
            {
                editFlag = (EditFlag)((value > editFlag) ? value : editFlag);
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
            editFlag = EditFlag.Unchanged; // adjusts Downward, so doesn't use setter
        }

        /// <summary>
        /// Date Created.
        /// </summary>
        public DateTime DateCreated
        {
            get
            {
                return dateCreated;
            }
            set
            {
                dateCreated = value;
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
                return dateModified;
            }
            set
            {
                dateModified = value;
                Mod();
            }
        }

        /// <summary>
        /// Update all data fields except for Id - keep this.Id, ignore source.Id (adjusts EditFlag, too)
        /// </summary>
        /// <remarks>The source record is expected to be dropped from the DB, replaced by this one</remarks>
        /// <param name="source">record from which to copy all data except for Id</param>
        public abstract void PopulateFrom(BaseDbRecord source);

        /// <summary>
        /// To be called by the PopulateFrom() method in each derived class.
        /// </summary>
        /// <param name="source">record from which to copy all data except for Id, abandoning it afterward</param>
        /// <returns>success</returns>
        protected bool PopulateBaseFrom(BaseDbRecord source)
        {
            if (source.RecordKind != RecordKind)
            {
                Logger.Error(null, "Bad DB Record mismatch " + RecordKind + " != " + source.RecordKind);
                return false;
            }
            this.id = source.id;
            this.editFlag = source.EditFlag;   // May need to adjust Downward, so don't use setter
            this.DateCreated = source.DateCreated;
            this.DateModified = source.DateModified;
            source.EditFlag = EditFlag.Zombie;
            return true;
        }

    }
}
