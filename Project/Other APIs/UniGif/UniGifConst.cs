/*
UniGif
Copyright (c) 2015 WestHillApps (Hironari Nishioka)
This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using UnityEngine;
using Debug = Lerp2API._Debug.Debug;

/// <summary>
/// Class UniGif.
/// </summary>
public static partial class UniGif
{
    /// <summary>
    /// Struct GifTexture
    /// </summary>
    public struct GifTexture
    {
        /// <summary>
        /// The texture2d
        /// </summary>
        public Texture2D texture2d;

        /// <summary>
        /// The delay sec
        /// </summary>
        public float delaySec;

        /// <summary>
        /// Initializes a new instance of the <see cref="GifTexture"/> struct.
        /// </summary>
        /// <param name="texture2d">The texture2d.</param>
        /// <param name="delaySec">The delay sec.</param>
        public GifTexture(Texture2D texture2d, float delaySec)
        {
            this.texture2d = texture2d;
            this.delaySec = delaySec;
        }
    }

    /// <summary>
    /// GIF Data Format
    /// </summary>
    private struct GifData
    {
        // Signature
        /// <summary>
        /// The sig0
        /// </summary>
        public byte sig0, sig1, sig2;

        // Version
        /// <summary>
        /// The ver0
        /// </summary>
        public byte ver0, ver1, ver2;

        // Logical Screen Width
        /// <summary>
        /// The logical screen width
        /// </summary>
        public ushort logicalScreenWidth;

        // Logical Screen Height
        /// <summary>
        /// The logical screen height
        /// </summary>
        public ushort logicalScreenHeight;

        // Global Color Table Flag
        /// <summary>
        /// The global color table flag
        /// </summary>
        public bool globalColorTableFlag;

        // Color Resolution
        /// <summary>
        /// The color resolution
        /// </summary>
        public int colorResolution;

        // Sort Flag
        /// <summary>
        /// The sort flag
        /// </summary>
        public bool sortFlag;

        // Size of Global Color Table
        /// <summary>
        /// The size of global color table
        /// </summary>
        public int sizeOfGlobalColorTable;

        // Background Color Index
        /// <summary>
        /// The bg color index
        /// </summary>
        public byte bgColorIndex;

        // Pixel Aspect Ratio
        /// <summary>
        /// The pixel aspect ratio
        /// </summary>
        public byte pixelAspectRatio;

        // Global Color Table
        /// <summary>
        /// The global color table
        /// </summary>
        public List<byte[]> globalColorTable;

        // ImageBlock
        /// <summary>
        /// The image block list
        /// </summary>
        public List<ImageBlock> imageBlockList;

        // GraphicControlExtension
        /// <summary>
        /// The graphic control ex list
        /// </summary>
        public List<GraphicControlExtension> graphicCtrlExList;

        // Comment Extension
        /// <summary>
        /// The comment ex list
        /// </summary>
        public List<CommentExtension> commentExList;

        // Plain Text Extension
        /// <summary>
        /// The plain text ex list
        /// </summary>
        public List<PlainTextExtension> plainTextExList;

        // Application Extension
        /// <summary>
        /// The application ex
        /// </summary>
        public ApplicationExtension appEx;

        // Trailer
        /// <summary>
        /// The trailer
        /// </summary>
        public byte trailer;

        /// <summary>
        /// Gets the signature.
        /// </summary>
        /// <value>The signature.</value>
        public string signature
        {
            get
            {
                char[] c = { (char)sig0, (char)sig1, (char)sig2 };
                return new string(c);
            }
        }

        /// <summary>
        /// Gets the version.
        /// </summary>
        /// <value>The version.</value>
        public string version
        {
            get
            {
                char[] c = { (char)ver0, (char)ver1, (char)ver2 };
                return new string(c);
            }
        }

        /// <summary>
        /// Dumps this instance.
        /// </summary>
        public void Dump()
        {
            Debug.Log("GIF Type: " + signature + "-" + version);
            Debug.Log("Image Size: " + logicalScreenWidth + "x" + logicalScreenHeight);
            Debug.Log("Animation Image Count: " + imageBlockList.Count);
            Debug.Log("Animation Loop Count (0 is infinite): " + appEx.loopCount);
            Debug.Log("Application Identifier: " + appEx.applicationIdentifier);
            Debug.Log("Application Authentication Code: " + appEx.applicationAuthenticationCode);
        }
    }

    /// <summary>
    /// Image Block
    /// </summary>
    private struct ImageBlock
    {
        // Image Separator
        /// <summary>
        /// The image separator
        /// </summary>
        public byte imageSeparator;

        // Image Left Position
        /// <summary>
        /// The image left position
        /// </summary>
        public ushort imageLeftPosition;

        // Image Top Position
        /// <summary>
        /// The image top position
        /// </summary>
        public ushort imageTopPosition;

        // Image Width
        /// <summary>
        /// The image width
        /// </summary>
        public ushort imageWidth;

        // Image Height
        /// <summary>
        /// The image height
        /// </summary>
        public ushort imageHeight;

        // Local Color Table Flag
        /// <summary>
        /// The local color table flag
        /// </summary>
        public bool localColorTableFlag;

        // Interlace Flag
        /// <summary>
        /// The interlace flag
        /// </summary>
        public bool interlaceFlag;

        // Sort Flag
        /// <summary>
        /// The sort flag
        /// </summary>
        public bool sortFlag;

        // Size of Local Color Table
        /// <summary>
        /// The size of local color table
        /// </summary>
        public int sizeOfLocalColorTable;

        // Local Color Table
        /// <summary>
        /// The local color table
        /// </summary>
        public List<byte[]> localColorTable;

        // LZW Minimum Code Size
        /// <summary>
        /// The LZW minimum code size
        /// </summary>
        public byte LzwMinimumCodeSize;

        // Block Size & Image Data List
        /// <summary>
        /// The image data list
        /// </summary>
        public List<ImageDataBlock> imageDataList;

        /// <summary>
        /// Struct ImageDataBlock
        /// </summary>
        public struct ImageDataBlock
        {
            // Block Size
            /// <summary>
            /// The block size
            /// </summary>
            public byte blockSize;

            // Image Data
            /// <summary>
            /// The image data
            /// </summary>
            public byte[] imageData;
        }
    }

    /// <summary>
    /// Graphic Control Extension
    /// </summary>
    private struct GraphicControlExtension
    {
        // Extension Introducer
        /// <summary>
        /// The extension introducer
        /// </summary>
        public byte extensionIntroducer;

        // Graphic Control Label
        /// <summary>
        /// The graphic control label
        /// </summary>
        public byte graphicControlLabel;

        // Block Size
        /// <summary>
        /// The block size
        /// </summary>
        public byte blockSize;

        // Disposal Mothod
        /// <summary>
        /// The disposal method
        /// </summary>
        public ushort disposalMethod;

        // Transparent Color Flag
        /// <summary>
        /// The transparent color flag
        /// </summary>
        public bool transparentColorFlag;

        // Delay Time
        /// <summary>
        /// The delay time
        /// </summary>
        public ushort delayTime;

        // Transparent Color Index
        /// <summary>
        /// The transparent color index
        /// </summary>
        public byte transparentColorIndex;

        // Block Terminator
        /// <summary>
        /// The block terminator
        /// </summary>
        public byte blockTerminator;
    }

    /// <summary>
    /// Comment Extension
    /// </summary>
    private struct CommentExtension
    {
        // Extension Introducer
        /// <summary>
        /// The extension introducer
        /// </summary>
        public byte extensionIntroducer;

        // Comment Label
        /// <summary>
        /// The comment label
        /// </summary>
        public byte commentLabel;

        // Block Size & Comment Data List
        /// <summary>
        /// The comment data list
        /// </summary>
        public List<CommentDataBlock> commentDataList;

        /// <summary>
        /// Struct CommentDataBlock
        /// </summary>
        public struct CommentDataBlock
        {
            // Block Size
            /// <summary>
            /// The block size
            /// </summary>
            public byte blockSize;

            // Image Data
            /// <summary>
            /// The comment data
            /// </summary>
            public byte[] commentData;
        }
    }

    /// <summary>
    /// Plain Text Extension
    /// </summary>
    private struct PlainTextExtension
    {
        // Extension Introducer
        /// <summary>
        /// The extension introducer
        /// </summary>
        public byte extensionIntroducer;

        // Plain Text Label
        /// <summary>
        /// The plain text label
        /// </summary>
        public byte plainTextLabel;

        // Block Size
        /// <summary>
        /// The block size
        /// </summary>
        public byte blockSize;

        // Block Size & Plain Text Data List
        /// <summary>
        /// The plain text data list
        /// </summary>
        public List<PlainTextDataBlock> plainTextDataList;

        /// <summary>
        /// Struct PlainTextDataBlock
        /// </summary>
        public struct PlainTextDataBlock
        {
            // Block Size
            /// <summary>
            /// The block size
            /// </summary>
            public byte blockSize;

            // Plain Text Data
            /// <summary>
            /// The plain text data
            /// </summary>
            public byte[] plainTextData;
        }
    }

    /// <summary>
    /// Application Extension
    /// </summary>
    private struct ApplicationExtension
    {
        // Extension Introducer
        /// <summary>
        /// The extension introducer
        /// </summary>
        public byte extensionIntroducer;

        // Extension Label
        /// <summary>
        /// The extension label
        /// </summary>
        public byte extensionLabel;

        // Block Size
        /// <summary>
        /// The block size
        /// </summary>
        public byte blockSize;

        // Application Identifier
        /// <summary>
        /// The application id1
        /// </summary>
        public byte appId1, appId2, appId3, appId4, appId5, appId6, appId7, appId8;

        // Application Authentication Code
        /// <summary>
        /// The application authentication code1
        /// </summary>
        public byte appAuthCode1, appAuthCode2, appAuthCode3;

        // Block Size & Application Data List
        /// <summary>
        /// The application data list
        /// </summary>
        public List<ApplicationDataBlock> appDataList;

        /// <summary>
        /// Struct ApplicationDataBlock
        /// </summary>
        public struct ApplicationDataBlock
        {
            // Block Size
            /// <summary>
            /// The block size
            /// </summary>
            public byte blockSize;

            // Application Data
            /// <summary>
            /// The application data
            /// </summary>
            public byte[] applicationData;
        }

        /// <summary>
        /// Gets the application identifier.
        /// </summary>
        /// <value>The application identifier.</value>
        public string applicationIdentifier
        {
            get
            {
                char[] c = { (char)appId1, (char)appId2, (char)appId3, (char)appId4, (char)appId5, (char)appId6, (char)appId7, (char)appId8 };
                return new string(c);
            }
        }

        /// <summary>
        /// Gets the application authentication code.
        /// </summary>
        /// <value>The application authentication code.</value>
        public string applicationAuthenticationCode
        {
            get
            {
                char[] c = { (char)appAuthCode1, (char)appAuthCode2, (char)appAuthCode3 };
                return new string(c);
            }
        }

        /// <summary>
        /// Gets the loop count.
        /// </summary>
        /// <value>The loop count.</value>
        public int loopCount
        {
            get
            {
                if (appDataList == null || appDataList.Count < 1 ||
                    appDataList[0].applicationData.Length < 3 ||
                    appDataList[0].applicationData[0] != 0x01)
                {
                    return 0;
                }
                return BitConverter.ToUInt16(appDataList[0].applicationData, 1);
            }
        }
    }
}