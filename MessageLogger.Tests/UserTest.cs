using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessageLogger.Data;

namespace MessageLogger.Tests
{
    public class UserTest
    {
        [Fact]
        public void IsCreated_WithConstructor()
        {
            User user = new User("Megan McMahon", "mmcmahon");

            Assert.Equal("Megan McMahon", user.Name);
            Assert.Equal("mmcmahon", user.Username);
            Assert.Equal(new List<Message>(), user.Messages);
        }
    }
}
