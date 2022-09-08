using NpgsqlTypes;

namespace Altinn.AccessGroups.Core.Models
{
    /// <summary>
    /// The differenst supported external registers
    /// </summary>
    public enum ExternalSource
    {
        /// <summary>
        /// Undefined default value
        /// </summary>
        [PgName("undefined")]
        Undefined = 0,

        /// <summary>
        /// Enhetsregisteret
        /// </summary>
        [PgName("enhetsregisteret")]
        Enhetsregisteret = 1
    }
}
