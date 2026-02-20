using BaseApi.Extensions;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace BaseApi.UnitTests.Extensions;

public class LinqExtensionsTests
{
    private class TestEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    [Fact]
    public void WhereIf_AppliesPredicate_WhenConditionIsTrue()
    {
        // Arrange
        var list = new List<TestEntity>
        {
            new TestEntity { Id = 1, Name = "A" },
            new TestEntity { Id = 2, Name = "B" }
        }.AsQueryable();

        // Act
        var result = list.WhereIf(true, x => x.Name == "A").ToList();

        // Assert
        result.Should().HaveCount(1);
        result.First().Name.Should().Be("A");
    }

    [Fact]
    public void WhereIf_DoesNotApplyPredicate_WhenConditionIsFalse()
    {
        // Arrange
        var list = new List<TestEntity>
        {
            new TestEntity { Id = 1, Name = "A" },
            new TestEntity { Id = 2, Name = "B" }
        }.AsQueryable();

        // Act
        var result = list.WhereIf(false, x => x.Name == "A").ToList();

        // Assert
        result.Should().HaveCount(2);
    }

    [Fact]
    public void PageBy_SkipsAndTakesCorrectly()
    {
        // Arrange
        var list = Enumerable.Range(1, 10).Select(x => new TestEntity { Id = x }).AsQueryable();

        // Act
        var result = list.PageBy(2, 3).ToList();

        // Assert
        result.Should().HaveCount(3);
        result.Select(x => x.Id).Should().BeEquivalentTo(new[] { 3, 4, 5 });
    }

    [Fact]
    public void OrderByIf_AppliesOrdering_WhenConditionIsTrue()
    {
        // Arrange
        var list = new List<TestEntity>
        {
            new TestEntity { Id = 2, Name = "B" },
            new TestEntity { Id = 1, Name = "A" }
        }.AsQueryable();

        // Act
        var result = list.OrderByIf(true, x => x.Id).ToList();

        // Assert
        result.First().Id.Should().Be(1);
        result.Last().Id.Should().Be(2);
    }
    
    [Fact]
    public void OrderByIf_DoesNotApplyOrdering_WhenConditionIsFalse()
    {
        // Arrange
        var list = new List<TestEntity>
        {
            new TestEntity { Id = 2, Name = "B" },
            new TestEntity { Id = 1, Name = "A" }
        }.AsQueryable();

        // Act
        var result = list.OrderByIf(false, x => x.Id).ToList();

        // Assert
        result.First().Id.Should().Be(2);
        result.Last().Id.Should().Be(1);
    }
}
