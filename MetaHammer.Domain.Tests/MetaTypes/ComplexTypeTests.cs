using MetaHammer.Domain.Types;

namespace MetaHammer.Domain.Tests.Types;

public class ComplexTypeTests
{
    public PrimitiveMetaType stringType { get; set; }
    public PrimitiveMetaType intType { get; set; }
    public PrimitiveMetaType decimalType { get; set; }
    public PrimitiveMetaType dateTimeType { get; set; }
    public PrimitiveMetaType booleanType { get; set; }
    
    
    public ComplexTypeTests()
    {
        var stringTypeName = "String";
        var intTypeName = "Int";
        var decimalTypeName = "Decimal";
        var dateTimeTypeName = "DateTime";
        var booleanTypeName = "Boolean";
        
        stringType = new PrimitiveMetaType(Guid.NewGuid(), stringTypeName);
        intType = new PrimitiveMetaType(Guid.NewGuid(), intTypeName);
        decimalType = new PrimitiveMetaType(Guid.NewGuid(), decimalTypeName);
        dateTimeType = new PrimitiveMetaType(Guid.NewGuid(), dateTimeTypeName);
        booleanType = new PrimitiveMetaType(Guid.NewGuid(), booleanTypeName);
    }
    
    [Fact]
    public void CreatePrimitiveType()
    {
        var primitiveType = new PrimitiveMetaType(Guid.NewGuid(), "String");
        
        Assert.NotNull(primitiveType);
    }
    [Fact]
    public void CreateValueObjectType_AddStringProperties()
    {
        var valueObjectType = new ValueObjectMetaType(Guid.NewGuid(), "Address");
        
        valueObjectType.AddProperty("street", stringType);
        valueObjectType.AddProperty("city", stringType);
        valueObjectType.AddProperty("zip_code", intType);
        
        Assert.NotNull(valueObjectType);
        Assert.Contains(valueObjectType.Properties, p => p.Name == "street");
        Assert.Contains(valueObjectType.Properties, p => p.Name == "city");
        Assert.Contains(valueObjectType.Properties, p => p.Name == "zip_code");
        
        Assert.Equal(valueObjectType.Properties.First(x =>x.Name =="street").MetaType.Name, stringType.Name);
        Assert.Equal(valueObjectType.Properties.First(x =>x.Name =="city").MetaType.Name, stringType.Name);
        Assert.Equal(valueObjectType.Properties.First(x =>x.Name =="zip_code").MetaType.Name, intType.Name);
        
    }
    
    [Fact]
    public void CreateComplexType_AddStringProperties()
    {
        var complexType = new ComplexMetaType(Guid.NewGuid(), "Person");
        
        complexType.AddProperty("first_name", stringType);
        complexType.AddProperty("last_name", stringType);
        complexType.AddProperty("birth_date", dateTimeType);
        complexType.AddProperty("height", decimalType);
        complexType.AddProperty("is_employed", booleanType);
        
        Assert.NotNull(complexType);
        Assert.Contains(complexType.Properties, p => p.Name == "first_name");
        Assert.Contains(complexType.Properties, p => p.Name == "last_name");
        Assert.Contains(complexType.Properties, p => p.Name == "birth_date");
        Assert.Contains(complexType.Properties, p => p.Name == "height");
        Assert.Contains(complexType.Properties, p => p.Name == "is_employed");
        
        Assert.Equal(complexType.Properties.First(x =>x.Name =="first_name").MetaType.Name, stringType.Name);
        Assert.Equal(complexType.Properties.First(x =>x.Name =="last_name").MetaType.Name, stringType.Name);
        Assert.Equal(complexType.Properties.First(x =>x.Name =="birth_date").MetaType.Name, dateTimeType.Name);
        Assert.Equal(complexType.Properties.First(x =>x.Name =="height").MetaType.Name, decimalType.Name);
        Assert.Equal(complexType.Properties.First(x =>x.Name =="is_employed").MetaType.Name, booleanType.Name);
        
    }

    [Fact]
    public void ComplexType_CreateMethod()
    {
        var person = new ComplexMetaType(Guid.NewGuid(), "Person");
        
        person.AddProperty("first_name", stringType);
        person.AddProperty("last_name", stringType);
        person.AddProperty("birth_date", dateTimeType);

        var constructor = person.AddConstructor();
        constructor.AddParameter("first_name", stringType);
        constructor.AddParameter("last_name", stringType);

        var testMethod1 = person.AddMethod("GetFullName", stringType);
    }
}
