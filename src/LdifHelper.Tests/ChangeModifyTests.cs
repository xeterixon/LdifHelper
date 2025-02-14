﻿namespace LdifHelper.Tests;

using System;
using System.Linq;
using System.Text;
using Xunit;

/// <summary>
/// Represents change type modify tests.
/// </summary>
public class ChangeModifyTests
{
    private const string DistinguishedName = "CN=Leonardo Pisano Bigollo,OU=users,DC=company,DC=com";

    private static readonly ModSpec[] ModSpecs = {new(ModSpecType.Add, "description", new object[] {"Contractor"})};

    /// <summary>
    /// Ensures the constructor rejects an empty distinguished name.
    /// </summary>
    [Fact]
    public void CtorParameterDistinguishedNameEmptyThrows() =>
        Assert.Throws<ArgumentOutOfRangeException>(() => new ChangeModify(string.Empty, ModSpecs));

    /// <summary>
    /// Ensures the constructor rejects a null distinguished name.
    /// </summary>
    [Fact]
    public void CtorParameterDistinguishedNameNullThrows() =>
        Assert.Throws<ArgumentNullException>(() => new ChangeModify(null, ModSpecs));

    /// <summary>
    /// Ensures the constructor rejects a white space distinguished name.
    /// </summary>
    [Fact]
    public void CtorParameterDistinguishedNameWhiteSpaceThrows() =>
        Assert.Throws<ArgumentOutOfRangeException>(() => new ChangeModify(" ", ModSpecs));

    /// <summary>
    /// Ensures the constructor rejects an empty spec collection.
    /// </summary>
    [Fact]
    public void CtorParameterModSpecsEmptyThrows() =>
        Assert.Throws<ArgumentOutOfRangeException>(() => new ChangeModify(DistinguishedName, Array.Empty<ModSpec>()));

    /// <summary>
    /// Ensures the constructor rejects a null mod spec collection.
    /// </summary>
    [Fact]
    public void CtorParameterModSpecsNullThrows() =>
        Assert.Throws<ArgumentNullException>(() => new ChangeModify(DistinguishedName, null));

    /// <summary>
    /// Ensures the Count property is valid.
    /// </summary>
    [Fact]
    public void PropertyCountIsValid()
    {
        // Act.
        var sut = new ChangeModify(
            DistinguishedName,
            new[]
            {
                new ModSpec(ModSpecType.Add, "description", new object[] { "Contractor" }),
                new ModSpec(ModSpecType.Replace, "telephonenumber", new object[] { "+1 (415) 555 1234" })
            });

        // Arrange.
        Assert.Equal(2, sut.Count);
    }

    /// <summary>
    /// Ensures the DistinguishedName property is valid.
    /// </summary>
    [Fact]
    public void PropertyDistinguishedNameIsValid()
    {
        // Act.
        var sut = new ChangeModify(DistinguishedName, ModSpecs);

        // Arrange.
        Assert.Equal(DistinguishedName, sut.DistinguishedName);
    }

    /// <summary>
    /// Ensures the Values property is valid.
    /// </summary>
    [Fact]
    public void PropertyValuesIsValid()
    {
        // Arrange.
        const string attributeTypeDescription = "description";
        object[] attributeValuesDescription = { "Contractor" };
        var modSpecDescription = new ModSpec(ModSpecType.Add, attributeTypeDescription, attributeValuesDescription);

        const string attributeTypeTelephoneNumber = "telephonenumber";
        object[] attributeValuesTelephoneNumber = { "+1 (415) 555 1234" };
        var modSpecTelephoneNumber = new ModSpec(ModSpecType.Replace, attributeTypeTelephoneNumber, attributeValuesTelephoneNumber);

        // Act.
        var sut = new ChangeModify(DistinguishedName, new[] { modSpecDescription, modSpecTelephoneNumber });
        var sutModSpecDescription = sut.ModSpecs.First(x => x.AttributeType.Equals(attributeTypeDescription, StringComparison.Ordinal));
        var sutModSpecTelephoneNumber = sut.ModSpecs.First(x => x.AttributeType.Equals(attributeTypeTelephoneNumber, StringComparison.Ordinal));

        // Assert.
        Assert.Equal(2, sut.ModSpecs.ToArray().Length);

        Assert.Equal(attributeTypeDescription, sutModSpecDescription.AttributeType);
        Assert.Equal(attributeValuesDescription, sutModSpecDescription.AttributeValues);

        Assert.Equal(attributeTypeTelephoneNumber, sutModSpecTelephoneNumber.AttributeType);
        Assert.Equal(attributeValuesTelephoneNumber, sutModSpecTelephoneNumber.AttributeValues);
    }

    /// <summary>
    /// Ensures a single value is added to the specified attribute type.
    /// </summary>
    [Fact]
    public void ShouldAddOneValue()
    {
        // Arrange.
        ModSpec[] modSpec = { new(ModSpecType.Add, "postaladdress", new object[] {"2400 Fulton St, San Francisco, CA 94118, USA" })};
        var dump = string.Join(
            Environment.NewLine,
            "dn: CN=Leonardo Pisano Bigollo,OU=users,DC=company,DC=com",
            "changetype: modify",
            "add: postaladdress",
            "postaladdress: 2400 Fulton St, San Francisco, CA 94118, USA",
            "-",
            string.Empty);

        // Act.
        var sut = new ChangeModify(DistinguishedName, modSpec);

        // Assert.
        Assert.Equal(dump, sut.Dump());
    }

    /// <summary>
    /// Ensures the object is an enumerable of type <see cref="ModSpec"/>s.
    /// </summary>
    [Fact]
    public void ShouldBeEnumerable()
    {
        // Arrange.
        const string attributeTypeDescription = "description";
        object[] attributeValuesDescription = { "Contractor" };
        var modSpecDescription = new ModSpec(ModSpecType.Add, attributeTypeDescription, attributeValuesDescription);

        const string attributeTypeTelephoneNumber = "telephonenumber";
        object[] attributeValuesTelephoneNumber = { "+1 (415) 555 1234" };
        var modSpecTelephoneNumber = new ModSpec(ModSpecType.Replace, attributeTypeTelephoneNumber, attributeValuesTelephoneNumber);

        // Act.
        var sut = new ChangeModify(DistinguishedName, new[] { modSpecDescription, modSpecTelephoneNumber });
        var sutModSpecDescription = sut.First(x => x.AttributeType.Equals(attributeTypeDescription, StringComparison.Ordinal));
        var sutModSpecTelephoneNumber = sut.First(x => x.AttributeType.Equals(attributeTypeTelephoneNumber, StringComparison.Ordinal));

        // Assert.
        Assert.Equal(2, sut.ToArray().Length);

        Assert.Equal(attributeTypeDescription, sutModSpecDescription.AttributeType);
        Assert.Equal(attributeValuesDescription, sutModSpecDescription.AttributeValues);

        Assert.Equal(attributeTypeTelephoneNumber, sutModSpecTelephoneNumber.AttributeType);
        Assert.Equal(attributeValuesTelephoneNumber, sutModSpecTelephoneNumber.AttributeValues);
    }

    /// <summary>
    /// Ensures an entire attribute type is scheduled to be removed.
    /// </summary>
    [Fact]
    public void ShouldDeleteAllValues()
    {
        // Arrange.
        ModSpec[] modSpec = { new(ModSpecType.Delete, "description", null) };
        var dump = string.Join(
            Environment.NewLine,
            "dn: CN=Leonardo Pisano Bigollo,OU=users,DC=company,DC=com",
            "changetype: modify",
            "delete: description",
            "-",
            string.Empty);

        // Act.
        var sut = new ChangeModify(DistinguishedName, modSpec);

        // Assert.
        Assert.Equal(dump, sut.Dump());
    }

    /// <summary>
    /// Ensures a single value for the specified attribute type is scheduled to be removed.
    /// </summary>
    [Fact]
    public void ShouldDeleteSingleValues()
    {
        // Arrange.
        ModSpec[] modSpec = { new(ModSpecType.Delete, "description", new object[] { "Contractor" }) };
        var dump = string.Join(
            Environment.NewLine,
            "dn: CN=Leonardo Pisano Bigollo,OU=users,DC=company,DC=com",
            "changetype: modify",
            "delete: description",
            "description: Contractor",
            "-",
            string.Empty);

        // Act.
        var sut = new ChangeModify(DistinguishedName, modSpec);

        // Assert.
        Assert.Equal(dump, sut.Dump());
    }

    /// <summary>
    /// Ensures an entire attribute type is scheduled to be replaced with no new values.
    /// </summary>
    [Fact]
    public void ShouldReplaceAllValues()
    {
        // Arrange.
        ModSpec[] modSpec = { new(ModSpecType.Replace, "telephonenumber", null) };
        var dump = string.Join(
            Environment.NewLine,
            "dn: CN=Leonardo Pisano Bigollo,OU=users,DC=company,DC=com",
            "changetype: modify",
            "replace: telephonenumber",
            "-",
            string.Empty);

        // Act.
        var sut = new ChangeModify(DistinguishedName, modSpec);

        // Assert.
        Assert.Equal(dump, sut.Dump());
    }

    /// <summary>
    /// Ensures an attribute type is replaced with a single value.
    /// </summary>
    [Fact]
    public void ShouldReplaceWithSingleValue()
    {
        // Arrange.
        ModSpec[] modSpec = { new(ModSpecType.Replace, "telephonenumber", new object[] { "+1 (415) 555 1234" }) };
        var dump = string.Join(
            Environment.NewLine,
            "dn: CN=Leonardo Pisano Bigollo,OU=users,DC=company,DC=com",
            "changetype: modify",
            "replace: telephonenumber",
            "telephonenumber: +1 (415) 555 1234",
            "-",
            string.Empty);

        // Act.
        var sut = new ChangeModify(DistinguishedName, modSpec);

        // Assert.
        Assert.Equal(dump, sut.Dump());
    }
    [Fact]
    public void ChangeTypeIsValid()
    {
        // Arrange.
        ModSpec[] modSpec = { new(ModSpecType.Replace, "telephonenumber", null) };
        var dump = string.Join(
            Environment.NewLine,
            "dn: CN=Leonardo Pisano Bigollo,OU=users,DC=company,DC=com",
            "changetype: modify",
            "replace: telephonenumber",
            "-",
            string.Empty);

        // Act.
        IChangeRecord record = new ChangeModify(DistinguishedName, modSpec);
        Assert.Equal(ChangeType.Modify, record.Change);
    }
    [Fact]
    public void ShouldParseBinaryOption()
    {
        // Arrange.
        ModSpec[] modSpec = { new(ModSpecType.Delete, "userCertificate","userCertificate;binary", new object[] { Encoding.ASCII.GetBytes("Random binary data") }) };
        var dump = string.Join(
            Environment.NewLine,
            "dn: CN=Leonardo Pisano Bigollo,OU=users,DC=company,DC=com",
            "changetype: modify",
            "delete: userCertificate",
            "userCertificate;binary:: UmFuZG9tIGJpbmFyeSBkYXRh",
            "-",
            string.Empty);

        // Act.
        IChangeRecord record = new ChangeModify(DistinguishedName, modSpec);
        var rdump = record.Dump();
        Assert.Equal(dump,rdump);
    }
}