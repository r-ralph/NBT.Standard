using System.Collections.Generic;

namespace NBT
{
    public interface ICollectionTag
    {
        #region Properties

        bool IsList { get; }

        TagType ListType { get; set; }

        IList<Tag> Values { get; }

        #endregion
    }
}