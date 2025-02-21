using FinanceApp.Domain.Entities;

namespace FinanceApp.Domain.Tests.Entities;

public class BaseEntityTests
{
    private class TestEntity : BaseEntity { }
    private class AnotherEntity : BaseEntity { }

    [Fact]
    public void Equals_ShouldReturnTrue_WhenEntitiesHaveSameIdAndType()
    {
        var id = Guid.NewGuid();
        var entity1 = new TestEntity { Id = id };
        var entity2 = new TestEntity { Id = id };

        Assert.Equal(entity1, entity2);
    }

    [Fact]
    public void Equals_ShouldReturnFalse_WhenEntitiesHaveDifferentIds()
    {
        var entity1 = new TestEntity { Id = Guid.NewGuid() };
        var entity2 = new TestEntity { Id = Guid.NewGuid() };

        Assert.NotEqual(entity1, entity2);
    }

    [Fact]
    public void Equals_ShouldReturnFalse_WhenEntitiesAreDifferentTypesEvenIfIdIsSame()
    {
        var id = Guid.NewGuid();
        var entity1 = new TestEntity { Id = id };
        var entity2 = new AnotherEntity { Id = id };

        Assert.False(entity1.Equals(entity2));
    }

    [Fact]
    public void Equals_ShouldReturnFalse_WhenComparedWithNull()
    {
        var entity = new TestEntity { Id = Guid.NewGuid() };

        Assert.False(entity.Equals(null));
    }

    [Fact]
    public void GetHashCode_ShouldBeSame_ForEntitiesWithSameIdAndType()
    {
        var id = Guid.NewGuid();
        var entity1 = new TestEntity { Id = id };
        var entity2 = new TestEntity { Id = id };

        Assert.Equal(entity1.GetHashCode(), entity2.GetHashCode());
    }

    [Fact]
    public void GetHashCode_ShouldBeDifferent_ForEntitiesWithDifferentIds()
    {
        var entity1 = new TestEntity { Id = Guid.NewGuid() };
        var entity2 = new TestEntity { Id = Guid.NewGuid() };

        Assert.NotEqual(entity1.GetHashCode(), entity2.GetHashCode());
    }

    [Fact]
    public void HashSet_ShouldNotAllowDuplicates_WhenEntitiesHaveSameIdAndType()
    {
        var id = Guid.NewGuid();
        var entity1 = new TestEntity { Id = id };
        var entity2 = new TestEntity { Id = id };

        var set = new HashSet<BaseEntity> { entity1, entity2 };

        Assert.Single(set);
    }

    [Fact]
    public void HashSet_ShouldTreatEntitiesAsDifferent_WhenTheyHaveDifferentIds()
    {
        var entity1 = new TestEntity { Id = Guid.NewGuid() };
        var entity2 = new TestEntity { Id = Guid.NewGuid() };

        var set = new HashSet<BaseEntity> { entity1, entity2 };

        Assert.Equal(2, set.Count);
    }
}
