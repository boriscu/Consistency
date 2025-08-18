using System.Runtime.Serialization;

namespace Sensors.Contracts
{
    [DataContract]
    public enum SensorId
    {
        [EnumMember] S1 = 1,
        [EnumMember] S2 = 2,
        [EnumMember] S3 = 3,
        [EnumMember] S4 = 4,
        [EnumMember] S5 = 5,
        [EnumMember] S6 = 6,
        [EnumMember] S7 = 7,
        [EnumMember] S8 = 8,
        [EnumMember] S9 = 9,
        [EnumMember] S10 = 10
    }
}
