﻿namespace LdifHelper;

using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Represents a mod-spec operation for an RFC2849 change-modify record.
/// </summary>
public class ModSpec : IEnumerable<object>
{
    /// <summary>
    /// Represents the attribute values for the mod-spec operation.
    /// </summary>
    private readonly List<object> attributeValues;

    /// <summary>
    /// Initializes a new instance of the <see cref="ModSpec"/> class.
    /// </summary>
    /// <param name="modSpecType">The type of mod-spec operation.</param>
    /// <param name="attributeType">The attribute type associated with the mod-spec operation.</param>
    /// <param name="attributeValues">The attribute values associated with the mod-spec operation.</param>
    public ModSpec(ModSpecType modSpecType, string attributeType, IEnumerable<object> attributeValues):this(modSpecType, attributeType,attributeType, attributeValues){}


    /// <summary>
    /// Initializes a new instance of the <see cref="ModSpec"/> class.
    /// </summary>
    /// <param name="modSpecType">The type of mod-spec operation.</param>
    /// <param name="attributeType">The attribute type associated with the mod-spec operation.</param>
    /// <param name="attributeOptionType">The attribute type associated with the mod-spec operation for binary option.</param>
    /// <param name="attributeValues">The attribute values associated with the mod-spec operation.</param>
    public ModSpec(ModSpecType modSpecType, string attributeType, string attributeOptionType, IEnumerable<object> attributeValues)
    {
        if (!Enum.IsDefined(typeof(ModSpecType), modSpecType))
        {
            throw new ArgumentOutOfRangeException(nameof(modSpecType), $"Unknown mod-spec \"{modSpecType}\".");
        }

        if (attributeType is null)
        {
            throw new ArgumentNullException(nameof(attributeType), "The attribute type can not be null.");
        }

        if (string.IsNullOrWhiteSpace(attributeType))
        {
            throw new ArgumentOutOfRangeException(nameof(attributeType), "The attribute type can not be empty or whitespace.");
        }

        this.ModSpecType = modSpecType;
        this.AttributeType = attributeType;
        AttributeOptionType = attributeOptionType;
        this.attributeValues = attributeValues is null ? new List<object>() : new List<object>(attributeValues);

        if (this.ModSpecType == ModSpecType.Add
            && this.attributeValues.Count == 0)
        {
            throw new ArgumentOutOfRangeException(nameof(attributeValues), "At least one attribute value must be present with an Add mod-spec.");
        }
    }

    /// <summary>
    /// Gets the attribute type for the mod-spec operation.
    /// </summary>
    /// <value>The attribute type for the mod-spec operation.</value>
    public string AttributeType { get; }
    /// <summary>
    /// Gets the attribute type for the mod-spec operation for binary option.
    /// </summary>
    /// <value>The attribute type for the mod-spec operation for binary option.</value>
    public string AttributeOptionType { get; } = string.Empty;

    /// <summary>
    /// Gets the attribute values for the mod-spec operation.
    /// </summary>
    /// <value>The attribute values for the mod-spec operation.</value>
    public IEnumerable<object> AttributeValues => this.attributeValues;

    /// <summary>
    /// Gets the number of attribute values in the mod-spec operation.
    /// </summary>
    /// <value>The number of attribute values in the mod-spec operation.</value>
    public int Count => this.attributeValues.Count;

    /// <summary>
    /// Gets the type of mod-spec operation.
    /// </summary>
    /// <value>The type of mod-spec operation.</value>
    public ModSpecType ModSpecType { get; }

    /// <summary>
    /// Returns an enumerator that iterates through the attribute values in the mod-spec operation.
    /// </summary>
    /// <returns>An enumerator that can be used to iterate through the attribute values in the mod-spec operation.</returns>
    public IEnumerator<object> GetEnumerator() => this.attributeValues.GetEnumerator();

    /// <summary>
    /// Returns an enumerator that iterates through the attribute values in the mod-spec operation.
    /// </summary>
    /// <returns>An enumerator that can be used to iterate through the attribute values in the mod-spec operation.</returns>
    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

    /// <summary>
    /// Returns a string that represents the current object.
    /// </summary>
    /// <returns>A string that represents the current object.</returns>
    public override string ToString() => $"{this.ModSpecType}<{this.AttributeType}>";
}