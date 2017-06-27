using System;

namespace Serialization
{
    /// <summary>
    /// Interface IStorage
    /// </summary>
    public interface IStorage
    {
        /// <summary>
        /// Starts the serialization process, the serializer should initialize and wait for data
        /// </summary>
        void StartSerializing();

        /// <summary>
        /// Called when serialization is complete, should return the data or a key
        /// encoded as a byte array that will be used to reinitialize the serializer
        /// later
        /// </summary>
        /// <returns></returns>
        void FinishedSerializing();

        /// <summary>
        /// Called when deserialization is complete, so that resources may be released
        /// </summary>
        void FinishedDeserializing();

        /// <summary>
        /// Called when serializing a new object, the Entry parameter may have MustHaveName set
        /// when this is true the name must be persisted as is so that the property or field can
        /// be set when retrieving the data.
        /// If this routine returns TRUE then no further processing is executed and the object
        /// is presumed persisted in its entirety
        /// </summary>
        /// <returns>Normally FALSE.  True if the object is already fully persisted</returns>
        /// <param name="entry">The item being serialized</param>
        bool StartSerializing(Entry entry, int id);

        /// <summary>
        /// Called when the last information about an object has been written
        /// </summary>
        /// <param name="entry">The object being written</param>
        void FinishSerializing(Entry entry);

        /// <summary>
        /// Called when deserializing an object.  If the Entry parameter has MustHaveName set then
        /// the routine should return with the Entry parameter updated with the name and
        /// the type of the object in StoredType
        /// If  the storage is capable of fully recreating the object then this routine should return
        /// the fully constructed object, and no further processing will occur.  Not this does mean
        /// that it must handle its own references for previously seen objects
        /// This will be called after DeserializeGetName
        /// </summary>
        /// <returns>Normally NULL, it may also return a fully depersisted object</returns>
        /// <param name="entry"></param>
        object StartDeserializing(Entry entry);

        /// <summary>
        /// Called to allow the storage to retrieve the name of the item being deserialized
        /// All entries must be named before a call to StartDeserializing, this enables
        /// the system to fill out the property setter and capture default stored type
        /// information before deserialization commences
        /// </summary>
        /// <param name="entry">The entry whose name should be filled in</param>
        void DeserializeGetName(Entry entry);

        /// <summary>
        /// Called when an object has deserialization complete
        /// </summary>
        /// <param name="entry"></param>
        void FinishDeserializing(Entry entry);

        /// <summary>
        /// Reads a simple type (or array of bytes) from storage
        /// </summary>
        /// <param name="name">The name of the item</param>
        /// <param name="type">The type to be read</param>
        /// <returns></returns>

        Entry[] ShouldWriteFields(Entry[] fields);

        /// <summary>
        /// Shoulds the write properties.
        /// </summary>
        /// <param name="properties">The properties.</param>
        /// <returns>Entry[].</returns>
        Entry[] ShouldWriteProperties(Entry[] properties);

        /// <summary>
        /// Starts the deserializing.
        /// </summary>
        void StartDeserializing();

        #region reading

        /// <summary>
        /// Determines whether this instance has more.
        /// </summary>
        /// <returns><c>true</c> if this instance has more; otherwise, <c>false</c>.</returns>
        bool HasMore();

        /// <summary>
        /// Begins the read property.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <returns>Entry.</returns>
        Entry BeginReadProperty(Entry entry);

        /// <summary>
        /// Ends the read property.
        /// </summary>
        void EndReadProperty();

        /// <summary>
        /// Begins the read field.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <returns>Entry.</returns>
        Entry BeginReadField(Entry entry);

        /// <summary>
        /// Ends the read field.
        /// </summary>
        void EndReadField();

        /// <summary>
        /// Begins the read properties.
        /// </summary>
        /// <returns>System.Int32.</returns>
        int BeginReadProperties();

        /// <summary>
        /// Begins the read fields.
        /// </summary>
        /// <returns>System.Int32.</returns>
        int BeginReadFields();

        /// <summary>
        /// Ends the read properties.
        /// </summary>
        void EndReadProperties();

        /// <summary>
        /// Ends the read fields.
        /// </summary>
        void EndReadFields();

        /// <summary>
        /// Reads the simple value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>T.</returns>
        T ReadSimpleValue<T>();

        /// <summary>
        /// Reads the simple value.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>System.Object.</returns>
        object ReadSimpleValue(Type type);

        /// <summary>
        /// Determines whether [is multi dimensional array] [the specified length].
        /// </summary>
        /// <param name="length">The length.</param>
        /// <returns><c>true</c> if [is multi dimensional array] [the specified length]; otherwise, <c>false</c>.</returns>
        bool IsMultiDimensionalArray(out int length);

        /// <summary>
        /// Begins the read multi dimensional array.
        /// </summary>
        /// <param name="dimension">The dimension.</param>
        /// <param name="count">The count.</param>
        void BeginReadMultiDimensionalArray(out int dimension, out int count);

        /// <summary>
        /// Ends the read multi dimensional array.
        /// </summary>
        void EndReadMultiDimensionalArray();

        /// <summary>
        /// Reads the array dimension.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>System.Int32.</returns>
        int ReadArrayDimension(int index);

        /// <summary>
        /// Reads the simple array.
        /// </summary>
        /// <param name="elementType">Type of the element.</param>
        /// <param name="count">The count.</param>
        /// <returns>Array.</returns>
        Array ReadSimpleArray(Type elementType, int count);

        //int BeginRead();

        /// <summary>
        /// Begins the read object.
        /// </summary>
        /// <param name="isReference">if set to <c>true</c> [is reference].</param>
        /// <returns>System.Int32.</returns>
        int BeginReadObject(out bool isReference);

        /// <summary>
        /// Ends the read object.
        /// </summary>
        void EndReadObject();

        /// <summary>
        /// Begins the read list.
        /// </summary>
        /// <param name="valueType">Type of the value.</param>
        /// <returns>System.Int32.</returns>
        int BeginReadList(Type valueType);

        /// <summary>
        /// Begins the read list item.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="entry">The entry.</param>
        /// <returns>System.Object.</returns>
        object BeginReadListItem(int index, Entry entry);

        /// <summary>
        /// Ends the read list item.
        /// </summary>
        void EndReadListItem();

        /// <summary>
        /// Ends the read list.
        /// </summary>
        void EndReadList();

        /// <summary>
        /// Begins the read dictionary.
        /// </summary>
        /// <param name="keyType">Type of the key.</param>
        /// <param name="valueType">Type of the value.</param>
        /// <returns>System.Int32.</returns>
        int BeginReadDictionary(Type keyType, Type valueType);

        /// <summary>
        /// Begins the read dictionary keys.
        /// </summary>
        void BeginReadDictionaryKeys();

        /// <summary>
        /// Begins the read dictionary key item.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="entry">The entry.</param>
        /// <returns>System.Object.</returns>
        object BeginReadDictionaryKeyItem(int index, Entry entry);

        /// <summary>
        /// Ends the read dictionary key item.
        /// </summary>
        void EndReadDictionaryKeyItem();

        /// <summary>
        /// Ends the read dictionary keys.
        /// </summary>
        void EndReadDictionaryKeys();

        /// <summary>
        /// Begins the read dictionary values.
        /// </summary>
        void BeginReadDictionaryValues();

        /// <summary>
        /// Begins the read dictionary value item.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="entry">The entry.</param>
        /// <returns>System.Object.</returns>
        object BeginReadDictionaryValueItem(int index, Entry entry);

        /// <summary>
        /// Ends the read dictionary value item.
        /// </summary>
        void EndReadDictionaryValueItem();

        /// <summary>
        /// Ends the read dictionary values.
        /// </summary>
        void EndReadDictionaryValues();

        /// <summary>
        /// Ends the read dictionary.
        /// </summary>
        void EndReadDictionary();

        /// <summary>
        /// Begins the read object array.
        /// </summary>
        /// <param name="valueType">Type of the value.</param>
        /// <returns>System.Int32.</returns>
        int BeginReadObjectArray(Type valueType);

        /// <summary>
        /// Begins the read object array item.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="entry">The entry.</param>
        /// <returns>System.Object.</returns>
        object BeginReadObjectArrayItem(int index, Entry entry);

        /// <summary>
        /// Ends the read object array item.
        /// </summary>
        void EndReadObjectArrayItem();

        /// <summary>
        /// Ends the read object array.
        /// </summary>
        void EndReadObjectArray();

        #endregion reading

        #region writing

        /// <summary>
        /// Begins the write object.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="wasSeen">if set to <c>true</c> [was seen].</param>
        void BeginWriteObject(int id, Type objectType, bool wasSeen);

        /// <summary>
        /// Ends the write object.
        /// </summary>
        void EndWriteObject();

        /// <summary>
        /// Begins the write list.
        /// </summary>
        /// <param name="count">The count.</param>
        /// <param name="listType">Type of the list.</param>
        void BeginWriteList(int count, Type listType);

        /// <summary>
        /// Begins the write list item.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool BeginWriteListItem(int index, object value);

        /// <summary>
        /// Ends the write list item.
        /// </summary>
        void EndWriteListItem();

        /// <summary>
        /// Ends the write list.
        /// </summary>
        void EndWriteList();

        /// <summary>
        /// Begins the write object array.
        /// </summary>
        /// <param name="count">The count.</param>
        /// <param name="arrayType">Type of the array.</param>
        void BeginWriteObjectArray(int count, Type arrayType);

        /// <summary>
        /// Begins the write object array item.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool BeginWriteObjectArrayItem(int index, object value);

        /// <summary>
        /// Ends the write object array item.
        /// </summary>
        void EndWriteObjectArrayItem();

        /// <summary>
        /// Ends the write object array.
        /// </summary>
        void EndWriteObjectArray();

        /// <summary>
        /// Begins the multi dimension array.
        /// </summary>
        /// <param name="arrayType">Type of the array.</param>
        /// <param name="dimensions">The dimensions.</param>
        /// <param name="count">The count.</param>
        void BeginMultiDimensionArray(Type arrayType, int dimensions, int count);

        /// <summary>
        /// Ends the multi dimension array.
        /// </summary>
        void EndMultiDimensionArray();

        /// <summary>
        /// Writes the array dimension.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="count">The count.</param>
        void WriteArrayDimension(int index, int count);

        /// <summary>
        /// Writes the simple array.
        /// </summary>
        /// <param name="count">The count.</param>
        /// <param name="array">The array.</param>
        void WriteSimpleArray(int count, Array array);

        /// <summary>
        /// Writes the simple value.
        /// </summary>
        /// <param name="value">The value.</param>
        void WriteSimpleValue(object value);

        // dictionaries
        /// <summary>
        /// Begins the write dictionary.
        /// </summary>
        /// <param name="count">The count.</param>
        /// <param name="dictionaryType">Type of the dictionary.</param>
        void BeginWriteDictionary(int count, Type dictionaryType);

        /// <summary>
        /// Begins the write dictionary keys.
        /// </summary>
        void BeginWriteDictionaryKeys();

        /// <summary>
        /// Begins the write dictionary key.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool BeginWriteDictionaryKey(int id, object value);

        /// <summary>
        /// Ends the write dictionary key.
        /// </summary>
        void EndWriteDictionaryKey();

        /// <summary>
        /// Ends the write dictionary keys.
        /// </summary>
        void EndWriteDictionaryKeys();

        /// <summary>
        /// Begins the write dictionary values.
        /// </summary>
        void BeginWriteDictionaryValues();

        /// <summary>
        /// Begins the write dictionary value.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool BeginWriteDictionaryValue(int id, object value);

        /// <summary>
        /// Ends the write dictionary value.
        /// </summary>
        void EndWriteDictionaryValue();

        /// <summary>
        /// Ends the write dictionary values.
        /// </summary>
        void EndWriteDictionaryValues();

        /// <summary>
        /// Ends the write dictionary.
        /// </summary>
        void EndWriteDictionary();

        // properties and fields
        /// <summary>
        /// Begins the write properties.
        /// </summary>
        /// <param name="count">The count.</param>
        void BeginWriteProperties(int count);

        /// <summary>
        /// Ends the write properties.
        /// </summary>
        void EndWriteProperties();

        /// <summary>
        /// Begins the write property.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="type">The type.</param>
        void BeginWriteProperty(string name, Type type);

        /// <summary>
        /// Ends the write property.
        /// </summary>
        void EndWriteProperty();

        /// <summary>
        /// Begins the write fields.
        /// </summary>
        /// <param name="count">The count.</param>
        void BeginWriteFields(int count);

        /// <summary>
        /// Ends the write fields.
        /// </summary>
        void EndWriteFields();

        /// <summary>
        /// Begins the write field.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="type">The type.</param>
        void BeginWriteField(string name, Type type);

        /// <summary>
        /// Ends the write field.
        /// </summary>
        void EndWriteField();

        /// <summary>
        /// Gets a value indicating whether [supports on demand].
        /// </summary>
        /// <value><c>true</c> if [supports on demand]; otherwise, <c>false</c>.</value>
        bool SupportsOnDemand { get; }

        /// <summary>
        /// Begins the on demand.
        /// </summary>
        /// <param name="id">The identifier.</param>
        void BeginOnDemand(int id);

        /// <summary>
        /// Ends the on demand.
        /// </summary>
        void EndOnDemand();

        #endregion writing
    }
}