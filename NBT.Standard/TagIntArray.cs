using System.Text;

namespace NBT
{
    public sealed partial class TagIntArray
    {
        #region Methods

        public override string ToValueString()
        {
            var sb = new StringBuilder();

            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < _value.Length; i++)
            {
                if (sb.Length != 0)
                {
                    sb.Append(", ");
                }

                sb.Append(_value[i].ToString());
            }

            return sb.ToString();
        }

        #endregion
    }
}