using MessageLogger.Data;

namespace MessageLogger.Tests
{
    public class MessageTest
    {
        [Fact]
        public void IsCreated_WithConstructor()
        {
            Message message = new Message("This is a message");

            Assert.Equal("This is a message", message.Content);
            Assert.IsType<DateTime>(message.CreatedAt);
        }
    }
}