using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenMetaverse;

using ProtoBuf;

namespace Halcyon.Phlox.VM
{
    /// <summary>
    /// Variables that are set during some events to provide the script
    /// with access to llDetectedKey, llDetectedPos etc
    /// </summary>
    [ProtoContract]
    public class DetectVariables
    {
        /// <summary>
        /// DEPRECATED DO NOT USE. Field must remain for backwards compat
        /// </summary>
        [Obsolete]
        [ProtoMember(1)]
        private string GrabPbuf1Deprecated
        {
            get
            {
                return null; 
            }

            set
            {
                if (value != null)
                {
                    Grab = Vector3.Parse(value);
                }
            }
        }

        [ProtoMember(2)]
        public string Group;

        [ProtoMember(3)]
        public string Key;

        [ProtoMember(4)]
        public int LinkNumber;

        [ProtoMember(5)]
        public string Name;

        [ProtoMember(6)]
        public string Owner;

        /// <summary>
        /// DEPRECATED DO NOT USE. Field must remain for backwards compat
        /// </summary>
        [Obsolete]
        [ProtoMember(7)]
        private string PosPbuf1Deprecated
        {
            get
            {
                return null;
            }

            set
            {
                if (value != null)
                {
                    Pos = Vector3.Parse(value);
                }
            }
        }

        /// <summary>
        /// DEPRECATED DO NOT USE. Field must remain for backwards compat
        /// </summary>
        [Obsolete]
        [ProtoMember(8)]
        private string RotPbuf1Deprecated
        {
            get
            {
                return null;
            }

            set
            {
                if (value != null)
                {
                    Rot = Quaternion.Parse(value);
                }
            }
        }

        [ProtoMember(9)]
        public int Type;

        /// <summary>
        /// DEPRECATED DO NOT USE. Field must remain for backwards compat
        /// </summary>
        [Obsolete]
        [ProtoMember(10)]
        private string VelPbuf1Deprecated
        {
            get
            {
                return null;
            }

            set
            {
                if (value != null)
                {
                    Vel = Vector3.Parse(value);
                }
            }
        }


        /// <summary>
        /// DEPRECATED DO NOT USE. Field must remain for backwards compat
        /// </summary>
        [Obsolete]
        [ProtoMember(11)]
        private string TouchBinormalPbuf1Deprecated
        {
            get
            {
                return null;
            }

            set
            {
                if (value != null)
                {
                    TouchBinormal = Vector3.Parse(value);
                }
            }
        }

        [ProtoMember(12)]
        public int TouchFace;

        /// <summary>
        /// DEPRECATED DO NOT USE. Field must remain for backwards compat
        /// </summary>
        [Obsolete]
        [ProtoMember(13)]
        private string TouchNormalPbuf1Deprecated
        {
            get
            {
                return null;
            }

            set
            {
                if (value != null)
                {
                    TouchNormal = Vector3.Parse(value);
                }
            }
        }

        /// <summary>
        /// DEPRECATED DO NOT USE. Field must remain for backwards compat
        /// </summary>
        [Obsolete]
        [ProtoMember(14)]
        private string TouchPosPbuf1Deprecated
        {
            get
            {
                return null;
            }

            set
            {
                if (value != null)
                {
                    TouchPos = Vector3.Parse(value);
                }
            }
        }

        /// <summary>
        /// DEPRECATED DO NOT USE. Field must remain for backwards compat
        /// </summary>
        [Obsolete]
        [ProtoMember(15)]
        private string TouchSTPbuf1Deprecated
        {
            get
            {
                return null;
            }

            set
            {
                if (value != null)
                {
                    TouchST = Vector3.Parse(value);
                }
            }
        }

        /// <summary>
        /// DEPRECATED DO NOT USE. Field must remain for backwards compat
        /// </summary>
        [Obsolete]
        [ProtoMember(16)]
        private string TouchUVPbuf1Deprecated
        {
            get
            {
                return null;
            }

            set
            {
                if (value != null)
                {
                    TouchUV = Vector3.Parse(value);
                }
            }
        }

        [ProtoMember(17)]
        public Vector3 Grab;

        [ProtoMember(18)]
        public Vector3 Pos;

        [ProtoMember(19)]
        public Quaternion Rot;

        [ProtoMember(20)]
        public Vector3 Vel;

        [ProtoMember(21)]
        public Vector3 TouchBinormal;

        [ProtoMember(22)]
        public Vector3 TouchNormal;

        [ProtoMember(23)]
        public Vector3 TouchPos;

        [ProtoMember(24)]
        public Vector3 TouchST;

        [ProtoMember(25)]
        public Vector3 TouchUV;

        /// <summary>
        /// This is the UUID of a bot that is having sensor/listen events
        /// called on it
        /// See botSensor/botSensorRepeat/iwDetectedBot functions
        /// </summary>
        [ProtoMember(26)]
        public string BotID;
    }
}
