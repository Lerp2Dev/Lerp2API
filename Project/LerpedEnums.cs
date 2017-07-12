using System;

namespace Lerp2Assets.CrossPlatformInput
{
    /// <summary>
    /// Class CrossPlatformInputManager.
    /// </summary>
    public static partial class CrossPlatformInputManager
    {
        /// <summary>
        /// Enum ActiveInputMethod
        /// </summary>
        public enum ActiveInputMethod
        {
            /// <summary>
            /// The hardware
            /// </summary>
            Hardware,

            /// <summary>
            /// The touch
            /// </summary>
            Touch
        }
    }
}

namespace Lerp2API.Game
{
    /// <summary>
    /// Enum Types
    /// </summary>
    public enum Types
    {
        /// <summary>
        /// Enum SStyles
        /// </summary>
        Unknown,

        /// <summary>
        /// The int
        /// </summary>
        Int,

        /// <summary>
        /// The string
        /// </summary>
        String,

        /// <summary>
        /// The float
        /// </summary>
        Float,

        /// <summary>
        /// The bool
        /// </summary>
        Bool
    }

    /// <summary>
    /// Enum TaskState
    /// </summary>
    public enum Styles
    {
        /// <summary>
        /// The none
        /// </summary>
        None,

        /// <summary>
        /// The Optimizers namespace.
        /// </summary>
        /// <summary>
        /// The Optimizers namespace.
        /// </summary>
        Bold,

        /// <summary>
        /// The italic
        /// </summary>
        Italic,

        /// <summary>
        /// Enum Direction
        /// </summary>
        Color
    }

    /// <summary>
    /// Enum SStyles
    /// </summary>
    public enum SStyles
    {
        /// <summary>
        /// The none
        /// </summary>
        None,

        /// <summary>
        /// The underline
        /// </summary>
        Underline,

        /// <summary>
        /// The obfuscated
        /// </summary>
        Obfuscated,

        /// <summary>
        /// The single quote
        /// </summary>
        Strike
    }
}

namespace Lerp2API
{
    /// <summary>
    /// Enum Position
    /// </summary>
    public enum Position
    {
        /// <summary>
        /// The upper left
        /// </summary>
        UpperLeft,

        /// <summary>
        /// The upper right
        /// </summary>
        UpperRight,

        /// <summary>
        /// The Optimizers namespace.
        /// </summary>
        /// <summary>
        /// The Optimizers namespace.
        /// </summary>
        BottomLeft,

        /// <summary>
        /// The bottom right
        /// </summary>
        BottomRight
    }

    /// <summary>
    /// Enum TaskState
    /// </summary>
    public enum TaskState
    {
        /// <summary>
        /// The HtmlAgilityPack namespace.
        /// </summary>
        /// <summary>
        /// The HtmlAgilityPack namespace.
        /// </summary>
        NotStarted,

        /// <summary>
        /// The running
        /// </summary>
        Running,

        /// <summary>
        /// Enum AttributeValueQuote
        /// </summary>
        /// <summary>
        /// Enum AttributeValueQuote
        /// </summary>
        Paused,

        /// <summary>
        /// The stopped
        /// </summary>
        Stopped
    }
}

namespace Lerp2API.Optimizers
{
    /// <summary>
    /// Enum Direction
    /// </summary>
    public enum Direction
    {
        /// <summary>
        /// Up
        /// </summary>
        up,

        /// <summary>
        /// The upper right
        /// </summary>
        upperRight,

        /// <summary>
        /// The right
        /// </summary>
        right,

        /// <summary>
        /// Down right
        /// </summary>
        downRight,

        /// <summary>
        /// Down
        /// </summary>
        down,

        /// <summary>
        /// Down left
        /// </summary>
        downLeft,

        /// <summary>
        /// The left
        /// </summary>
        left,

        /// <summary>
        /// The upper left
        /// </summary>
        upperLeft
    }
}

namespace HtmlAgilityPack
{
    /// <summary>
    /// Enum AttributeValueQuote
    /// </summary>
    public enum AttributeValueQuote
    {
        /// <summary>
        /// The single quote
        /// </summary>
        SingleQuote,

        /// <summary>
        /// The double quote
        /// </summary>
        DoubleQuote
    }

    /// <summary>
    /// Enum HtmlElementFlag
    /// </summary>
    [Flags]
    public enum HtmlElementFlag
    {
        /// <summary>
        /// The can overlap
        /// </summary>
        CanOverlap = 8,

        /// <summary>
        /// The c data
        /// </summary>
        CData = 1,

        /// <summary>
        /// The closed
        /// </summary>
        Closed = 4,

        /// <summary>
        /// The empty
        /// </summary>
        Empty = 2
    }

    /// <summary>
    /// Enum HtmlNodeType
    /// </summary>
    public enum HtmlNodeType
    {
        /// <summary>
        /// The document
        /// </summary>
        Document,

        /// <summary>
        /// The element
        /// </summary>
        Element,

        /// <summary>
        /// The comment
        /// </summary>
        Comment,

        /// <summary>
        /// The text
        /// </summary>
        Text
    }

    /// <summary>
    /// Enum HtmlParseErrorCode
    /// </summary>
    public enum HtmlParseErrorCode
    {
        /// <summary>
        /// The tag not closed
        /// </summary>
        TagNotClosed,

        /// <summary>
        /// The tag not opened
        /// </summary>
        TagNotOpened,

        /// <summary>
        /// The charset mismatch
        /// </summary>
        CharsetMismatch,

        /// <summary>
        /// The end tag not required
        /// </summary>
        EndTagNotRequired,

        /// <summary>
        /// The end tag invalid here
        /// </summary>
        EndTagInvalidHere
    }

    /// <summary>
    /// Enum MixedCodeDocumentFragmentType
    /// </summary>
    public enum MixedCodeDocumentFragmentType
    {
        /// <summary>
        /// The code
        /// </summary>
        Code,

        /// <summary>
        /// The text
        /// </summary>
        Text
    }
}

namespace Lerp2APIEditor.EditorWindows
{
    /// <summary>
    /// Enum LerpedAPIChange
    /// </summary>
    public enum LerpedAPIChange
    {
        /// <summary>
        /// The automatic
        /// </summary>
        /// <summary>
        /// The automatic
        /// </summary>
        Auto,

        /// <summary>
        /// The in enter
        /// </summary>
        InEnter,

        /// <summary>
        /// The default
        /// </summary>
        Default
    }
}

namespace Lerp2API.SafeECalls
{
    /// <summary>
    /// Enum LoggerType
    /// </summary>
    public enum LoggerType
    {
        /// <summary>
        /// The information
        /// </summary>
        INFO,

        /// <summary>
        /// The warn
        /// </summary>
        WARN,

        /// <summary>
        /// The error
        /// </summary>
        ERROR
    }
}

namespace FullSerializer
{
    /// <summary>
    /// The actual type that a JsonData instance can store.
    /// </summary>
    public enum fsDataType
    {
        /// <summary>
        /// The array
        /// </summary>
        Array,

        /// <summary>
        /// The object
        /// </summary>
        Object,

        /// <summary>
        /// The double
        /// </summary>
        Double,

        /// <summary>
        /// The int64
        /// </summary>
        Int64,

        /// <summary>
        /// The boolean
        /// </summary>
        Boolean,

        /// <summary>
        /// The string
        /// </summary>
        String,

        /// <summary>
        /// The null
        /// </summary>
        Null
    }

    /// <summary>
    /// Controls how the reflected converter handles member serialization.
    /// </summary>
    public enum fsMemberSerialization
    {
        /// <summary>
        /// Only members with [SerializeField] or [fsProperty] attributes are serialized.
        /// </summary>
        OptIn,

        /// <summary>
        /// Only members with [NotSerialized] or [fsIgnore] will not be serialized.
        /// </summary>
        OptOut,

        /// <summary>
        /// The default member serialization behavior is applied.
        /// </summary>
        Default
    }
}

namespace RadicalLibrary
{
    /// <summary>
    /// Enum EasingType
    /// </summary>
    public enum EasingType
    {
        /// <summary>
        /// The step
        /// </summary>
        Step,

        /// <summary>
        /// The linear
        /// </summary>
        Linear,

        /// <summary>
        /// The sine
        /// </summary>
        Sine,

        /// <summary>
        /// The quadratic
        /// </summary>
        Quadratic,

        /// <summary>
        /// The cubic
        /// </summary>
        Cubic,

        /// <summary>
        /// The quartic
        /// </summary>
        Quartic,

        /// <summary>
        /// The quintic
        /// </summary>
        Quintic
    }

    /// <summary>
    /// Enum SmoothingMode
    /// </summary>
    public enum SmoothingMode
    {
        /// <summary>
        /// The slerp
        /// </summary>
        slerp = 3,

        /// <summary>
        /// The damp
        /// </summary>
        damp = 1,

        /// <summary>
        /// The lerp
        /// </summary>
        lerp = 2,

        /// <summary>
        /// The smooth
        /// </summary>
        smooth = 0
    }
}

namespace Serialization
{
    /// <summary>
    /// Class LevelSerializer.
    /// </summary>
    public static partial class LevelSerializer
    {
        /// <summary>
        /// Serialization modes.
        /// </summary>
        public enum SerializationModes
        {
            /// <summary>
            /// Serialize when suspended
            /// </summary>
            SerializeWhenFree,

            /// <summary>
            /// Ensure that there is serialization data
            /// when suspending
            /// </summary>
            CacheSerialization
        }
    }

    /// <summary>
    /// Class StoreMaterials.
    /// </summary>
    /// <seealso cref="UnityEngine.MonoBehaviour" />
    public partial class StoreMaterials
    {
        /// <summary>
        /// Class MaterialProperty.
        /// </summary>
        public partial class MaterialProperty
        {
            /// <summary>
            /// Enum PropertyType
            /// </summary>
            [Serializable]
            public enum PropertyType
            {
                /// <summary>
                /// The color
                /// </summary>
                Color = 0,

                /// <summary>
                /// The vector
                /// </summary>
                Vector = 1,

                /// <summary>
                /// The float
                /// </summary>
                Float = 2,

                /// <summary>
                /// The range
                /// </summary>
                Range = 3,

                /// <summary>
                /// The tex env
                /// </summary>
                TexEnv = 4,
            }
        }
    }

    /// <summary>
    /// Class StoreAnimator.
    /// </summary>
    /// <seealso cref="UnityEngine.MonoBehaviour" />
    public partial class StoreAnimator
    {
        /// <summary>
        /// Unity's API doesn't allow to restore mecanim transitions. When a save occurs during one,
        /// you can either choose to revert back to the starting point of the transition or skip it.
        /// </summary>
        public enum LoadingMode
        {
            /// <summary>
            /// The revert
            /// </summary>
            REVERT,

            /// <summary>
            /// The skip
            /// </summary>
            SKIP
        }
    }
}

namespace Lerp2API.Utility
{
    /// <summary>
    /// Enum FileBrowserType
    /// </summary>
    public enum FileBrowserType
    {
        /// <summary>
        /// The file
        /// </summary>
        File,

        /// <summary>
        /// The directory
        /// </summary>
        Directory
    }

    /// <summary>
    /// Enum BorderType
    /// </summary>
    public enum BorderType
    {
        /// <summary>
        /// The solid
        /// </summary>
        Solid,

        /// <summary>
        /// The dashed
        /// </summary>
        Dashed,

        /// <summary>
        /// The dotted
        /// </summary>
        Dotted,

        /// <summary>
        /// The double
        /// </summary>
        Double,

        /// <summary>
        /// The groove
        /// </summary>
        Groove,

        /// <summary>
        /// The ridge
        /// </summary>
        Ridge,

        /// <summary>
        /// The inset
        /// </summary>
        Inset,

        /// <summary>
        /// The outset
        /// </summary>
        Outset
    }

    /// <summary>
    /// Enum RequiredData
    /// </summary>
    public enum RequiredData
    {
        /// <summary>
        /// The tags
        /// </summary>
        Tags,

        /// <summary>
        /// The layers
        /// </summary>
        Layers
    }
}

namespace ICSharpCode.SharpZipLib.Zip.Compression
{
    /// <summary>
    /// Strategies for deflater
    /// </summary>
    public enum DeflateStrategy
    {
        /// <summary>
        /// The default strategy
        /// </summary>
        Default = 0,

        /// <summary>
        /// This strategy will only allow longer string repetitions.  It is
        /// useful for random data with a small character set.
        /// </summary>
        Filtered = 1,

        /// <summary>
        /// This strategy will not look for string repetitions at all.  It
        /// only encodes with Huffman trees (which means, that more common
        /// characters get a smaller encoding.
        /// </summary>
        HuffmanOnly = 2
    }
}

namespace ICSharpCode.SharpZipLib.Zip
{
    /// <summary>
    /// FastZip provides facilities for creating and extracting zip files.
    /// </summary>
    public partial class FastZip
    {
        /// <summary>
        /// Defines the desired handling when overwriting files during extraction.
        /// </summary>
        public enum Overwrite
        {
            /// <summary>
            /// Prompt the user to confirm overwriting
            /// </summary>
            Prompt,

            /// <summary>
            /// Never overwrite files.
            /// </summary>
            Never,

            /// <summary>
            /// Always overwrite files.
            /// </summary>
            Always
        }
    }

    /// <summary>
    /// Determines how entries are tested to see if they should use Zip64 extensions or not.
    /// </summary>
    public enum UseZip64
    {
        /// <summary>
        /// Zip64 will not be forced on entries during processing.
        /// </summary>
        /// <remarks>An entry can have this overridden if required <see cref="ZipEntry.ForceZip64"></see></remarks>
        Off,

        /// <summary>
        /// Zip64 should always be used.
        /// </summary>
        On,

        /// <summary>
        /// #ZipLib will determine use based on entry values when added to archive.
        /// </summary>
        Dynamic,
    }

    /// <summary>
    /// The kind of compression used for an entry in an archive
    /// </summary>
    public enum CompressionMethod
    {
        /// <summary>
        /// A direct copy of the file contents is held in the archive
        /// </summary>
        Stored = 0,

        /// <summary>
        /// Common Zip compression method using a sliding dictionary
        /// of up to 32KB and secondary compression from Huffman/Shannon-Fano trees
        /// </summary>
        Deflated = 8,

        /// <summary>
        /// An extension to deflate with a 64KB window. Not supported by #Zip currently
        /// </summary>
        Deflate64 = 9,

        /// <summary>
        /// BZip2 compression. Not supported by #Zip.
        /// </summary>
        BZip2 = 11,

        /// <summary>
        /// WinZip special for AES encryption, Now supported by #Zip.
        /// </summary>
        WinZipAES = 99,
    }

    /// <summary>
    /// Identifies the encryption algorithm used for an entry
    /// </summary>
    public enum EncryptionAlgorithm
    {
        /// <summary>
        /// No encryption has been used.
        /// </summary>
        None = 0,

        /// <summary>
        /// Encrypted using PKZIP 2.0 or 'classic' encryption.
        /// </summary>
        PkzipClassic = 1,

        /// <summary>
        /// DES encryption has been used.
        /// </summary>
        Des = 0x6601,

        /// <summary>
        /// RCS encryption has been used for encryption.
        /// </summary>
        RC2 = 0x6602,

        /// <summary>
        /// Triple DES encryption with 168 bit keys has been used for this entry.
        /// </summary>
        TripleDes168 = 0x6603,

        /// <summary>
        /// Triple DES with 112 bit keys has been used for this entry.
        /// </summary>
        TripleDes112 = 0x6609,

        /// <summary>
        /// AES 128 has been used for encryption.
        /// </summary>
        Aes128 = 0x660e,

        /// <summary>
        /// AES 192 has been used for encryption.
        /// </summary>
        Aes192 = 0x660f,

        /// <summary>
        /// AES 256 has been used for encryption.
        /// </summary>
        Aes256 = 0x6610,

        /// <summary>
        /// RC2 corrected has been used for encryption.
        /// </summary>
        RC2Corrected = 0x6702,

        /// <summary>
        /// Blowfish has been used for encryption.
        /// </summary>
        Blowfish = 0x6720,

        /// <summary>
        /// Twofish has been used for encryption.
        /// </summary>
        Twofish = 0x6721,

        /// <summary>
        /// RC4 has been used for encryption.
        /// </summary>
        RC4 = 0x6801,

        /// <summary>
        /// An unknown algorithm has been used for encryption.
        /// </summary>
        Unknown = 0xffff
    }

    /// <summary>
    /// Defines the contents of the general bit flags field for an archive entry.
    /// </summary>
    [Flags]
    public enum GeneralBitFlags : int
    {
        /// <summary>
        /// Bit 0 if set indicates that the file is encrypted
        /// </summary>
        Encrypted = 0x0001,

        /// <summary>
        /// Bits 1 and 2 - Two bits defining the compression method (only for Method 6 Imploding and 8,9 Deflating)
        /// </summary>
        Method = 0x0006,

        /// <summary>
        /// Bit 3 if set indicates a trailing data desciptor is appended to the entry data
        /// </summary>
        Descriptor = 0x0008,

        /// <summary>
        /// Bit 4 is reserved for use with method 8 for enhanced deflation
        /// </summary>
        ReservedPKware4 = 0x0010,

        /// <summary>
        /// Bit 5 if set indicates the file contains Pkzip compressed patched data.
        /// Requires version 2.7 or greater.
        /// </summary>
        Patched = 0x0020,

        /// <summary>
        /// Bit 6 if set indicates strong encryption has been used for this entry.
        /// </summary>
        StrongEncryption = 0x0040,

        /// <summary>
        /// Bit 7 is currently unused
        /// </summary>
        Unused7 = 0x0080,

        /// <summary>
        /// Bit 8 is currently unused
        /// </summary>
        Unused8 = 0x0100,

        /// <summary>
        /// Bit 9 is currently unused
        /// </summary>
        Unused9 = 0x0200,

        /// <summary>
        /// Bit 10 is currently unused
        /// </summary>
        Unused10 = 0x0400,

        /// <summary>
        /// Bit 11 if set indicates the filename and
        /// comment fields for this file must be encoded using UTF-8.
        /// </summary>
        UnicodeText = 0x0800,

        /// <summary>
        /// Bit 12 is documented as being reserved by PKware for enhanced compression.
        /// </summary>
        EnhancedCompress = 0x1000,

        /// <summary>
        /// Bit 13 if set indicates that values in the local header are masked to hide
        /// their actual values, and the central directory is encrypted.
        /// </summary>
        /// <remarks>
        /// Used when encrypting the central directory contents.
        /// </remarks>
        HeaderMasked = 0x2000,

        /// <summary>
        /// Bit 14 is documented as being reserved for use by PKware
        /// </summary>
        ReservedPkware14 = 0x4000,

        /// <summary>
        /// Bit 15 is documented as being reserved for use by PKware
        /// </summary>
        ReservedPkware15 = 0x8000
    }

    /// <summary>
    /// Defines known values for the <see cref="HostSystemID"/> property.
    /// </summary>
    public enum HostSystemID
    {
        /// <summary>
        /// Host system = MSDOS
        /// </summary>
        Msdos = 0,

        /// <summary>
        /// Host system = Amiga
        /// </summary>
        Amiga = 1,

        /// <summary>
        /// Host system = Open VMS
        /// </summary>
        OpenVms = 2,

        /// <summary>
        /// Host system = Unix
        /// </summary>
        Unix = 3,

        /// <summary>
        /// Host system = VMCms
        /// </summary>
        VMCms = 4,

        /// <summary>
        /// Host system = Atari ST
        /// </summary>
        AtariST = 5,

        /// <summary>
        /// Host system = OS2
        /// </summary>
        OS2 = 6,

        /// <summary>
        /// Host system = Macintosh
        /// </summary>
        Macintosh = 7,

        /// <summary>
        /// Host system = ZSystem
        /// </summary>
        ZSystem = 8,

        /// <summary>
        /// Host system = Cpm
        /// </summary>
        Cpm = 9,

        /// <summary>
        /// Host system = Windows NT
        /// </summary>
        WindowsNT = 10,

        /// <summary>
        /// Host system = MVS
        /// </summary>
        MVS = 11,

        /// <summary>
        /// Host system = VSE
        /// </summary>
        Vse = 12,

        /// <summary>
        /// Host system = Acorn RISC
        /// </summary>
        AcornRisc = 13,

        /// <summary>
        /// Host system = VFAT
        /// </summary>
        Vfat = 14,

        /// <summary>
        /// Host system = Alternate MVS
        /// </summary>
        AlternateMvs = 15,

        /// <summary>
        /// Host system = BEOS
        /// </summary>
        BeOS = 16,

        /// <summary>
        /// Host system = Tandem
        /// </summary>
        Tandem = 17,

        /// <summary>
        /// Host system = OS400
        /// </summary>
        OS400 = 18,

        /// <summary>
        /// Host system = OSX
        /// </summary>
        OSX = 19,

        /// <summary>
        /// Host system = WinZIP AES
        /// </summary>
        WinZipAES = 99,
    }

    /// <summary>
    /// Class ZipEntryFactory.
    /// </summary>
    /// <seealso cref="ICSharpCode.SharpZipLib.Zip.IEntryFactory" />
    public partial class ZipEntryFactory
    {
        /// <summary>
        /// Defines the possible values to be used for the <see cref="ZipEntry.DateTime"/>.
        /// </summary>
        public enum TimeSetting
        {
            /// <summary>
            /// Use the recorded LastWriteTime value for the file.
            /// </summary>
            LastWriteTime,

            /// <summary>
            /// Use the recorded LastWriteTimeUtc value for the file
            /// </summary>
            LastWriteTimeUtc,

            /// <summary>
            /// Use the recorded CreateTime value for the file.
            /// </summary>
            CreateTime,

            /// <summary>
            /// Use the recorded CreateTimeUtc value for the file.
            /// </summary>
            CreateTimeUtc,

            /// <summary>
            /// Use the recorded LastAccessTime value for the file.
            /// </summary>
            LastAccessTime,

            /// <summary>
            /// Use the recorded LastAccessTimeUtc value for the file.
            /// </summary>
            LastAccessTimeUtc,

            /// <summary>
            /// Use a fixed value.
            /// </summary>
            /// <remarks>The actual <see cref="DateTime"/> value used can be
            /// specified via the <see cref="ZipEntryFactory(DateTime)"/> constructor or
            /// using the <see cref="ZipEntryFactory(TimeSetting)"/> with the setting set
            /// to <see cref="TimeSetting.Fixed"/> which will use the <see cref="DateTime"/> when this class was constructed.
            /// The <see cref="FixedDateTime"/> property can also be used to set this value.</remarks>
            Fixed,
        }
    }

    /// <summary>
    /// Class representing extended unix date time values.
    /// </summary>
    public partial class ExtendedUnixData
    {
        /// <summary>
        /// Flags indicate which values are included in this instance.
        /// </summary>
        [Flags]
        public enum Flags : byte
        {
            /// <summary>
            /// The modification time is included
            /// </summary>
            ModificationTime = 0x01,

            /// <summary>
            /// The access time is included
            /// </summary>
            AccessTime = 0x02,

            /// <summary>
            /// The create time is included.
            /// </summary>
            CreateTime = 0x04,
        }
    }

    /// <summary>
    /// The strategy to apply to testing.
    /// </summary>
    public enum TestStrategy
    {
        /// <summary>
        /// Find the first error only.
        /// </summary>
        FindFirstError,

        /// <summary>
        /// Find all possible errors.
        /// </summary>
        FindAllErrors,
    }

    /// <summary>
    /// The operation in progress reported by a <see cref="ZipTestResultHandler"/> during testing.
    /// </summary>
    /// <seealso cref="ZipFile.TestArchive(bool)">TestArchive</seealso>
    public enum TestOperation
    {
        /// <summary>
        /// Setting up testing.
        /// </summary>
        Initialising,

        /// <summary>
        /// Testing an individual entries header
        /// </summary>
        EntryHeader,

        /// <summary>
        /// Testing an individual entries data
        /// </summary>
        EntryData,

        /// <summary>
        /// Testing an individual entry has completed.
        /// </summary>
        EntryComplete,

        /// <summary>
        /// Running miscellaneous tests
        /// </summary>
        MiscellaneousTests,

        /// <summary>
        /// Testing is complete
        /// </summary>
        Complete,
    }

    /// <summary>
    /// The possible ways of <see cref="ZipFile.CommitUpdate()">applying updates</see> to an archive.
    /// </summary>
    public enum FileUpdateMode
    {
        /// <summary>
        /// Perform all updates on temporary files ensuring that the original file is saved.
        /// </summary>
        Safe,

        /// <summary>
        /// Update the archive directly, which is faster but less safe.
        /// </summary>
        Direct,
    }
}

namespace Lerp2API.Controllers.PersonView
{
    /// <summary>
    /// Enum PersonView
    /// </summary>
    public enum PersonView
    {
        /// <summary>
        /// The first
        /// </summary>
        First = 1,

        /// <summary>
        /// The third
        /// </summary>
        Third = 3
    }
}

namespace Lerp2API.Utility.CSG
{
    /// <summary>
    /// Enum CSGType
    /// </summary>
    public enum CSGType
    {
        /// <summary>
        /// The union
        /// </summary>
        Union,

        /// <summary>
        /// The difference
        /// </summary>
        Difference,

        /// <summary>
        /// The intersection
        /// </summary>
        Intersection
    }
}

namespace TeamUtility.IO
{
    /// <summary>
    /// Enum InputDPADButton
    /// </summary>
    public enum InputDPADButton
    {
        /// <summary>
        /// The left
        /// </summary>
        Left,

        /// <summary>
        /// The right
        /// </summary>
        Right,

        /// <summary>
        /// Up
        /// </summary>
        Up,

        /// <summary>
        /// Down
        /// </summary>
        Down,

        /// <summary>
        /// The left up
        /// </summary>
        Left_Up,

        /// <summary>
        /// The right up
        /// </summary>
        Right_Up,

        /// <summary>
        /// The left down
        /// </summary>
        Left_Down,

        /// <summary>
        /// The right down
        /// </summary>
        Right_Down,

        /// <summary>
        /// Any
        /// </summary>
        Any
    }

    /// <summary>
    /// Enum InputTriggerButton
    /// </summary>
    public enum InputTriggerButton
    {
        /// <summary>
        /// The left
        /// </summary>
        Left,

        /// <summary>
        /// The right
        /// </summary>
        Right,

        /// <summary>
        /// Any
        /// </summary>
        Any
    }

    /// <summary>
    /// Enum InputTriggerAxis
    /// </summary>
    public enum InputTriggerAxis
    {
        /// <summary>
        /// The left
        /// </summary>
        Left,

        /// <summary>
        /// The right
        /// </summary>
        Right
    }

    /// <summary>
    /// Enum InputDPADAxis
    /// </summary>
    public enum InputDPADAxis
    {
        /// <summary>
        /// The horizontal
        /// </summary>
        Horizontal,

        /// <summary>
        /// The vertical
        /// </summary>
        Vertical
    }

    /// <summary>
    /// Enum InputDevice
    /// </summary>
    public enum InputDevice
    {
        /// <summary>
        /// The keyboard and mouse
        /// </summary>
        KeyboardAndMouse,

        /// <summary>
        /// The joystick
        /// </summary>
        Joystick
    }

    /// <summary>
    /// Enum InputEventType
    /// </summary>
    public enum InputEventType
    {
        /// <summary>
        /// The axis
        /// </summary>
        Axis,

        /// <summary>
        /// The button
        /// </summary>
        Button,

        /// <summary>
        /// The key
        /// </summary>
        Key
    }

    /// <summary>
    /// Enum InputState
    /// </summary>
    public enum InputState
    {
        /// <summary>
        /// The pressed
        /// </summary>
        Pressed,

        /// <summary>
        /// The released
        /// </summary>
        Released,

        /// <summary>
        /// The held
        /// </summary>
        Held
    }

    /// <summary>
    /// Class MappingWizard.
    /// </summary>
    public partial class MappingWizard
    {
        /// <summary>
        /// Enum ScanType
        /// </summary>
        public enum ScanType
        {
            /// <summary>
            /// The button
            /// </summary>
            Button,

            /// <summary>
            /// The axis
            /// </summary>
            Axis
        }
    }

    /// <summary>
    /// Class StandaloneInputModule.
    /// </summary>
    public partial class StandaloneInputModule
    {
        /// <summary>
        /// Enum InputMode
        /// </summary>
        [Obsolete("Mode is no longer needed on input module as it handles both mouse and keyboard simultaneously.", false)]
        public enum InputMode
        {
            /// <summary>
            /// The mouse
            /// </summary>
            Mouse,

            /// <summary>
            /// The buttons
            /// </summary>
            Buttons
        }
    }

    /// <summary>
    /// Enum InputType
    /// </summary>
    public enum InputType
    {
        /// <summary>
        /// The button
        /// </summary>
        Button,

        /// <summary>
        /// The mouse axis
        /// </summary>
        MouseAxis,

        /// <summary>
        /// The digital axis
        /// </summary>
        DigitalAxis,

        /// <summary>
        /// The analog axis
        /// </summary>
        AnalogAxis,

        /// <summary>
        /// The remote axis
        /// </summary>
        RemoteAxis,

        /// <summary>
        /// The remote button
        /// </summary>
        RemoteButton,

        /// <summary>
        /// The analog button
        /// </summary>
        AnalogButton
    }

    /// <summary>
    /// Enum PlayerID
    /// </summary>
    public enum PlayerID
    {
        /// <summary>
        /// The one
        /// </summary>
        One,

        /// <summary>
        /// The two
        /// </summary>
        Two,

        /// <summary>
        /// The three
        /// </summary>
        Three,

        /// <summary>
        /// The four
        /// </summary>
        Four
    }

    /// <summary>
    /// Enum ScanFlags
    /// </summary>
    public enum ScanFlags
    {
        /// <summary>
        /// The none
        /// </summary>
        None = 0,

        /// <summary>
        /// The key
        /// </summary>
        Key = 2,

        /// <summary>
        /// The joystick button
        /// </summary>
        JoystickButton = 4,

        /// <summary>
        /// The joystick axis
        /// </summary>
        JoystickAxis = 8,

        /// <summary>
        /// The mouse axis
        /// </summary>
        MouseAxis = 16
    }

    /// <summary>
    /// Class AdvancedInputEditor. This class cannot be inherited.
    /// </summary>
    public sealed partial class AdvancedInputEditor
    {
        /// <summary>
        /// Enum FileMenuOptions
        /// </summary>
        public enum FileMenuOptions
        {
            /// <summary>
            /// The overrite input settings
            /// </summary>
            OverriteInputSettings = 0,

            /// <summary>
            /// The create snapshot
            /// </summary>
            CreateSnapshot,

            /// <summary>
            /// The load snapshot
            /// </summary>
            LoadSnapshot,

            /// <summary>
            /// The export
            /// </summary>
            Export,

            /// <summary>
            /// The import
            /// </summary>
            Import,

            /// <summary>
            /// The import joystick mapping
            /// </summary>
            ImportJoystickMapping,

            /// <summary>
            /// The configure for input adapter
            /// </summary>
            ConfigureForInputAdapter,

            /// <summary>
            /// The create default input configuration
            /// </summary>
            CreateDefaultInputConfig
        }

        /// <summary>
        /// Enum EditMenuOptions
        /// </summary>
        public enum EditMenuOptions
        {
            /// <summary>
            /// The new input configuration
            /// </summary>
            NewInputConfiguration = 0,

            /// <summary>
            /// The new axis configuration
            /// </summary>
            NewAxisConfiguration,

            /// <summary>
            /// The duplicate
            /// </summary>
            Duplicate,

            /// <summary>
            /// The delete
            /// </summary>
            Delete,

            /// <summary>
            /// The delete all
            /// </summary>
            DeleteAll,

            /// <summary>
            /// The select target
            /// </summary>
            SelectTarget,

            /// <summary>
            /// The ignore timescale
            /// </summary>
            IgnoreTimescale,

            /// <summary>
            /// The dont destroy on load
            /// </summary>
            DontDestroyOnLoad,

            /// <summary>
            /// The copy
            /// </summary>
            Copy,

            /// <summary>
            /// The paste
            /// </summary>
            Paste
        }
    }
}

namespace UnityInputConverter.YamlDotNet.Core.Events
{
    /// <summary>
    /// Specifies the style of a mapping.
    /// </summary>
    public enum MappingStyle
    {
        /// <summary>
        /// Let the emitter choose the style.
        /// </summary>
        Any,

        /// <summary>
        /// The block mapping style.
        /// </summary>
        Block,

        /// <summary>
        /// The flow mapping style.
        /// </summary>
        Flow
    }

    /// <summary>
    /// Specifies the style of a sequence.
    /// </summary>
    public enum SequenceStyle
    {
        /// <summary>
        /// Let the emitter choose the style.
        /// </summary>
        Any,

        /// <summary>
        /// The block sequence style.
        /// </summary>
        Block,

        /// <summary>
        /// The flow sequence style.
        /// </summary>
        Flow
    }
}

namespace UnityInputConverter.YamlDotNet.Core
{
    /// <summary>
    /// Specifies the style of a YAML scalar.
    /// </summary>
    public enum ScalarStyle
    {
        /// <summary>
        /// Let the emitter choose the style.
        /// </summary>
        Any,

        /// <summary>
        /// The plain scalar style.
        /// </summary>
        Plain,

        /// <summary>
        /// The single-quoted scalar style.
        /// </summary>
        SingleQuoted,

        /// <summary>
        /// The double-quoted scalar style.
        /// </summary>
        DoubleQuoted,

        /// <summary>
        /// The literal scalar style.
        /// </summary>
        Literal,

        /// <summary>
        /// The folded scalar style.
        /// </summary>
        Folded,
    }
}

namespace UnityInputConverter.YamlDotNet.RepresentationModel
{
    /// <summary>
    /// Specifies the type of node in the representation model.
    /// </summary>
    public enum YamlNodeType
    {
        /// <summary>
        /// The node is a <see cref="YamlAliasNode"/>.
        /// </summary>
        Alias,

        /// <summary>
        /// The node is a <see cref="YamlMappingNode"/>.
        /// </summary>
        Mapping,

        /// <summary>
        /// The node is a <see cref="YamlScalarNode"/>.
        /// </summary>
        Scalar,

        /// <summary>
        /// The node is a <see cref="YamlSequenceNode"/>.
        /// </summary>
        Sequence
    }
}

namespace UnityInputConverter.YamlDotNet.Serialization
{
    /// <summary>
    /// Options that control the serialization process.
    /// </summary>
    [Flags]
    public enum SerializationOptions
    {
        /// <summary>
        /// Serializes using the default options
        /// </summary>
        None = 0,

        /// <summary>
        /// Ensures that it will be possible to deserialize the serialized objects.
        /// </summary>
        Roundtrip = 1,

        /// <summary>
        /// If this flag is specified, if the same object appears more than once in the
        /// serialization graph, it will be serialized each time instead of just once.
        /// </summary>
        /// <remarks>
        /// If the serialization graph contains circular references and this flag is set,
        /// a StackOverflowException will be thrown.
        /// If this flag is not set, there is a performance penalty because the entire
        /// object graph must be walked twice.
        /// </remarks>
        DisableAliases = 2,

        /// <summary>
        /// Forces every value to be serialized, even if it is the default value for that type.
        /// </summary>
        EmitDefaults = 4,

        /// <summary>
        /// Ensures that the result of the serialization is valid JSON.
        /// </summary>
        JsonCompatible = 8,

        /// <summary>
        /// Use the static type of values instead of their actual type.
        /// </summary>
        DefaultToStaticType = 16,
    }
}

namespace Lerp2APIEditor.CustomDrawers
{
    /// <summary>
    /// Enum MessengerMode
    /// </summary>
    public enum MessengerMode
    {
        /// <summary>
        /// The dont require listener
        /// </summary>
        DONT_REQUIRE_LISTENER,

        /// <summary>
        /// The require listener
        /// </summary>
        REQUIRE_LISTENER,
    }
}

namespace UnityStandardAssets.ImageEffects
{
    /// <summary>
    /// Enum ColorCorrectionMode
    /// </summary>
    public enum ColorCorrectionMode
    {
        /// <summary>
        /// The simple
        /// </summary>
        Simple = 0,

        /// <summary>
        /// The advanced
        /// </summary>
        Advanced = 1
    }
}

namespace Lerp2APIEditor.Utility.GUI_Extensions
{
    /// <summary>
    /// Enum ReferType
    /// </summary>
    public enum ReferType
    {
        /// <summary>
        /// The editor
        /// </summary>
        Editor,

        /// <summary>
        /// The editor window
        /// </summary>
        EditorWindow
    }
}

namespace Malee.Editor
{
    /// <summary>
    /// Enum ElementDisplayType
    /// </summary>
    public enum ElementDisplayType
    {
        /// <summary>
        /// The automatic
        /// </summary>
        Auto,

        /// <summary>
        /// The expandable
        /// </summary>
        Expandable,

        /// <summary>
        /// The single line
        /// </summary>
        SingleLine
    }
}

namespace Lerp2API.Utility.StandardInstaller
{
    /// <summary>
    /// Enum AssetLocation
    /// </summary>
    [Serializable]
    public enum AssetLocation
    {
        /// <summary>
        /// The local
        /// </summary>
        Local,

        /// <summary>
        /// The URL
        /// </summary>
        URL,

        /// <summary>
        /// The HDD
        /// </summary>
        HDD
    }
}

namespace Lerp2APIEditor.Utility.UnityLib.Two_D.TileMaps
{
    /// <summary>
    /// Enum FrameMode
    /// </summary>
    public enum FrameMode
    {
        /// <summary>
        /// The framed
        /// </summary>
        Framed,

        /// <summary>
        /// The pixel perfect
        /// </summary>
        PixelPerfect,

        /// <summary>
        /// The half way
        /// </summary>
        HalfWay
    }
}