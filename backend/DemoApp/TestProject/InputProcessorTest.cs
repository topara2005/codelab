using DemoApp.Application;
using DemoApp.Domain;

namespace TestProject
{
    [TestClass]
    public sealed class InputProcessorTest
    {
        private IInputProcessor processor = new InputProcessor();


        [TestMethod]
        public void StartProcessingInput_ShouldReturnValidToken()
        {
            // Arrange
            var requestId = "test-001";
            var input = "hello";

            // Act
            var token = processor.StartProcessingInput(requestId, input);

            // Assert
            Assert.IsNotNull(token);
            Assert.IsFalse(token.IsCancellationRequested);
        }

        [TestMethod]
        public void GetNextProcessedData_ShouldReturnCharactersAndPercentage()
        {
            // Arrange
            var requestId = "test-002";
            var input = "abc";
            processor.StartProcessingInput(requestId, input);

            // Act
            var (firstChar, firstPercentage) = processor.GetNextProcessedData(requestId);

            // Assert
            Assert.AreNotEqual('\0', firstChar);
            Assert.IsTrue(firstPercentage > 0);
            Assert.IsTrue(firstPercentage <= 100);
        }


        [TestMethod]
        public void GetNextProcessedData_ShouldShowProgressPercentage()
        {
            // Arrange
            var requestId = "test-004";
            var input = "test";
            processor.StartProcessingInput(requestId, input);

            // Act & Assert - verify percentage increases
            var previousPercentage = 0;
            var (character, percentage) = processor.GetNextProcessedData(requestId);

            while (percentage<100)
            {
                Assert.IsTrue(percentage >= previousPercentage,
                    $"Percentage should increase or stay same. Previous: {previousPercentage}, Current: {percentage}");
                previousPercentage = percentage;
                (character, percentage) = processor.GetNextProcessedData(requestId);
            }
        }

        [TestMethod]
        public void CancelProcessing_ShouldCancelTokenSuccessfully()
        {
            // Arrange
            var requestId = "test-005";
            var input = "hello";
            var token = processor.StartProcessingInput(requestId, input);

            // Act
            var result = processor.CancelProcessing(requestId);

            // Assert
            Assert.IsTrue(result);
            Assert.IsTrue(token.IsCancellationRequested);
        }

        [TestMethod]
        public void CancelProcessing_ShouldReturnFalseForNonExistentRequest()
        {
            // Arrange
            var requestId = "non-existent";

            // Act
            var result = processor!.CancelProcessing(requestId);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void GetToken_ShouldReturnValidTokenForExistingRequest()
        {
            // Arrange
            var requestId = "test-006";
            var input = "test";
            var originalToken = processor.StartProcessingInput(requestId, input);

            // Act
            var retrievedToken = processor.GetToken(requestId);

            // Assert
            Assert.AreEqual(originalToken, retrievedToken);
        }

        
        [TestMethod]
        public void MultipleRequests_ShouldBeHandledIndependently()
        {
            // Arrange
            var requestId1 = "test-007";
            var requestId2 = "test-008";
            var input1 = "hello";
            var input2 = "world";

            // Act
            var token1 = processor.StartProcessingInput(requestId1, input1);
            var token2 = processor.StartProcessingInput(requestId2, input2);

            processor.CancelProcessing(requestId1);

            // Assert
            Assert.IsTrue(token1.IsCancellationRequested);
            Assert.IsFalse(token2.IsCancellationRequested);
        }
    }
}
