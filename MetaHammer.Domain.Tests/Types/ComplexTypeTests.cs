using MetaHammer.Domain.Types;
using MetaHammer.Domain.Types.Enums;

namespace MetaHammer.Domain.Tests.Types;

public class ComplexTypeTests
{
    public MetaType stringType { get; set; }
    public MetaType intType { get; set; }
    public MetaType decimalType { get; set; }
    public MetaType dateTimeType { get; set; }
    public MetaType booleanType { get; set; }
    
    
    public ComplexTypeTests()
    {
        var stringTypeName = "String";
        var intTypeName = "Int";
        var decimalTypeName = "Decimal";
        var dateTimeTypeName = "DateTime";
        var booleanTypeName = "Boolean";
        
        stringType = new MetaType(Guid.NewGuid(), stringTypeName, MetaTypeNature.Primitive, isNative:true);
        intType = new MetaType(Guid.NewGuid(), intTypeName, MetaTypeNature.Primitive, isNative:true);
        decimalType = new MetaType(Guid.NewGuid(), decimalTypeName, MetaTypeNature.Primitive, isNative:true);
        dateTimeType = new MetaType(Guid.NewGuid(), dateTimeTypeName, MetaTypeNature.Primitive, isNative:true);
        booleanType = new MetaType(Guid.NewGuid(), booleanTypeName, MetaTypeNature.Primitive, isNative:true);
    }
    
    [Fact]
    public void CreatePrimitiveType()
    {
        var primitiveType = new MetaType(Guid.NewGuid(), "String", MetaTypeNature.Primitive, isNative:true);
        
        Assert.NotNull(primitiveType);
    }
    [Fact]
    public void CreateValueObjectType_AddStringProperties()
    {
        var valueObjectType = new MetaType(Guid.NewGuid(), "Address", MetaTypeNature.ValueObject);
        
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
        var complexType = new MetaType(Guid.NewGuid(), "Person", MetaTypeNature.Complex);
        
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
        var person = new MetaType(Guid.NewGuid(), "Person", MetaTypeNature.Complex);
        
        person.AddProperty("first_name", stringType);
        person.AddProperty("last_name", stringType);
        person.AddProperty("birth_date", dateTimeType);

        var constructor = person.AddConstructor();
        constructor.AddParameter("first_name", stringType);
        constructor.AddParameter("last_name", stringType);

        var testMethod1 = person.AddMethod("GetFullName", stringType);
    }
}
