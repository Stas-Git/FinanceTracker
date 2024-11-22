using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Expense_Tracker.Controllers;
using Expense_Tracker.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using System.Linq;

namespace Expense_Tracker.Tests
{
    public class TransactionControllerTests
    {
        private readonly Mock<ApplicationDbContext> _mockContext;
        private readonly TransactionController _controller;

        public TransactionControllerTests()
        {
            // Мокування DbSet для Transactions
            var mockDbSet = new Mock<DbSet<Transaction>>();
            var mockCategorySet = new Mock<DbSet<Category>>();

            // Мокування даних для тесту
            var transactions = new List<Transaction>
            {
                new Transaction { TransactionId = 1, Amount = 100, CategoryId = 1, Note = "Test", Date = System.DateTime.Now },
                new Transaction { TransactionId = 2, Amount = 200, CategoryId = 2, Note = "Test 2", Date = System.DateTime.Now }
            }.AsQueryable();

            mockDbSet.As<IQueryable<Transaction>>().Setup(m => m.Provider).Returns(transactions.Provider);
            mockDbSet.As<IQueryable<Transaction>>().Setup(m => m.Expression).Returns(transactions.Expression);
            mockDbSet.As<IQueryable<Transaction>>().Setup(m => m.ElementType).Returns(transactions.ElementType);
            mockDbSet.As<IQueryable<Transaction>>().Setup(m => m.GetEnumerator()).Returns(transactions.GetEnumerator());

            var mockCategory = new List<Category>
            {
                new Category { CategoryId = 1, Title = "Food" },
                new Category { CategoryId = 2, Title = "Transport" }
            }.AsQueryable();

            mockCategorySet.As<IQueryable<Category>>().Setup(m => m.Provider).Returns(mockCategory.Provider);
            mockCategorySet.As<IQueryable<Category>>().Setup(m => m.Expression).Returns(mockCategory.Expression);
            mockCategorySet.As<IQueryable<Category>>().Setup(m => m.ElementType).Returns(mockCategory.ElementType);
            mockCategorySet.As<IQueryable<Category>>().Setup(m => m.GetEnumerator()).Returns(mockCategory.GetEnumerator());

            _mockContext = new Mock<ApplicationDbContext>();
            _mockContext.Setup(c => c.Transactions).Returns(mockDbSet.Object);
            _mockContext.Setup(c => c.Categories).Returns(mockCategorySet.Object);

            _controller = new TransactionController(_mockContext.Object);
        }

        [Fact]
        public async Task Index_ReturnsAViewResult_WithAListOfTransactions()
        {
            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<List<Transaction>>(viewResult.Model);
            Assert.Equal(2, model.Count);
        }

        [Fact]
        public void AddOrEdit_ReturnsViewResult_WhenIdIsZero()
        {
            // Act
            var result = _controller.AddOrEdit(0);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<Transaction>(viewResult.Model);
            Assert.Equal(0, model.TransactionId);
        }

        [Fact]
        public void AddOrEdit_ReturnsViewResult_WhenIdIsNotZero()
        {
            // Act
            var result = _controller.AddOrEdit(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<Transaction>(viewResult.Model);
            Assert.Equal(1, model.TransactionId);
        }

        [Fact]
        public async Task AddOrEdit_Post_ReturnsRedirectToActionResult_WhenModelIsValid()
        {
            var transaction = new Transaction { TransactionId = 0, Amount = 100, CategoryId = 1, Note = "Test", Date = System.DateTime.Now };

            var result = await _controller.AddOrEdit(transaction);

            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task DeleteConfirmed_RemovesTransactionAndRedirectsToIndex()
        {
            // Arrange
            var transactionId = 1;
            var transaction = new Transaction { TransactionId = transactionId, Amount = 100, CategoryId = 1, Note = "Test", Date = System.DateTime.Now };

            _mockContext.Setup(c => c.Transactions.FindAsync(transactionId)).ReturnsAsync(transaction);

            // Act
            var result = await _controller.DeleteConfirmed(transactionId);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }
    }
}
