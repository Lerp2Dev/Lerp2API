using System;
using System.IO;

namespace Serialization
{
    /// <summary>
    /// Class BinarySerializer. This class cannot be inherited.
    /// </summary>
    /// <seealso cref="Serialization.IStorage" />
    public sealed class BinarySerializer : IStorage //, IDisposable //WIP
    {
        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <value>The data.</value>
        public byte[] Data { get; private set; }
        private MemoryStream _myStream;

        /// <summary>
        /// Used when serializing
        /// </summary>
        public BinarySerializer() { }

        /// <summary>
        /// Used when deserializaing
        /// </summary>
        /// <param name="data"></param>
        public BinarySerializer(byte[] data)
        {
            Data = data;
        }

        #region writing

        private BinaryWriter _writer;

        private void EncodeType(object item, Type storedType)
        {
            if (item == null)
            {
                WriteSimpleValue((ushort)0xFFFE);
                return;
            }

            var itemType = item.GetType();

            //If this isn't a simple type, then this might be a subclass so we need to
            //store the type
            if (storedType == null || storedType != item.GetType() || UnitySerializer.Verbose)
            {
                //Write the type identifier
                var tpId = UnitySerializer.GetTypeId(itemType);
                WriteSimpleValue(tpId);
            }
            else {
                //Write a dummy identifier
                WriteSimpleValue((ushort)0xFFFF);
            }
        }

        /// <summary>
        /// Called when serializing a new object, the Entry parameter may have MustHaveName set
        /// when this is true the name must be persisted as is so that the property or field can
        /// be set when retrieving the data.
        /// If this routine returns TRUE then no further processing is executed and the object
        /// is presumed persisted in its entirety
        /// </summary>
        /// <param name="entry">The item being serialized</param>
        /// <param name="id">The identifier.</param>
        /// <returns>Normally FALSE.  True if the object is already fully persisted</returns>
        public bool StartSerializing(Entry entry, int id)
        {
            if (entry.MustHaveName)
            {
                ushort nameID = UnitySerializer.GetPropertyDefinitionId(entry.Name);
                WriteSimpleValue(nameID);
            }
            var item = entry.Value ?? new UnitySerializer.Nuller();
            EncodeType(item, entry.StoredType);
            return false;
        }

        /// <summary>
        /// Starts the serialization process, the serializer should initialize and wait for data
        /// </summary>
        public void StartSerializing()
        {
            _myStream = new MemoryStream();
            _writer = new BinaryWriter(_myStream);
            UnitySerializer.PushKnownTypes();
            UnitySerializer.PushPropertyNames();
        }

        /// <summary>
        /// Called when serialization is complete, should return the data or a key
        /// encoded as a byte array that will be used to reinitialize the serializer
        /// later
        /// </summary>
        public void FinishedSerializing() 
        {
            _writer.Flush();
            _writer.Close();
            _myStream.Flush();
            var data = _myStream.ToArray();
            _myStream.Close();
            _myStream = null;

            var stream = new MemoryStream();
            var outputWr = new BinaryWriter(stream);
            outputWr.Write("SerV10");
            //New, store the verbose property
            outputWr.Write(UnitySerializer.Verbose);
            if (UnitySerializer.SerializationScope.IsPrimaryScope) {
                outputWr.Write(UnitySerializer._knownTypesLookup.Count);
                foreach (var kt in UnitySerializer._knownTypesLookup.Keys) {
                    outputWr.Write(kt.FullName);
                }
                outputWr.Write(UnitySerializer._propertyLookup.Count);
                foreach (var pi in UnitySerializer._propertyLookup.Keys) {
                    outputWr.Write(pi);
                }
            }
            else {
                outputWr.Write(0);
                outputWr.Write(0);
            }
            outputWr.Write(data.Length);
            outputWr.Write(data);
            outputWr.Flush();
            outputWr.Close();
            stream.Flush();

            Data = stream.ToArray();
            stream.Close();
            _writer = null;
            _reader = null;

            UnitySerializer.PopKnownTypes();
            UnitySerializer.PopPropertyNames();

        }


        //WIP
        /*public void FinishedSerializing()
        {
            _writer.Flush();
            _writer.Close();
            _myStream.Flush();
            var data = _myStream.ToArray();
            _myStream.Close();
            _myStream = null;

            using (var stream = new MemoryStream())
            {
                using (var outputWr = new BinaryWriter(stream))
                {
                    outputWr.Write("SerV10");
                    //New, store the verbose property
                    outputWr.Write(UnitySerializer.Verbose);
                    if (UnitySerializer.SerializationScope.IsPrimaryScope)
                    {
                        outputWr.Write(UnitySerializer._knownTypesLookup.Count);
                        foreach (var kt in UnitySerializer._knownTypesLookup.Keys)
                        {
                            outputWr.Write(kt.FullName);
                        }
                        outputWr.Write(UnitySerializer._propertyLookup.Count);
                        foreach (var pi in UnitySerializer._propertyLookup.Keys)
                        {
                            outputWr.Write(pi);
                        }
                    }
                    else 
                    {
                        outputWr.Write(0);
                        outputWr.Write(0);
                    }
                    outputWr.Write(data.Length);
                    outputWr.Write(data);
                    outputWr.Flush();
                }
                stream.Flush();
                Data = stream.ToArray();
            }
            _writer = null;
            _reader = null;

            UnitySerializer.PopKnownTypes();
            UnitySerializer.PopPropertyNames();
        }*/

        /// <summary>
        /// Gets a value indicating whether [supports on demand].
        /// </summary>
        /// <value><c>true</c> if [supports on demand]; otherwise, <c>false</c>.</value>
        public bool SupportsOnDemand
        {
            get { return false; }
        }

        /// <summary>
        /// Begins the on demand.
        /// </summary>
        /// <param name="id">The identifier.</param>
        public void BeginOnDemand(int id)
        {
        }

        /// <summary>
        /// Ends the on demand.
        /// </summary>
        public void EndOnDemand()
        {
        }

        /// <summary>
        /// Begins the write object.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="wasSeen">if set to <c>true</c> [was seen].</param>
        public void BeginWriteObject(int id, Type objectType, bool wasSeen)
        {
            if (objectType == null)
            {
                WriteSimpleValue('X');
            }
            else if (wasSeen)
            {
                WriteSimpleValue('S');
                WriteSimpleValue(id);
            }
            else {
                WriteSimpleValue('O');
            }
        }

        /// <summary>
        /// Begins the write properties.
        /// </summary>
        /// <param name="count">The count.</param>
        public void BeginWriteProperties(int count)
        {
            if (count > 250)
            {
                WriteSimpleValue((byte)255);
                WriteSimpleValue(count);
            }
            else {
                WriteSimpleValue((byte)count);
            }
        }

        /// <summary>
        /// Begins the write fields.
        /// </summary>
        /// <param name="count">The count.</param>
        public void BeginWriteFields(int count)
        {
            if (count > 250)
            {
                WriteSimpleValue((byte)255);
                WriteSimpleValue(count);
            }
            else {
                WriteSimpleValue((byte)count);
            }
        }

        /// <summary>
        /// Writes the simple value.
        /// </summary>
        /// <param name="value">The value.</param>
        public void WriteSimpleValue(object value)
        {
            UnitySerializer.WriteValue(_writer, value);
        }

        /// <summary>
        /// Begins the write list.
        /// </summary>
        /// <param name="count">The count.</param>
        /// <param name="listType">Type of the list.</param>
        public void BeginWriteList(int count, Type listType)
        {
            WriteSimpleValue(count);
        }

        /// <summary>
        /// Begins the write dictionary.
        /// </summary>
        /// <param name="count">The count.</param>
        /// <param name="dictionaryType">Type of the dictionary.</param>
        public void BeginWriteDictionary(int count, Type dictionaryType)
        {
            WriteSimpleValue(count);
        }

        /// <summary>
        /// Writes the simple array.
        /// </summary>
        /// <param name="count">The count.</param>
        /// <param name="array">The array.</param>
        public void WriteSimpleArray(int count, Array array)
        {
            WriteSimpleValue(count);

            var elementType = array.GetType().GetElementType();
            if (elementType == typeof(byte))
            {
                UnitySerializer.WriteValue(_writer, array);
            }
            else if (elementType.IsPrimitive)
            {
                var ba = new byte[Buffer.ByteLength(array)];
                Buffer.BlockCopy(array, 0, ba, 0, ba.Length);
                UnitySerializer.WriteValue(_writer, ba);
            }
            else {
                for (int i = 0; i < count; i++)
                {
                    var v = array.GetValue(i);
                    if (v == null)
                    {
                        UnitySerializer.WriteValue(_writer, (byte)0);
                    }
                    else {
                        UnitySerializer.WriteValue(_writer, (byte)1);
                        UnitySerializer.WriteValue(_writer, v);
                    }
                }
            }
        }

        /// <summary>
        /// Begins the multi dimension array.
        /// </summary>
        /// <param name="arrayType">Type of the array.</param>
        /// <param name="dimensions">The dimensions.</param>
        /// <param name="count">The count.</param>
        public void BeginMultiDimensionArray(Type arrayType, int dimensions, int count)
        {
            WriteSimpleValue(-1);
            WriteSimpleValue(dimensions);
            WriteSimpleValue(count);
        }

        /// <summary>
        /// Writes the array dimension.
        /// </summary>
        /// <param name="dimension">The dimension.</param>
        /// <param name="count">The count.</param>
        public void WriteArrayDimension(int dimension, int count)
        {
            WriteSimpleValue(count);
        }

        /// <summary>
        /// Begins the write object array.
        /// </summary>
        /// <param name="count">The count.</param>
        /// <param name="arrayType">Type of the array.</param>
        public void BeginWriteObjectArray(int count, Type arrayType)
        {
            WriteSimpleValue(count);
        }

        /// <summary>
        /// Reads a simple type (or array of bytes) from storage
        /// </summary>
        /// <param name="fields">The fields.</param>
        /// <returns>Entry[].</returns>
        public Entry[] ShouldWriteFields(Entry[] fields)
        {
            return fields;
        }

        /// <summary>
        /// Shoulds the write properties.
        /// </summary>
        /// <param name="properties">The properties.</param>
        /// <returns>Entry[].</returns>
        public Entry[] ShouldWriteProperties(Entry[] properties)
        {
            return properties;
        }

        #endregion writing

        #region reading

        private BinaryReader _reader;

        private Type DecodeType(Type storedType)
        {
            try
            {
                var tid = ReadSimpleValue<ushort>();
                if (tid == 0xffff)
                    return storedType;
                if (tid == 0xFFFE)
                {
                    return null;
                }
                if (tid >= 60000)
                {
                    try
                    {
                        return UnitySerializer.PrewarmLookup[tid - 60000];
                    }
                    catch
                    {
                        throw new Exception("Data stream appears corrupt, found a TYPE ID of " + tid.ToString());
                    }
                }

                storedType = UnitySerializer._knownTypesList[tid];
                return storedType;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Called when deserialization is complete, so that resources may be released
        /// </summary>
        public void FinishedDeserializing()
        {
            _reader.Close();
            _myStream.Close();
            _reader = null;
            _myStream = null;
            _writer = null;

            UnitySerializer.PopKnownTypes();
            UnitySerializer.PopPropertyNames();
        }

        //Gets the name from the stream
        /// <summary>
        /// Called to allow the storage to retrieve the name of the item being deserialized
        /// All entries must be named before a call to StartDeserializing, this enables
        /// the system to fill out the property setter and capture default stored type
        /// information before deserialization commences
        /// </summary>
        /// <param name="entry">The entry whose name should be filled in</param>
        /// <exception cref="Exception">Data stream may be corrupt, found an id of " + id + " when looking a property name id</exception>
        public void DeserializeGetName(Entry entry)
        {
            if (!entry.MustHaveName)
                return;
            var id = ReadSimpleValue<ushort>();
            try
            {
                entry.Name = id >= 50000 ? PreWarm.PrewarmNames[id - 50000] : UnitySerializer._propertyList[id];
            }
            catch
            {
                throw new Exception("Data stream may be corrupt, found an id of " + id + " when looking a property name id");
            }
        }

        /// <summary>
        /// Starts to deserialize the object
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        public object StartDeserializing(Entry entry)
        {
            var itemType = DecodeType(entry.StoredType);
            entry.StoredType = itemType;
            return null;
        }

        /// <summary>
        /// Begins the read property.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <returns>Entry.</returns>
        public Entry BeginReadProperty(Entry entry)
        {
            return entry;
        }

        /// <summary>
        /// Ends the read property.
        /// </summary>
        public void EndReadProperty()
        {
        }

        /// <summary>
        /// Begins the read field.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <returns>Entry.</returns>
        public Entry BeginReadField(Entry entry)
        {
            return entry;
        }

        /// <summary>
        /// Ends the read field.
        /// </summary>
        public void EndReadField()
        {
        }

        /// <summary>
        /// Starts the deserializing.
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public void StartDeserializing() {
            UnitySerializer.PushKnownTypes();
            UnitySerializer.PushPropertyNames();

            var stream = new MemoryStream(Data);
            var reader = new BinaryReader(stream);
            var version = reader.ReadString();
            UnitySerializer.currentVersion = int.Parse(version.Substring(4));
            if (UnitySerializer.currentVersion >= 3) {
                UnitySerializer.Verbose = reader.ReadBoolean();
            }

            var count = reader.ReadInt32();
            for (var i = 0; i < count; i++) {
                var typeName = reader.ReadString();
                var tp = UnitySerializer.GetTypeEx(typeName);
                if (tp == null) {
                    var map = new UnitySerializer.TypeMappingEventArgs {
                        TypeName = typeName
                    };
                    UnitySerializer.InvokeMapMissingType(map);
                    tp = map.UseType;
                }
                if (tp == null) {
                    throw new ArgumentException(string.Format("Cannot reference type {0} in this context", typeName));
                }

                UnitySerializer._knownTypesList.Add(tp);
            }
            count = reader.ReadInt32();
            for (var i = 0; i < count; i++) {
                UnitySerializer._propertyList.Add(reader.ReadString());
            }

            var data = reader.ReadBytes(reader.ReadInt32());

            _myStream = new MemoryStream(data);
            _reader = new BinaryReader(_myStream);
            reader.Close();
            stream.Close();
        }

        //WIP
        /*public void StartDeserializing()
        {
            UnitySerializer.PushKnownTypes();
            UnitySerializer.PushPropertyNames();

            using (var stream = new MemoryStream(Data))
            using (var reader = new BinaryReader(stream))
            {
                var version = reader.ReadString();
                UnitySerializer.currentVersion = int.Parse(version.Substring(4));
                if (UnitySerializer.currentVersion >= 3)
                {
                    UnitySerializer.Verbose = reader.ReadBoolean();
                }

                var count = reader.ReadInt32();
                for (var i = 0; i < count; i++)
                {
                    var typeName = reader.ReadString();
                    var tp = UnitySerializer.GetTypeEx(typeName);
                    if (tp == null)
                    {
                        var map = new UnitySerializer.TypeMappingEventArgs
                        {
                            TypeName = typeName
                        };
                        UnitySerializer.InvokeMapMissingType(map);
                        tp = map.UseType;
                    }
                    if (tp == null)
                        throw new ArgumentException(string.Format("Cannot reference type {0} in this context", typeName));
                    UnitySerializer._knownTypesList.Add(tp);
                }
                count = reader.ReadInt32();
                for (var i = 0; i < count; i++)
                    UnitySerializer._propertyList.Add(reader.ReadString());

                var data = reader.ReadBytes(reader.ReadInt32());

                _myStream = new MemoryStream(data);
                _reader = new BinaryReader(_myStream);
            }
        }*/

        /// <summary>
        /// Called when an object has deserialization complete
        /// </summary>
        /// <param name="entry">The entry.</param>
        public void FinishDeserializing(Entry entry)
        {
        }

        /// <summary>
        /// Reads the simple array.
        /// </summary>
        /// <param name="elementType">Type of the element.</param>
        /// <param name="count">The count.</param>
        /// <returns>Array.</returns>
        public Array ReadSimpleArray(Type elementType, int count)
        {
            if (count == -1)
                count = ReadSimpleValue<int>();

            if (elementType == typeof(byte))
                return ReadSimpleValue<byte[]>();
            if (elementType.IsPrimitive && UnitySerializer.currentVersion >= 6)
            {
                var ba = ReadSimpleValue<byte[]>();
                var a = Array.CreateInstance(elementType, count);
                Buffer.BlockCopy(ba, 0, a, 0, ba.Length);
                return a;
            }
            var result = Array.CreateInstance(elementType, count);
            if (UnitySerializer.currentVersion >= 8)
                for (var l = 0; l < count; l++)
                {
                    var go = (byte)ReadSimpleValue(typeof(byte));
                    result.SetValue(go != 0 ? ReadSimpleValue(elementType) : null, l);
                }
            else
                for (var l = 0; l < count; l++)
                    result.SetValue(ReadSimpleValue(elementType), l);
            return result;
        }

        /// <summary>
        /// Begins the read properties.
        /// </summary>
        /// <returns>System.Int32.</returns>
        public int BeginReadProperties()
        {
            var count = ReadSimpleValue<byte>();
            return count == 255 ? ReadSimpleValue<int>() : count;
        }

        /// <summary>
        /// Begins the read fields.
        /// </summary>
        /// <returns>System.Int32.</returns>
        public int BeginReadFields()
        {
            var count = ReadSimpleValue<byte>();
            return count == 255 ? ReadSimpleValue<int>() : count;
        }

        /// <summary>
        /// Reads the simple value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>T.</returns>
        public T ReadSimpleValue<T>()
        {
            return (T)ReadSimpleValue(typeof(T));
        }

        /// <summary>
        /// Reads the simple value.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>System.Object.</returns>
        public object ReadSimpleValue(Type type)
        {
            UnitySerializer.ReadAValue read;
            if (!UnitySerializer.Readers.TryGetValue(type, out read))
            {
                return _reader.ReadInt32();
            }
            return read(_reader);
        }

        /// <summary>
        /// Determines whether [is multi dimensional array] [the specified length].
        /// </summary>
        /// <param name="length">The length.</param>
        /// <returns><c>true</c> if [is multi dimensional array] [the specified length]; otherwise, <c>false</c>.</returns>
        public bool IsMultiDimensionalArray(out int length)
        {
            var count = ReadSimpleValue<int>();
            if (count == -1)
            {
                length = -1;
                return true;
            }
            length = count;
            return false;
        }

        /// <summary>
        /// Begins the read dictionary.
        /// </summary>
        /// <param name="keyType">Type of the key.</param>
        /// <param name="valueType">Type of the value.</param>
        /// <returns>System.Int32.</returns>
        public int BeginReadDictionary(Type keyType, Type valueType)
        {
            return ReadSimpleValue<int>();
        }

        /// <summary>
        /// Ends the read dictionary.
        /// </summary>
        public void EndReadDictionary()
        {
        }

        /// <summary>
        /// Begins the read object array.
        /// </summary>
        /// <param name="valueType">Type of the value.</param>
        /// <returns>System.Int32.</returns>
        public int BeginReadObjectArray(Type valueType)
        {
            return ReadSimpleValue<int>();
        }

        /// <summary>
        /// Ends the read object array.
        /// </summary>
        public void EndReadObjectArray()
        {
        }

        /// <summary>
        /// Begins the read multi dimensional array.
        /// </summary>
        /// <param name="dimension">The dimension.</param>
        /// <param name="count">The count.</param>
        public void BeginReadMultiDimensionalArray(out int dimension, out int count)
        {
            //var dimensions = storage.ReadValue<int>("dimensions");
            //var totalLength = storage.ReadValue<int>("length");
            dimension = ReadSimpleValue<int>();
            count = ReadSimpleValue<int>();
        }

        /// <summary>
        /// Ends the read multi dimensional array.
        /// </summary>
        public void EndReadMultiDimensionalArray()
        {
        }

        /// <summary>
        /// Reads the array dimension.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>System.Int32.</returns>
        public int ReadArrayDimension(int index)
        {
            // //.ReadValue<int>("dim_len" + item);
            return ReadSimpleValue<int>();
        }

        /// <summary>
        /// Begins the read list.
        /// </summary>
        /// <param name="valueType">Type of the value.</param>
        /// <returns>System.Int32.</returns>
        public int BeginReadList(Type valueType)
        {
            return ReadSimpleValue<int>();
        }

        /// <summary>
        /// Ends the read list.
        /// </summary>
        public void EndReadList()
        {
        }

        /// <summary>
        /// Begins the read object.
        /// </summary>
        /// <param name="isReference">if set to <c>true</c> [is reference].</param>
        /// <returns>System.Int32.</returns>
        public int BeginReadObject(out bool isReference)
        {
            int result;
            var knownType = ReadSimpleValue<char>();
            if (knownType == 'X')
            {
                isReference = false;
                return -1;
            }
            if (knownType == 'O')
            {
                result = -1;

                isReference = false;
            }
            else {
                result = ReadSimpleValue<int>();
                isReference = true;
            }

            return result;
        }

        #endregion reading

        #region do nothing methods

        /// <summary>
        /// Ends the write object array.
        /// </summary>
        public void EndWriteObjectArray()
        {
        }

        /// <summary>
        /// Ends the write list.
        /// </summary>
        public void EndWriteList()
        {
        }

        /// <summary>
        /// Ends the write dictionary.
        /// </summary>
        public void EndWriteDictionary()
        {
        }

        /// <summary>
        /// Begins the write dictionary key.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool BeginWriteDictionaryKey(int id, object value)
        {
            return false;
        }

        /// <summary>
        /// Ends the write dictionary key.
        /// </summary>
        public void EndWriteDictionaryKey()
        {
        }

        /// <summary>
        /// Begins the write dictionary value.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool BeginWriteDictionaryValue(int id, object value)
        {
            return false;
        }

        /// <summary>
        /// Ends the write dictionary value.
        /// </summary>
        public void EndWriteDictionaryValue()
        {
        }

        /// <summary>
        /// Ends the multi dimension array.
        /// </summary>
        public void EndMultiDimensionArray()
        {
        }

        /// <summary>
        /// Ends the read object.
        /// </summary>
        public void EndReadObject()
        {
        }

        /// <summary>
        /// Begins the write list item.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool BeginWriteListItem(int index, object value)
        {
            return false;
        }

        /// <summary>
        /// Ends the write list item.
        /// </summary>
        public void EndWriteListItem()
        {
        }

        /// <summary>
        /// Begins the write object array item.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool BeginWriteObjectArrayItem(int index, object value)
        {
            return false;
        }

        /// <summary>
        /// Ends the write object array item.
        /// </summary>
        public void EndWriteObjectArrayItem()
        {
        }

        /// <summary>
        /// Ends the read properties.
        /// </summary>
        public void EndReadProperties()
        {
        }

        /// <summary>
        /// Ends the read fields.
        /// </summary>
        public void EndReadFields()
        {
        }

        /// <summary>
        /// Begins the read list item.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="entry">The entry.</param>
        /// <returns>System.Object.</returns>
        public object BeginReadListItem(int index, Entry entry)
        {
            return null;
        }

        /// <summary>
        /// Ends the read list item.
        /// </summary>
        public void EndReadListItem()
        {
        }

        /// <summary>
        /// Begins the read dictionary key item.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="entry">The entry.</param>
        /// <returns>System.Object.</returns>
        public object BeginReadDictionaryKeyItem(int index, Entry entry)
        {
            return null;
        }

        /// <summary>
        /// Ends the read dictionary key item.
        /// </summary>
        public void EndReadDictionaryKeyItem()
        {
        }

        /// <summary>
        /// Begins the read dictionary value item.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="entry">The entry.</param>
        /// <returns>System.Object.</returns>
        public object BeginReadDictionaryValueItem(int index, Entry entry)
        {
            return null;
        }

        /// <summary>
        /// Ends the read dictionary value item.
        /// </summary>
        public void EndReadDictionaryValueItem()
        {
        }

        /// <summary>
        /// Begins the read object array item.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="entry">The entry.</param>
        /// <returns>System.Object.</returns>
        public object BeginReadObjectArrayItem(int index, Entry entry)
        {
            return null;
        }

        /// <summary>
        /// Ends the read object array item.
        /// </summary>
        public void EndReadObjectArrayItem()
        {
        }

        /// <summary>
        /// Ends the write object.
        /// </summary>
        public void EndWriteObject()
        {
        }

        /// <summary>
        /// Begins the write property.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="type">The type.</param>
        public void BeginWriteProperty(string name, Type type)
        {
        }

        /// <summary>
        /// Ends the write property.
        /// </summary>
        public void EndWriteProperty()
        {
        }

        /// <summary>
        /// Begins the write field.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="type">The type.</param>
        public void BeginWriteField(string name, Type type)
        {
        }

        /// <summary>
        /// Ends the write field.
        /// </summary>
        public void EndWriteField()
        {
        }

        /// <summary>
        /// Ends the write properties.
        /// </summary>
        public void EndWriteProperties()
        {
        }

        /// <summary>
        /// Ends the write fields.
        /// </summary>
        public void EndWriteFields()
        {
        }

        /// <summary>
        /// Called when the last information about an object has been written
        /// </summary>
        /// <param name="entry">The object being written</param>
        public void FinishSerializing(Entry entry)
        {
        }

        /// <summary>
        /// Begins the read dictionary keys.
        /// </summary>
        public void BeginReadDictionaryKeys()
        {
        }

        /// <summary>
        /// Ends the read dictionary keys.
        /// </summary>
        public void EndReadDictionaryKeys()
        {
        }

        /// <summary>
        /// Begins the read dictionary values.
        /// </summary>
        public void BeginReadDictionaryValues()
        {
        }

        /// <summary>
        /// Ends the read dictionary values.
        /// </summary>
        public void EndReadDictionaryValues()
        {
        }

        /// <summary>
        /// Begins the write dictionary keys.
        /// </summary>
        public void BeginWriteDictionaryKeys()
        {
        }

        /// <summary>
        /// Ends the write dictionary keys.
        /// </summary>
        public void EndWriteDictionaryKeys()
        {
        }

        /// <summary>
        /// Begins the write dictionary values.
        /// </summary>
        public void BeginWriteDictionaryValues()
        {
        }

        /// <summary>
        /// Ends the write dictionary values.
        /// </summary>
        public void EndWriteDictionaryValues()
        {
        }

        /// <summary>
        /// Determines whether this instance has more.
        /// </summary>
        /// <returns><c>true</c> if this instance has more; otherwise, <c>false</c>.</returns>
        /// <exception cref="NotImplementedException"></exception>
        public bool HasMore()
        {
            throw new NotImplementedException();
        }

        #endregion do nothing methods

        //WIP
        /*public void Dispose()
        {
            if (_reader.BaseStream.CanRead)
                _reader.Close();
            if (_writer.BaseStream.CanWrite)
                _writer.Close();
        }*/
    }
}